using UnityEngine;
using UnityEngine.Playables;

public class TweenMixerBehaviour : PlayableBehaviour {
    private static readonly AnimationCurve s_DefaultCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    private Vector3 m_InitialPosition;
    private Quaternion m_InitialRotation;

    private bool m_ShouldInitializeTransform = true;

    // Performs blend of position and rotation of all clips connected to a track mixer
    // The result is applied to the track binding's (playerData) transform.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        var timeManipulation = playerData as TimeManipulation;
        var trackBindingTransform = timeManipulation.transform;

        if (timeManipulation == null)
            return;

        // Get the initial position and rotation of the track binding, only when ProcessFrame is first called
        InitializeIfNecessary(trackBindingTransform);

        var accumPosition = Vector3.zero;
        var accumRotation = new Quaternion(0f, 0f, 0f, 0f);

        var totalPositionWeight = 0.0f;
        var totalRotationWeight = 0.0f;
        var useGravity = false;
        var walkingOnGrass = false;
        var walkingOnWood = false;
        
        // Iterate on all mixer's inputs (ie each clip on the track)
        var inputCount = playable.GetInputCount();
        for (var i = 0; i < inputCount; i++) {
            var inputWeight = playable.GetInputWeight(i);
            if (inputWeight <= 0)
                continue;

            var input = playable.GetInput(i);
            var normalizedInputTime = (float)(input.GetTime() / input.GetDuration());

            // get the clip's behaviour and evaluate the progression along the curve
            var tweenInput = GetTweenBehaviour(input);
            var tweenProgress = GetCurve(tweenInput).Evaluate(normalizedInputTime);
            useGravity |= tweenInput.useGravity;
            walkingOnGrass |= tweenInput.walkingOnGrass;
            walkingOnWood |= tweenInput.walkingOnWood;

            // calculate the position's progression along the curve according to the input's (clip) weight
            if (tweenInput.shouldTweenPosition) {
                totalPositionWeight += inputWeight;
                accumPosition += TweenPosition(tweenInput, tweenProgress, inputWeight);
            }

            // calculate the rotation's progression along the curve according to the input's (clip) weight
            if (tweenInput.shouldTweenRotation) {
                totalRotationWeight += inputWeight;
                accumRotation = TweenRotation(tweenInput, accumRotation, tweenProgress, inputWeight);
            }
        }

        timeManipulation.UseGravity = useGravity;
        timeManipulation.WalkingOnGrass = walkingOnGrass;
        timeManipulation.WalkingOnWood = walkingOnWood;
        
        // Apply the final position and rotation values in the track binding
        var newPosition = accumPosition + m_InitialPosition * (1.0f - totalPositionWeight);
        timeManipulation.RaycastPosition = newPosition;
        
        trackBindingTransform.position = new Vector3(newPosition.x, (!useGravity) ? newPosition.y : trackBindingTransform.position.y, newPosition.z);
        //trackBinding.rotation = accumRotation.Blend(m_InitialRotation, 1.0f - totalRotationWeight);
        trackBindingTransform.rotation = Quaternion.Slerp(accumRotation, m_InitialRotation, (1.0f - totalRotationWeight));
        trackBindingTransform.rotation.Normalize();
    }

    private void InitializeIfNecessary(Transform transform) {
        if (!m_ShouldInitializeTransform) return;

        m_InitialPosition = transform.position;
        m_InitialRotation = transform.rotation;
        m_ShouldInitializeTransform = false;
    }

    private Vector3 TweenPosition(TweenBehaviour tweenInput, float progress, float weight) {
        var startPosition = m_InitialPosition;
        if (tweenInput.startLocation != null)
            startPosition = tweenInput.startLocation.position;

        var endPosition = m_InitialPosition;
        if (tweenInput.endLocation != null)
            endPosition = tweenInput.endLocation.position;

        return Vector3.Lerp(startPosition, endPosition, progress) * weight;
    }

    private Quaternion TweenRotation(TweenBehaviour tweenInput, Quaternion accumRotation, float progress,
        float weight) {
        var startRotation = m_InitialRotation;
        if (tweenInput.startLocation != null) startRotation = tweenInput.startLocation.rotation;

        var endRotation = m_InitialRotation;
        if (tweenInput.endLocation != null) endRotation = tweenInput.endLocation.rotation;

        var desiredRotation = Quaternion.Lerp(startRotation, endRotation, progress);
        return Quaternion.Slerp(accumRotation, desiredRotation.normalized, weight);
        //return accumRotation.Blend(desiredRotation.Normalize(), weight);
    }

    private static TweenBehaviour GetTweenBehaviour(Playable playable) {
        var tweenInput = (ScriptPlayable<TweenBehaviour>)playable;
        return tweenInput.GetBehaviour();
    }

    private static AnimationCurve GetCurve(TweenBehaviour tween) {
        return tween?.curve ?? s_DefaultCurve;
    }
}