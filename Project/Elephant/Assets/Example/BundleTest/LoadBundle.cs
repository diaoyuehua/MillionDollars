using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadBundle : MonoBehaviour {

    private string bundleName = "";
    private string assetName = "";

    private AssetRef ar = null;
    private GameObject go = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        bundleName = GUI.TextField(new Rect(0, 0, 256, 32), bundleName);
        assetName = GUI.TextField(new Rect(256, 0, 256, 32), assetName);
        if (GUI.Button(new Rect(512, 0, 64, 32), "Load"))
        {
            BundleReader.GetInstance().AsyncLoad(bundleName, assetName, delegate(AssetRef ar)
            {
                if(ar!=null)
                {
                    go = GameObject.Instantiate(ar.asset) as GameObject ;
                    this.ar = ar;
                }
                else
                {
                    Debug.LogWarning("Load fail");
                }
            });
        }

        if (GUI.Button(new Rect(576, 0, 64, 32), "Delete"))
        {
            if(go != null)
            {
                GameObject.Destroy(go);
                ar.Release();
                go = null;
                ar = null;
            }
        }

        if (GUI.Button(new Rect(0, 64, 64, 32), "Print"))
        {
            BundleReader.GetInstance().PrintBundles();
        }
    }
}
