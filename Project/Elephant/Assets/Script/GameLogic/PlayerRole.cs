using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRole : Role {

    private PlayerState curState = null;

    public override void Command(RoleCommand _cmd, params object[] _args)
    {
        if(curState == null) return;

        PlayerStateEnum nextState = curState.Command(_cmd, _args);
        if(nextState != curState.state)
        {
            curState.Exit();
            curState = GetState(nextState, _args);
            curState.Enter();
        }
    }

    private PlayerState GetState(PlayerStateEnum _pse, params object[] _args)
    {
        PlayerState _ret = null;
        switch (_pse)
        {
            case PlayerStateEnum.Stand:
                _ret = new PlayerStandState(gameObject);
                _ret.Reset();
                break;
            case PlayerStateEnum.Walk:
                Vector2 _walkDir = (Vector2)_args[0];
                PlayerWalkState _pws = new PlayerWalkState(gameObject);
                _pws.Dir = _walkDir;
                _ret = _pws;
                break;
            case PlayerStateEnum.Run:
                Vector2 _runDir = (Vector2)_args[0];
                PlayerRunState _prs = new PlayerRunState(gameObject);
                _prs.Dir = _runDir;
                _ret = _prs;
                break;
            default:
                break;
        }
        if(_ret == null)
        {
            MyLogger.Error("RoleControl", "{0} state don't have a creator!", _pse);
            return _ret;
        }
        else
        {
            return _ret;
        }
    }

    void Start()
    {
        curState = GetState(PlayerStateEnum.Stand);
        curState.Enter();
    }

    void Update()
    {
        if (curState != null)
        {
            PlayerStateEnum nextState = curState.Update();
            if (nextState != curState.state)
            {
                curState.Exit();
                curState = GetState(nextState);
                curState.Enter();
            }
        }
    }

}
