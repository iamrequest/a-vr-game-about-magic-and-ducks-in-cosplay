using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using TMPro;
using UnityEngine.UI;

public enum FacingDirection {
    Front, Back
}
public class CanvasCharacter : MonoBehaviour {
    private const bool DEBUG = true;

    [Header("Rotating the Canvas")]
    public Transform canvasTransform;
    public SpriteRenderer imageUI;

    // Typically, we only want to rotate along the Y axis.
    public bool constrainToYAxis;
    public FacingDirection facingDirection;

    [Header("Animation")]
    public CanvasCharacterState initialAnimationState;
    public CanvasCharacterState activeAnimationState;

    [Tooltip("This gameobject stores all of the animation states. Used for init")]
    public GameObject animationStatesGameObject;
    private List<CanvasCharacterState> animationStates;

    private void Start() {
        // -- Populate our available animation states
        animationStates = new List<CanvasCharacterState>();

        CanvasCharacterState[] states = animationStatesGameObject.GetComponents<CanvasCharacterState>();
        foreach (CanvasCharacterState state in states) {
            animationStates.Add(state);
        }

        SetAnimationState(initialAnimationState);
    }

    void Update() {
        if (constrainToYAxis) {
            // Figure out the direction from us to the player, and constrain that to the Y axis.
            canvasTransform.forward =
                Vector3.ProjectOnPlane(transform.position - Player.instance.hmdTransform.position,
                                       Vector3.up).normalized;
        } else {
            canvasTransform.LookAt(Player.instance.hmdTransform.position);
        }

        UpdateCanvasDirection();
        activeAnimationState.StateUpdate();

        if (DEBUG) {
            Debug.DrawRay(transform.position, transform.forward);
        }
    }

    void UpdateCanvasDirection() {
        float angularDifference = Quaternion.Angle(transform.rotation, canvasTransform.rotation);

        // The calculation seems backwards for some reason, but I want to maintain transform.forward meaning the character's forward direction
        if (angularDifference < 90) {
            facingDirection = FacingDirection.Back;
        } else {
            facingDirection = FacingDirection.Front;
        }
    }

    public void SetAnimationState(CanvasCharacterState newState) {
        if (activeAnimationState == newState) return;

        // Phase out of the previous state
        if (activeAnimationState != null) {
            activeAnimationState.OnStateExit();
        }

        // Enter the new state
        newState.parentCharacter = this;
        newState.OnStateEnter();
        activeAnimationState = newState;
    }

    public void SetAnimationState(AnimationState animationState) {
        // Find the state that has the given state name
        CanvasCharacterState state;
        try {
            state = animationStates.Find(s => s.animationState == animationState);
            SetAnimationState(state);
        } catch (ArgumentNullException) {
            Debug.LogError("Unable to find state for this character with a state of " + animationState.ToString());
        }
    }

    public void LookAt(Vector3 position) {
        transform.LookAt(position);
    }
}
