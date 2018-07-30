using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStandState : PlayerState {

    private GameObject roleObject;
    private Animator roleAnimator;

    public PlayerStandState(GameObject _role) : base(PlayerStateEnum.Stand)
    {
        roleObject = _role;
        roleAnimator = roleObject.GetComponent<Animator>();
    }

    public override PlayerStateEnum Command(RoleCommand cmd, params object[] args)
    {
        if(cmd == RoleCommand.Walk)
        {
            return PlayerStateEnum.Walk;
        }
        if (cmd == RoleCommand.Run)
        {
            return PlayerStateEnum.Run;
        }
        else
        {
            return state;
        }
    }

    public override PlayerStateEnum Update()
    {
        return state;
    }

    public override void Reset()
    {
        // do nothing
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {

    }
}
