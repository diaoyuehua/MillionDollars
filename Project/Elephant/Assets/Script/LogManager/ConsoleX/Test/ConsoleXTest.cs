using UnityEngine;
using System.Collections;

public class ConsoleXTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("log"))
            Debug.Log("log");
        if (GUILayout.RepeatButton("repeat log"))
            Debug.Log("repeat log");
        GUILayout.EndVertical();

        GUILayout.Space(32);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("warning"))
            Debug.LogWarning("warning");
        if (GUILayout.RepeatButton("repeat warning"))
            Debug.LogWarning("repeat log");
        GUILayout.EndVertical();

        GUILayout.Space(32);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("error"))
            Debug.LogError("error");
        if (GUILayout.RepeatButton("repeat error"))
            Debug.LogError("repeat log");
        GUILayout.EndVertical();

        GUILayout.Space(32);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("long"))
            Debug.Log("Loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong");
        if (GUILayout.RepeatButton("repeat long"))
            Debug.Log("Loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong");
        GUILayout.EndVertical();

        GUILayout.EndVertical();
    }
}
