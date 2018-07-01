using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateData")]
public class Data : ScriptableObject {
    public int a;
    public List<int> b;
    public string[] d;
    public Data2[] c;
}

public class Data2 : ScriptableObject {
    public int a;
    public int b;
}