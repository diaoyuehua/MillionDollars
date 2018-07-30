using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStateEnum
{
    None,

    Stand,
    Walk,
    Run,
}

public abstract class PlayerState {

    public readonly PlayerStateEnum state;

    public PlayerState(PlayerStateEnum pse)
    {
        state = pse;
    }

    public abstract PlayerStateEnum Update();

    public abstract void Reset();

    public abstract void Enter();

    public abstract void Exit();

    public abstract PlayerStateEnum Command(RoleCommand cmd, params object[] args);
}
