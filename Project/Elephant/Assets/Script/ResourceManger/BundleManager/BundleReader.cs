using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class BundleReader : MonoBehaviour {
    private static BundleReader instance = null;
    public static BundleReader GetInstance()
    {
        if(instance == null)
        {
            Debug.LogError("No Instance of BundleReader, you should attach BundleReader Component to a GameObject!");
        }
        return instance;
    }

    public Dictionary<string, List<string>> depMap = new Dictionary<string, List<string>>();

    public void InitDepMap()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(Application.dataPath + "/../../bundle/bundle.xml");
        XmlNode root = doc.SelectSingleNode("root");
        XmlNodeList assets = root.ChildNodes;
        foreach(XmlElement e in assets)
        {
            if(e.HasChildNodes)
            {
                string objName = e.GetAttribute("name");
                List<string> depnames = new List<string>();
                XmlNodeList deps = e.ChildNodes;
                foreach (XmlElement d in deps)
                {
                    string depName = d.GetAttribute("name");
                    depnames.Add(depName);
                }
                depMap.Add(objName, depnames);
            }
        }
    }

    public void Awake()
    {
        instance = this;
        InitDepMap();
    }

    public Dictionary<string, BundleHolder> bundles = new Dictionary<string, BundleHolder>();

    public void PrintBundles()
    {
        Debug.Log("======BundleInfo=====");
        foreach (BundleHolder bh in bundles.Values)
        {
            Debug.Log(bh.bundleName + ":" + bh.refCount);
        }
        Debug.Log("==================");
    }

    public void RemoveBundle(string path)
    {
        if(bundles.ContainsKey(path))
        {
            bundles.Remove(path);
        }
    }

    public void AddBundle(string path, BundleHolder bh)
    {
        bundles.Add(path, bh);
    }

	public void AsyncLoad(string bundlePath, string assetName, LoadDelegate lad)
    {
        StartCoroutine(AsyncLoadBundle(bundlePath, delegate (BundleHolder bh)
        {
            if (bh == null)
            {
                lad?.Invoke(null);
                return;
            }

            StartCoroutine(AsyncLoadAsset(bh.bundle, assetName, delegate (Object obj)
            {
                if (obj == null)
                {
                    lad?.Invoke(null);
                    bh.TryRelease();
                    return;
                }

                AssetRef ar = new AssetRef();
                ar.asset = obj;
                ar.bh = bh;
                ar.Retain();
                lad?.Invoke(ar);

                bh.TryRelease();
            }));
        }));
    }

    public IEnumerator AsyncLoadBundle(string path, LoadBundleDelegate lbd)
    {
        if (bundles.ContainsKey(path))
        {
            BundleHolder bh = bundles[path];
            if(bh.bundle)
            {
                lbd?.Invoke(bh);
            }
            else
            {
                bh.afterLoad += lbd;
            }
        }
        else
        {
            BundleHolder bh = new BundleHolder();
            bh.br = this;
            bh.bundleName = path;
            AddBundle(path, bh);

            bool fail = false;
            int depCount = 0;
            if (depMap.ContainsKey(path))
            {
                List<string> depList = depMap[path];
                depCount = depList.Count;
                foreach (string depName in depList)
                {
                    StartCoroutine(AsyncLoadBundle(depName, delegate (BundleHolder bh1)
                    {
                        if (bh1 == null)
                        {
                            fail = true;
                            return;
                        }

                        if (fail)
                        {
                            bh1.TryRelease();
                        }
                        else
                        {
                            BundleRef br = new BundleRef();
                            br.bh = bh1;
                            bh.AddRef(br);
                        }
                    }));
                }
            }

            string readPath = Path.Combine(Application.dataPath, "../../Bundle", path);
            AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(readPath);

            while (true)
            {
                if (fail)
                {
                    bh.TryRelease();
                    lbd?.Invoke(null);
                    bh.AfterLoad(null);
                    yield break;
                }
                else
                {
                    if (bh.bundleRefs.Count == depCount)
                    {
                        if(abcr.isDone)
                        {
                            AssetBundle ab = abcr.assetBundle;
                            if (ab != null)
                            {
                                bh.bundle = ab;
                                lbd?.Invoke(bh);
                                bh.AfterLoad(bh);
                                yield break;
                            }
                            else
                            {
                                bh.TryRelease();
                                lbd?.Invoke(null);
                                bh.AfterLoad(null);
                                yield break;
                            }
                        }
                        else
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
        }
    }
    
    public IEnumerator AsyncLoadAsset(AssetBundle ab, string name, LoadAssetDelegate lad)
    {
        AssetBundleRequest abr = ab.LoadAssetAsync(name);
        yield return abr;

        Object obj = abr.asset;
        if (obj != null)
        {
            lad?.Invoke(obj);
        }
        else
        {
            lad?.Invoke(null);
        }
    }
}

public delegate void LoadAssetDelegate(Object obj);
public delegate void LoadDelegate(AssetRef ar);
public delegate void LoadBundleDelegate(BundleHolder bh);

public class AssetRef
{
    public BundleHolder bh = null;
    public Object asset = null;
    public void Retain()
    {
        bh.refCount += 1;
    }

    public void Release()
    {
        bh.refCount -= 1;
        TryRelease();
    }

    public void TryRelease()
    {
        bh.TryRelease();
    }
}

public class BundleRef
{
    public BundleHolder bh = null;
    public void Retain()
    {
        bh.refCount += 1;
    }

    public void Release()
    {
        bh.refCount -= 1;
        TryRelease();
    }

    public void TryRelease()
    {
        bh.TryRelease();
    }
}

public class BundleHolder
{
    public AssetBundle bundle = null;
    public string bundleName = null;
    public BundleReader br = null;
    public List<BundleRef> bundleRefs = new List<BundleRef>();
    public int refCount = 0;

    public LoadBundleDelegate afterLoad;

    public void AfterLoad(BundleHolder bh)
    {
        afterLoad?.Invoke(bh);
        afterLoad = null;
    }

    public void AddRef(BundleRef br)
    {
        br.Retain();
        bundleRefs.Add(br);
    }

    public void TryRelease()
    {
        if(refCount <= 0)
        {
            if(bundle!= null)
            {
                bundle.Unload(true);
            }
            br.RemoveBundle(bundleName);

            foreach(BundleRef br in bundleRefs)
            {
                br.Release();
            }
        }
    }
}
