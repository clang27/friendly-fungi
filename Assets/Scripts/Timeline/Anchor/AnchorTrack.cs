using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[TrackColor(0.0f, 0.0f, 1.0f)]
[TrackBindingType(typeof(Transform))]
[TrackClipType(typeof(AnchorClip))]
public class AnchorTrack : TrackAsset {
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
        return ScriptPlayable<AnchorMixerBehaviour>.Create(graph, inputCount);
    }
}