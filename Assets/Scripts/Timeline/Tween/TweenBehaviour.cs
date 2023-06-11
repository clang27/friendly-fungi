using UnityEngine;
using UnityEngine.Playables;

public class TweenBehaviour : PlayableBehaviour {
    public Transform startLocation, endLocation;
    public bool shouldTweenPosition, shouldTweenRotation;
    public bool useGravity;

    public AnimationCurve curve;
}
