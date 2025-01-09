using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleClip : PlayableAsset
{
    public string subtitleText;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SubtitlesBehaviour>.Create(graph);
        SubtitlesBehaviour subtitlesBehaviour = playable.GetBehaviour();
        subtitlesBehaviour.subtitleText = subtitleText;

        return playable;
    }

}
