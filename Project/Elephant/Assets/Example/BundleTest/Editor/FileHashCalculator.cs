using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class FileHashCalculator
{
    [MenuItem("Assets/Calculate File Hash")]
    public static void CalculaterHash()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        string absulotePath = Application.dataPath + assetPath.Substring(6);

        FileStream fs = new FileStream(absulotePath, FileMode.Open, FileAccess.Read);

        //md5
        System.Security.Cryptography.MD5 md5calculator = System.Security.Cryptography.MD5.Create();
        byte[] buffer1 = md5calculator.ComputeHash(fs);
        StringBuilder sb1 = new StringBuilder();
        for (int i = 0; i < buffer1.Length; ++i)
        {
            sb1.Append(buffer1[i].ToString("x2"));
        }

        Debug.LogWarningFormat("MD5:{0}", sb1.ToString());

        System.Security.Cryptography.SHA1 sha1calculator = System.Security.Cryptography.SHA1.Create();
        byte[] buffer2 = sha1calculator.ComputeHash(fs);
        StringBuilder sb2 = new StringBuilder();
        for (int i = 0; i < buffer2.Length; ++i)
        {
            sb2.Append(buffer2[i].ToString("x2"));
        }

        Debug.LogWarningFormat("SHA1:{0}", sb2.ToString());

        fs.Close();
    }
}
