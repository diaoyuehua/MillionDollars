using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RoleCommand
{
    Stand,
    Walk,
    Run,
    Dodge,
}

public abstract class Role : MonoBehaviour {

    public abstract void Command(RoleCommand cmd, params object[] args);
}
