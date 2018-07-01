using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
/*
public class BundleNameAssigner : AssetPostprocessor
{
    private static List<IBundleNameAssigner> assigners = new List<IBundleNameAssigner>
    {
        new PathAssigner("Model/"),
        new TypeAssigner(typeof(Material)),
        new PathAndTypeAssigner("Charactor/", typeof(GameObject)),
    };

    public static string GetABundleName(string assetPath)
    {
        for(int i = 0; i < assigners.Count; ++i)
        {
            IBundleNameAssigner assigner = assigners[i];
            bool ret = assigner.IsInterestIn(assetPath);
            if (ret)
            {
                return assigner.GetBundleName(assetPath);
            }
        }
        return "";
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string path in importedAssets)
        {
            if(!Exclude(path))
            {
                string bundleName = GetABundleName(path);
                AssetImporter ai = AssetImporter.GetAtPath(path);
                ai.SetAssetBundleNameAndVariant(bundleName, "");
            }
        }

        foreach (string path in movedAssets)
        {
            if (!Exclude(path))
            {
                string bundleName = GetABundleName(path);
                AssetImporter ai = AssetImporter.GetAtPath(path);
                ai.SetAssetBundleNameAndVariant(bundleName, "");
            }
        }
    }

    public void OnPostprocessAssetbundleNameChanged(string assetPath, string previousAssetBundleName, string newAssetBundleName)
    {
        if (!Exclude(assetPath))
        {
            string bundleName = GetABundleName(assetPath);
            AssetImporter ai = AssetImporter.GetAtPath(assetPath);
            ai.SetAssetBundleNameAndVariant(bundleName, "");
        }
    }

    public static bool Exclude(string path)
    {
        System.Type t = AssetDatabase.GetMainAssetTypeAtPath(path);
        if(typeof(MonoScript) == t)
        {
            return true;
        }
        return false;
    }

    private interface IBundleNameAssigner
    {
        bool IsInterestIn(string assetPath);
        string GetBundleName(string assetPath);
    }

    //example path, type, path and type
    private class PathAssigner : IBundleNameAssigner
    {
        private Regex reg = null;

        public PathAssigner(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                reg = new Regex(path);
            }
        }

        public virtual string GetBundleName(string assetPath)
        {
            return Path.GetFileNameWithoutExtension(assetPath);
        }

        public bool IsInterestIn(string assetPath)
        {
            if(reg != null && reg.IsMatch(assetPath))
            {
                return true;
            }
            else{
                return false;
            }
        }
    }

    private class TypeAssigner : IBundleNameAssigner
    {
        private System.Type type = null;

        public TypeAssigner(System.Type t)
        {
            type = t;
        }

        public virtual string GetBundleName(string assetPath)
        {
            return type.Name + "." + Path.GetFileNameWithoutExtension(assetPath);
        }

        public bool IsInterestIn(string assetPath)
        {
            System.Type t = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (type != null && type == t)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private class PathAndTypeAssigner : IBundleNameAssigner
    {
        private System.Type type = null;
        private Regex reg = null;

        public PathAndTypeAssigner(string path, System.Type t)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                reg = new Regex(path);
            }
            type = t;
        }

        public virtual string GetBundleName(string assetPath)
        {
            return type.Name + "." + Path.GetFileNameWithoutExtension(assetPath);
        }

        public bool IsInterestIn(string assetPath)
        {
            System.Type t = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (reg != null && reg.IsMatch(assetPath) && type != null && type == t)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
*/