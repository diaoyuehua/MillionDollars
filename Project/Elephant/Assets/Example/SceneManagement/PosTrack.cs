using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[TrackClipType(typeof(PosClip))]
[TrackBindingType(typeof(GameObject))]
public class PosTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<PosMixer>.Create(graph, inputCount);
    }
}