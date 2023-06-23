using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[TrackColor(1.0f, 0.0f, 0.0f)]
[TrackBindingType(typeof(Transform))]
[TrackClipType(typeof(TweenClip))]
public class TweenTrack : TrackAsset {
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
        return ScriptPlayable<TweenMixerBehaviour>.Create(graph, inputCount);
    }
}