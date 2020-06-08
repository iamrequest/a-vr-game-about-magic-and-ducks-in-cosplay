using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ForceGrabSpell : BaseSpell {
    public SteamVR_Action_Boolean grabObjectAction;
    public SteamVR_Action_Boolean pullPushAction;

    [Header("Particle System")]
    public ParticleSystem particles;
    public Vector3 handOffset; // Offset from the hand while channeling
    public float particleSystemFollowSpeed;

    [Header("Base Force Grab")]
    private ForceGrabbable currentTarget;
    public float objectFollowSpeed;
    private bool isGrabbing;
    public float raycastDistance;
    public LayerMask layermask;

    public float hoverDistance, initialHoverDistance;

    // Line Renderer (likely reuse the dialog line renderer)
    public LineRenderer lineRenderer;
    public Animator lineRendererAnimator;

    [Header("Throw")]
    public int handPositionHistoryCount;
    private int lastHandHistoryIndex;
    private Vector3[] handDeltaHistory;
    private Vector3 previousHandPosition;
    public float throwForceMultiplier;

    // -- Pull/push mode
    private bool isPullPushModeActive;
    private Vector3 originalHandPosition; // Original position when starting pull/push mode
    public float pullPushSpeed;

    protected override void Start() {
        handDeltaHistory = new Vector3[handPositionHistoryCount];
        lastHandHistoryIndex = 0;
        previousHandPosition = Vector3.zero;
    }

    public override void OnSelected(Hand castingHand) {
        base.OnSelected(castingHand);

        hoverDistance = initialHoverDistance;

        particles.transform.position = castingHand.transform.position;
        particles.Play();

        grabObjectAction.AddOnUpdateListener(GrabObject, castingHand.handType);
        pullPushAction.AddOnChangeListener(SetPullPushMode, castingHand.handType); 
    }

    public override void OnDeselected() {
        base.OnDeselected();

        grabObjectAction.RemoveOnUpdateListener(GrabObject, castingHand.handType);
        pullPushAction.RemoveOnChangeListener(SetPullPushMode, castingHand.handType); 
        isGrabbing = false;
        isPullPushModeActive = false;
        particles.Stop();

        if (currentTarget!= null) {
            currentTarget.OnRelease();
        }
    }


    private void GrabObject(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) {
        if (!isSelected) return;

        if (isGrabbing) {
            if (!newState) {
                // -- Throw the object
                isGrabbing = false;
                currentTarget.OnRelease();

                // -- Add force to the projectile
                Vector3 handDelta = Vector3.zero;

                // Iterate over each hand position, and calculate the delta. Add them up, and find the average
                foreach(Vector3 delta in handDeltaHistory) {
                    handDelta += delta;
                }

                currentTarget.rb.AddForce(handDelta / handPositionHistoryCount * throwForceMultiplier, ForceMode.VelocityChange);
            }
        } else {
            if (newState) {
                // -- Attempt to pick up an object
                if (Physics.Raycast(castingHand.transform.position, castingHand.transform.forward * raycastDistance, out RaycastHit hit, layermask)) {
                    // -- Hit: Raycast to the collision point
                    lineRenderer.SetPosition(0, castingHand.transform.position);
                    lineRenderer.SetPosition(1, hit.point);
                    lineRendererAnimator.SetTrigger("fire");

                    // -- This object we collided with can be grabbed
                    // Check both the immediate object, and its parent.
                    //  We can have gameobjects with child colliders
                    ForceGrabbable forceGrabbable = hit.collider.GetComponent<ForceGrabbable>();
                    if (forceGrabbable == null) {
                        forceGrabbable = hit.collider.GetComponentInParent<ForceGrabbable>();
                    }
                    
                    if (forceGrabbable != null) {
                        hoverDistance = initialHoverDistance;

                        isGrabbing = true;
                        currentTarget = forceGrabbable;
                        currentTarget.OnGrab();
                    }
                } else {
                    // -- Missed: Raycast into air
                    lineRenderer.SetPosition(0, castingHand.transform.position);
                    lineRenderer.SetPosition(1, castingHand.transform.position + castingHand.transform.forward * raycastDistance);
                    lineRendererAnimator.SetTrigger("fire");
                }
            }
        }
    }

    private void SetPullPushMode(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) {
        isPullPushModeActive = newState;

        if (newState) {
            originalHandPosition = castingHand.transform.localPosition - Player.instance.transform.position;
        }
    }

    public Vector3 handLocalPosition;
    public Vector3 delta;
    public float deltaMagnitude;
    private void Update() {
        if (!isSelected) return;

        // Lerp the particle system towards the player hand
        LerpTowardsCastingHand(particles.transform, particleSystemFollowSpeed, handOffset);

        // -- Pull/push mode
        // Known bug: If the player turns (snap turn, or in meatspace), the deltas don't calculate as expected
        handLocalPosition = castingHand.transform.localPosition;
        if (isPullPushModeActive) {
            delta = castingHand.transform.localPosition - Player.instance.transform.position - originalHandPosition;
            deltaMagnitude = delta.magnitude;

            Debug.Log(delta);
            Debug.Log(deltaMagnitude);
        }


        // -- In the middle of a grab. Lerp the object towards the target
        if (isGrabbing) {
            // Rigidbody movement
            Vector3 newPosition = Vector3.Lerp(currentTarget.transform.position,
                                               castingHand.transform.position +
                                                castingHand.transform.forward * hoverDistance,
                                               Time.deltaTime * objectFollowSpeed);

            currentTarget.rb.MovePosition(newPosition);

            // Transform movement (object moves through walls)
            //currentTarget.transform.position = Vector3.Lerp(currentTarget.transform.position, 
            //                                                castingHand.transform.position 
            //                                                    + castingHand.transform.forward * hoverDistance,
            //                                                Time.deltaTime * objectFollowSpeed);
        }

        // -- Store a history of hand positions, so that we can neatly approximate the object's velocity post-release
        lastHandHistoryIndex = (lastHandHistoryIndex + 1) % handPositionHistoryCount;
        handDeltaHistory[lastHandHistoryIndex] = castingHand.transform.position - previousHandPosition;
        previousHandPosition = castingHand.transform.position;
    }
}
