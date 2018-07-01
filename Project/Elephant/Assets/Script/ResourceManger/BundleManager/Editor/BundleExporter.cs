using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;

public class BundleExporter
{
    public static void BuildProjectBundle(string outPath, bool rebuid)
    {
        Debug.LogWarningFormat("Build bundle to {0}.", outPath);
        int millisecondStart = System.DateTime.Now.Millisecond;

        BuildAssetBundleOptions options = 0;

        if(rebuid)
        {
            options = options | BuildAssetBundleOptions.ForceRebuildAssetBundle;
        }

        AssetBundleManifest abm = BuildPipeline.BuildAssetBundles(outPath, options, BuildTarget.StandaloneWindows64);

        if (!CircleCheck(abm))
        {
            int millisecondFail = System.DateTime.Now.Millisecond;
            Debug.LogWarningFormat("Build bundle to {0} failed, cost {1} seconds.", outPath, (millisecondFail - millisecondStart) / 1000);
            return;
        }
        
        ExportManifestToXml(abm, outPath);

        int millisecondEnd = System.DateTime.Now.Millisecond;
        Debug.LogWarningFormat("Build bundle to {0} end, cost {1} seconds.", outPath, (millisecondEnd - millisecondStart) / 1000);
    }

    public static void BuildSpecifiedBundle(string outPath, Object specified)
    {
        if(specified == null)
        {
            Debug.LogError("Can build a null object");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(specified);

        string assetBundleName = AssetDatabase.GetImplicitAssetBundleName(assetPath);

        if (string.IsNullOrEmpty(assetBundleName))
        {
            Debug.LogError("This object is not include in any bundle");
            return;
        }

        AssetBundleBuild abb = new AssetBundleBuild();
        abb.assetBundleName = assetBundleName;

        string[] paths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);

        abb.assetNames = paths;

       BuildPipeline.BuildAssetBundles(outPath, new AssetBundleBuild[1] { abb }, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows64);

        AssetBundleManifest abm2 = BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.DryRunBuild, BuildTarget.StandaloneWindows64);

        if (!CircleCheck(abm2))
        {
            Debug.LogWarningFormat("Build bundle {0} to {1} Failed", assetBundleName, outPath);
            return;
        }

        ExportManifestToXml(abm2, outPath);

        Debug.LogWarningFormat("Build bundle {0} to {1} Success", assetBundleName, outPath);
    }

    public static bool CircleCheck(AssetBundleManifest manifest)
    {
        Dictionary<string, bool> resultMap = new Dictionary<string, bool>();
        List<string> currentLine = new List<string>();
        string[] assetNames = manifest.GetAllAssetBundles();
        foreach(string name in assetNames)
        {
            if(!CircleCheckDFS(name, currentLine, resultMap, manifest))
            {
                return false;
            }
        }
        return true;
    }

    public static bool CircleCheckDFS(string assetName, List<string> currentLine, Dictionary<string, bool> resultMap, AssetBundleManifest manifest)
    {
        if(resultMap.ContainsKey(assetName) && resultMap[assetName])
        {
            return true;
        }

        int index = currentLine.IndexOf(assetName);
        if (index >= 0)
        {
            string circle = "";
            
            for(int i = index; i < currentLine.Count; ++i)
            {
                circle += currentLine[i];
                circle += "=>";
            }

            circle += assetName;

            Debug.LogError("Circle dependence detected:\n" + circle);
            return false;
        }

        currentLine.Add(assetName);

        string[] assetNames = manifest.GetDirectDependencies(assetName);
        foreach(string name in assetNames)
        {
            if (!CircleCheckDFS(name, currentLine, resultMap, manifest))
            {
                return false;
            }
        }

        currentLine.RemoveAt(currentLine.Count - 1);

        resultMap.Add(assetName, true);

        return true;
    }

    public static void ExportManifestToXml(AssetBundleManifest manifest, string outPath)
    {
        //xml
        string[] bundles = manifest.GetAllAssetBundles();

        XmlDocument xmlDoc = new XmlDocument();

        XmlElement rootNode = xmlDoc.CreateElement("root");

        foreach(string bundle in bundles)
        {
            XmlElement bundleNode = xmlDoc.CreateElement("bundle");

            bundleNode.SetAttribute("name", bundle);

            string[] deps = manifest.GetDirectDependencies(bundle);

            foreach (string dep in deps)
            {
                XmlElement depNode = xmlDoc.CreateElement("dep");
                depNode.SetAttribute("name", dep);

                bundleNode.AppendChild(depNode);
            }

            rootNode.AppendChild(bundleNode);
        }

        xmlDoc.AppendChild(rootNode);

        xmlDoc.Save(Path.Combine(outPath, "bundle.xml"));

    }

    [MenuItem("Assets/Build This Bundle")]
    public static void BuildThisBundle()
    {
        string outPath = EditorUtility.OpenFolderPanel("Select output bundle dir", "", "");

        BuildSpecifiedBundle(outPath, Selection.activeObject);
    }

    [MenuItem("Assets/Build All Bundle")]
    static void BuildAllBundle()
    {
        string outPath = EditorUtility.OpenFolderPanel("Select output bundle dir", "", "");

        BuildProjectBundle(outPath, false);

    }
}
