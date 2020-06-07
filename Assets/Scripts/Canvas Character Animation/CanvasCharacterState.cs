using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationState {
    Idle, Anger, Shock, Blush
}
public class CanvasCharacterState : MonoBehaviour {
    public CanvasCharacter parentCharacter; 

    [Tooltip("What does this state depict the character doing? Example: Idle, Sitting, etc")]
    public AnimationState animationState;
    public float animationSpeed;
    private float lastFrameChange;
    private int currentFrame;

    public List<Sprite> animationFront;
    public List<Sprite> animationBack;

    public void OnStateEnter() {
        lastFrameChange = Time.time;
        currentFrame = 0;
    }
    public void OnStateExit() {
    }

    // Called once per Update() frame, when the state is active.
    public void StateUpdate() {
        // Test if we need to advance to the next frame
        if (Time.time > lastFrameChange + animationSpeed) {
            // Assumption: The front animation has the same number of frames as the back animation
            currentFrame = (currentFrame + 1) % animationFront.Count;
            lastFrameChange = Time.time;
        }

        switch (parentCharacter.facingDirection) {
            case FacingDirection.Front:
                parentCharacter.imageUI.sprite = animationFront[currentFrame];
                break;
            default:
                parentCharacter.imageUI.sprite = animationBack[currentFrame];
                break;

        }
    }
}
