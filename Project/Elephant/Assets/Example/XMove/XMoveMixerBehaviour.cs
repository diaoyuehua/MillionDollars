using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class XMoveMixerBehaviour : PlayableBehaviour
{
    Vector3 m_DefaultPosition;

    Vector3 m_AssignedPosition;

    Transform m_TrackBinding;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as Transform;

        if (m_TrackBinding == null)
            return;

        if (m_TrackBinding.position != m_AssignedPosition)
            m_DefaultPosition = m_TrackBinding.position;

        int inputCount = playable.GetInputCount ();

        Vector3 blendedPosition = Vector3.zero;
        float totalWeight = 0f;
        float greatestWeight = 0f;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<XMoveBehaviour> inputPlayable = (ScriptPlayable<XMoveBehaviour>)playable.GetInput(i);
            XMoveBehaviour input = inputPlayable.GetBehaviour ();
            
            blendedPosition += input.position * inputWeight;
            totalWeight += inputWeight;

            if (inputWeight > greatestWeight)
            {
                greatestWeight = inputWeight;
            }
        }

        m_AssignedPosition = blendedPosition + m_DefaultPosition * (1f - totalWeight);
        m_TrackBinding.position = m_AssignedPosition;
    }
}
