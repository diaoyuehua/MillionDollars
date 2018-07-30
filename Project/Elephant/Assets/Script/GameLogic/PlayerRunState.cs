using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerState {

    private GameObject roleObject;
    private Animator roleAnimator;
    private float speed = 0;
    private Vector2 dir = Vector2.zero;

    public PlayerRunState(GameObject _role) : base(PlayerStateEnum.Run)
    {
        roleObject = _role;
        roleAnimator = roleObject.GetComponent<Animator>();
        speed = UserData.GetInstance().runSpeed;
    }

    public Vector2 Dir
    {
        set
        {
            dir = value.normalized;
        }
        get
        {
            return dir;
        }
    }

    public override PlayerStateEnum Command(RoleCommand _cmd, params object[] args)
    {
        if (_cmd == RoleCommand.Run)
        {
            Vector2 _dir = (Vector2)args[0];
            dir = _dir.normalized;
            roleObject.transform.localRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y));
            return PlayerStateEnum.None;
        }
        else if(_cmd == RoleCommand.Walk)
        {
            return PlayerStateEnum.Walk;
        }
        if (_cmd == RoleCommand.Stand)
        {
            return PlayerStateEnum.Stand;
        }
        else
        {
            return state;
        }
    }

    public override void Enter()
    {
        roleAnimator.SetBool("Move", true);
        roleAnimator.SetBool("Fast", true);
        roleObject.transform.localRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y));
    }

    public override void Exit()
    {
        roleAnimator.SetBool("Move", false);
        roleAnimator.SetBool("Fast", false);
    }

    public override void Reset()
    {
        dir = Vector2.zero;
    }

    public override PlayerStateEnum Update()
    {
        Vector3 move = new Vector3(dir.x, 0, dir.y) * speed;
        roleObject.transform.localPosition += move;
        return state;
    }
}
