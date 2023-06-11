using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[Serializable]
[DisplayName("Tween Clip")]
public class TweenClip : PlayableAsset, ITimelineClipAsset, IPropertyPreview {
    public ExposedReference<Transform> startLocation;
    public ExposedReference<Transform> endLocation;

    [Tooltip("Changes the position of the assigned object")]
    public bool shouldTweenPosition = true;

    [Tooltip("Changes the rotation of the assigned object")]
    public bool shouldTweenRotation = true;
    
    [Tooltip("Should use gravity")]
    public bool useGravity = false;

    [Tooltip("Only keys in the [0,1] range will be used")]
    public AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

    // Implementation of ITimelineClipAsset. This specifies the capabilities of this timeline clip inside the editor.
    public ClipCaps clipCaps => ClipCaps.Blending;

    // Creates the playable that represents the instance of this clip.
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        // create a new TweenBehaviour
        var playable = ScriptPlayable<TweenBehaviour>.Create(graph);
        var tween = playable.GetBehaviour();

        // set the behaviour's data
        tween.startLocation = startLocation.Resolve(graph.GetResolver());
        tween.endLocation = endLocation.Resolve(graph.GetResolver());
        tween.curve = curve;
        tween.shouldTweenPosition = shouldTweenPosition;
        tween.shouldTweenRotation = shouldTweenRotation;
        tween.useGravity = useGravity;

        return playable;
    }

    // Defines which properties are changed by this playable. Those properties will be reverted in editmode
    // when Timeline's preview is turned off.
    public void GatherProperties(PlayableDirector director, IPropertyCollector driver) {
        const string kLocalPosition = "m_LocalPosition";
        const string kLocalRotation = "m_LocalRotation";

        driver.AddFromName<Transform>(kLocalPosition + ".x");
        driver.AddFromName<Transform>(kLocalPosition + ".y");
        driver.AddFromName<Transform>(kLocalPosition + ".z");
        driver.AddFromName<Transform>(kLocalRotation + ".x");
        driver.AddFromName<Transform>(kLocalRotation + ".y");
        driver.AddFromName<Transform>(kLocalRotation + ".z");
        driver.AddFromName<Transform>(kLocalRotation + ".w");
    }
}
