using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour {

    public static UserData instance = null;
    public static UserData GetInstance()
    {
        return instance;
    }

    public void Awake()
    {
        instance = this;
    }

    public float walkSpeed = 0.1f;
    public float runSpeed = 0.2f;
    public float dodgeSpeed = 0.4f;
}
