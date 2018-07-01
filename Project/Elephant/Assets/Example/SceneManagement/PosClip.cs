using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class PosClip : PlayableAsset, ITimelineClipAsset
{
    public Vector3 pos;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        PosBehaviour template = new PosBehaviour();
        template.pos = pos;
        var playable = ScriptPlayable<PosBehaviour>.Create(graph, template);
        return playable;
    }
}