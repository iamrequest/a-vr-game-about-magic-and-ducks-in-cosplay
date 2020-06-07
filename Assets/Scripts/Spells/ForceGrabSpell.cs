using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ForceGrabSpell : BaseSpell {
    public SteamVR_Action_Boolean grabObjectAction;
    public SteamVR_Action_Boolean pullPushAction;
    public Vector3 handOffset; // Offset from the hand while channeling

    [Header("Base Force Grab")]
    private ForceGrabbable currentTarget;
    public float objectFollowSpeed;
    private bool isGrabbing;
    public float raycastDistance;
    public LayerMask layermask;

    public float hoverDistance, initialHoverDistance;
    private bool isPullPushModeActive;

    // Line Renderer (likely reuse the dialog line renderer)
    public LineRenderer lineRenderer;
    public Animator lineRendererAnimator;

    [Header("Throw")]
    public int handPositionHistoryCount;
    private int lastHandHistoryIndex;
    private Vector3[] handDeltaHistory;
    private Vector3 previousHandPosition;
    public float throwForceMultiplier;

    protected override void Start() {
        handDeltaHistory = new Vector3[handPositionHistoryCount];
        lastHandHistoryIndex = 0;
        previousHandPosition = Vector3.zero;
    }

    public override void OnSelected(Hand castingHand) {
        base.OnSelected(castingHand);

        hoverDistance = initialHoverDistance;

        grabObjectAction.AddOnUpdateListener(GrabObject, castingHand.handType);
        pullPushAction.AddOnChangeListener(SetPullPushMode, castingHand.handType); 
    }

    public override void OnDeselected() {
        base.OnDeselected();

        grabObjectAction.RemoveOnUpdateListener(GrabObject, castingHand.handType);
        pullPushAction.RemoveOnChangeListener(SetPullPushMode, castingHand.handType); 
        isGrabbing = false;
        isPullPushModeActive = false;

        if (currentTarget!= null) {
            currentTarget.rb.useGravity = true;
        }
    }


    private void GrabObject(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) {
        if (!isSelected) return;

        if (isGrabbing) {
            if (!newState) {
                // -- Throw the object
                isGrabbing = false;
                currentTarget.rb.useGravity = true;

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

                    if (hit.collider.TryGetComponent(out ForceGrabbable forceGrabbable)) {
                        isGrabbing = true;
                        currentTarget = forceGrabbable;
                        currentTarget.rb.useGravity = false;
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
        if (newState) {
            hoverDistance += 1;
        }
    }

    private void Update() {
        if (!isSelected) return;

        if (isGrabbing) {
            // -- In the middle of a grab. Lerp the object towards the target
            currentTarget.transform.position = Vector3.Lerp(currentTarget.transform.position, 
                                                            castingHand.transform.position 
                                                                + castingHand.transform.rotation * handOffset
                                                                + castingHand.transform.forward * hoverDistance,
                                                            Time.deltaTime * objectFollowSpeed);
        }

        // -- Store a history of hand positions, so that we can neatly approximate the object's velocity post-release
        lastHandHistoryIndex = (lastHandHistoryIndex + 1) % handPositionHistoryCount;
        handDeltaHistory[lastHandHistoryIndex] = castingHand.transform.position - previousHandPosition;
        previousHandPosition = castingHand.transform.position;
    }
}
