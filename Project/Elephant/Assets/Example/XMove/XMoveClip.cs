using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class XMoveClip : PlayableAsset, ITimelineClipAsset
{
    public XMoveBehaviour template = new XMoveBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<XMoveBehaviour>.Create (graph, template);
        return playable;
    }
}
