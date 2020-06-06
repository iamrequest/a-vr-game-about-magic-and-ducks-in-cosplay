using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class FireSpell : BaseSpell {
    public SteamVR_Action_Boolean throwFireballAction;
    public Vector3 handOffset; // Offset from the hand while channeling

    [Header("Prefab instantiation")]
    public GameObject fireballPrefab;
    private GameObject currentFireball;
    public float projectileLifespan;
    public float fireballFollowSpeed;

    private bool isChanneling;
    public float channelDelay;
    private float timeSinceLastCast;

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

        ChannelFireball();

        throwFireballAction.AddOnChangeListener(ThrowFireball, castingHand.handType);
    }

    public override void OnDeselected() {
        base.OnDeselected();
        throwFireballAction.AddOnChangeListener(ThrowFireball, castingHand.handType);

        if (currentFireball != null) {
            Destroy(currentFireball.gameObject);
            isChanneling = false;
        }
    }

    private void ChannelFireball() {
        if (!isSelected) return;

        isChanneling = true;
        currentFireball = Instantiate(fireballPrefab);
        currentFireball.transform.position = castingHand.transform.position + 
            castingHand.transform.rotation * handOffset;
    }

    private void ThrowFireball(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) {
        if (!isSelected || !isChanneling) return;

        if (newState) {
            // -- Prepare the fireball (ie: expand it)
            if (currentFireball.TryGetComponent(out FireballProjectile fbp)) {
                fbp.animator.SetTrigger("isChanneling");
            }
        } else {
            // -- Throw the fireball
            isChanneling = false;
            timeSinceLastCast = 0f;

            if (currentFireball.TryGetComponent(out FireballProjectile fbp)) {
                fbp.DestroyAfterLifespan(projectileLifespan);

                // Add force to the projectile
                Vector3 handDelta = Vector3.zero;

                // TODO: Iterate over each hand position, and calculate the delta. Add them up, and find the average
                foreach(Vector3 delta in handDeltaHistory) {
                    handDelta += delta;
                }

                fbp.rb.AddForce(handDelta / handPositionHistoryCount * throwForceMultiplier, ForceMode.VelocityChange);
            }
        }
    }
    private void Update() {
        if (!isSelected) return;

        if (!isChanneling) {
            timeSinceLastCast += Time.deltaTime;

            if (timeSinceLastCast > channelDelay) {
                ChannelFireball();
            }
        }

        if (isChanneling) {
            if (currentFireball == null) {
                // Player collided the fireball with some environment and it destroyed itself. Instantiate a new one
                isChanneling = false;
                timeSinceLastCast = 0f;

                return;
            }

            LerpTowardsCastingHand(currentFireball.transform, fireballFollowSpeed, handOffset);

            lastHandHistoryIndex = (lastHandHistoryIndex + 1) % handPositionHistoryCount;

            handDeltaHistory[lastHandHistoryIndex] = castingHand.transform.position - previousHandPosition;
            previousHandPosition = castingHand.transform.position;
        }
    }
}
