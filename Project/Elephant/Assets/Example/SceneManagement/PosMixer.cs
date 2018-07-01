using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PosMixer : PlayableBehaviour
{
    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        Debug.Log("MoveMixer.OnGraphStart");
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        Debug.Log("MoveMixer.OnGraphStop");
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log("MoveMixer.OnBehaviourPlay");
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        Debug.Log("MoveMixer.OnBehaviourPause");
    }

    // Called each frame while the state is set to Play
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Debug.Log("MoveMixer.PrepareFrame");

        int inputCount = playable.GetInputCount();

        if (inputCount > 0)
        {
            GameObject role = playerData as GameObject;

            float inputWeight = playable.GetInputWeight(0);
            ScriptPlayable<PosBehaviour> inputPlayable = (ScriptPlayable<PosBehaviour>)playable.GetInput(0);
            PosBehaviour input = inputPlayable.GetBehaviour();

            Vector3 blendedPosition = input.result;

            role.transform.position = blendedPosition;
        }
    }
}
