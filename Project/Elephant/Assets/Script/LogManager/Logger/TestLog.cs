using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLog : MonoBehaviour {

    private void Awake()
    {
        MyLogger.Init();
        MyLogger.Debug("Test", "Awake");
    }

    void Start () {
        MyLogger.Debug("Test", "Start");
	}

    void OnEnable()
    {
        MyLogger.Debug("Test", "OnEnable");
    }

    void OnDisable()
    {
        MyLogger.Debug("Test", "OnDisable");
        MyLogger mylog = null;
        //mylog.ToString();
    }

}
