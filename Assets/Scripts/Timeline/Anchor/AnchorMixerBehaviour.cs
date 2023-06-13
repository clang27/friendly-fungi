using UnityEngine;
using UnityEngine.Playables;

public class AnchorMixerBehaviour : PlayableBehaviour {

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        var trackBindingTransform = playerData as Transform;
        var newPosition = trackBindingTransform.position;
        var newRotation = trackBindingTransform.rotation;
        
        // Iterate on all mixer's inputs (ie each clip on the track)
        var inputCount = playable.GetInputCount();
        for (var i = 0; i < inputCount; i++) {
            var inputWeight = playable.GetInputWeight(i);
            if (inputWeight <= 0)
                continue;
            
            var input = playable.GetInput(i);
            var anchorInput = GetAnchorBehaviour(input);
            newPosition = anchorInput.anchorLocation.position;
            newRotation = anchorInput.anchorLocation.rotation;
        }
        
        trackBindingTransform.position = newPosition;
        trackBindingTransform.rotation = newRotation;
    }
    
    private static AnchorBehaviour GetAnchorBehaviour(Playable playable) {
        var anchorInput = (ScriptPlayable<AnchorBehaviour>)playable;
        return anchorInput.GetBehaviour();
    }
}