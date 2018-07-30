using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerState
{
    private GameObject roleObject;
    private Animator roleAnimator;
    private float speed = 0;
    private Vector2 dir = Vector2.zero;

    public PlayerWalkState(GameObject _role) : base(PlayerStateEnum.Walk)
    {
        roleObject = _role;
        roleAnimator = roleObject.GetComponent<Animator>();
        speed = UserData.GetInstance().walkSpeed;
    }

    public Vector2 Dir
    {
        set
        {
            dir = value;
        }
        get
        {
            return dir;
        }
    }

    public override PlayerStateEnum Command(RoleCommand _cmd, params object[] args)
    {
        if(_cmd == RoleCommand.Walk)
        {
            Vector2 _dir = (Vector2)args[0];
            dir = _dir.normalized;
            roleObject.transform.localRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y));
            return PlayerStateEnum.None;
        }
        else if(_cmd == RoleCommand.Run)
        {
            return PlayerStateEnum.Run;
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
        roleObject.transform.localRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y));
    }

    public override void Exit()
    {
        roleAnimator.SetBool("Move", false);
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
