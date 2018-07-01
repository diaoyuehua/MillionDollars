using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
[System.Serializable]
public class PosBehaviour : PlayableBehaviour
{
    public Vector3 pos;

    public Vector3 result;

    public Vector3 startPos;

	// Called when the owning graph starts playing
	public override void OnGraphStart(Playable playable) {
        Debug.Log("PosBehaviour.OnGraphStart");
        startPos = GameObject.Find("RPG-Character").transform.position;
        result = startPos;
        Debug.LogWarning("OnGraphStart:" + result);
    }

	// Called when the owning graph stops playing
	public override void OnGraphStop(Playable playable) {
        Debug.Log("PosBehaviour.OnGraphStop");
    }

	// Called when the state of the playable is set to Play
	public override void OnBehaviourPlay(Playable playable, FrameData info) {
        Debug.Log("PosBehaviour.OnBehaviourPlay");
    }

	// Called when the state of the playable is set to Paused
	public override void OnBehaviourPause(Playable playable, FrameData info) {
        Debug.Log("PosBehaviour.OnBehaviourPause");
    }

    // Called each frame while the state is set to Play
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Debug.Log("ProcessFrame.ProcessFrame");

        result = Vector3.Lerp(startPos, pos, (float)playable.GetTime() / (float)playable.GetDuration());
    }
}
