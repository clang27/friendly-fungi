using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[Serializable]
[DisplayName("Anchor Clip")]
public class AnchorClip : PlayableAsset, ITimelineClipAsset, IPropertyPreview {
    public ExposedReference<Transform> anchorLocation;
    public ClipCaps clipCaps => ClipCaps.Blending;

    // Creates the playable that represents the instance of this clip.
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        // create a new TweenBehaviour
        var playable = ScriptPlayable<AnchorBehaviour>.Create(graph);
        var anchor = playable.GetBehaviour();

        // set the behaviour's data
        anchor.anchorLocation = anchorLocation.Resolve(graph.GetResolver());;

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
