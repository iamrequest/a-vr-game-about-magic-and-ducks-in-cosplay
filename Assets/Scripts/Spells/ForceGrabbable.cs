using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Rigidbody))]
public class ForceGrabbable : MonoBehaviour {
    public Rigidbody rb;

    // -- Used for resetting the position when the player throws the object off the edge of the world
    // -- cause it's gonna happen eventually
    private bool originalKinematicEnabled;
    private bool originalGravityEnabled;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    [Header("Socket")]
    public GameObject forceGrabSocket;
    public bool isSlotted;
    public float slotLerpSpeed;
    public float minSnapDistance;
    public UnityEvent onSlotEvent;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        isSlotted = false;

        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalKinematicEnabled = rb.isKinematic;
        originalGravityEnabled = rb.useGravity;
    }

    public void OnGrab() {
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
    }

    public void OnRelease() {
        if (!isSlotted) {
            //rb.useGravity = wasGravityEnabled;
            rb.useGravity = true;
        } 
    }

    // If we get close enough to the target, then 
    private void OnTriggerEnter(Collider other) {
        if (!isSlotted) {
            if (other.gameObject == forceGrabSocket) {
                isSlotted = true;

                rb.isKinematic = true;
                rb.useGravity = true;

                onSlotEvent.Invoke();
                StartCoroutine(DoSlotLerp());
            }
        }
    }

    private IEnumerator DoSlotLerp() {
        forceGrabSocket.SetActive(false);

        Vector3 posDelta = transform.position - forceGrabSocket.transform.position;

        while (Mathf.Abs(posDelta.sqrMagnitude) > minSnapDistance) {
            posDelta = transform.position - forceGrabSocket.transform.position;

            // Still too far from the target - lerp towards it
            transform.position = Vector3.Lerp(transform.position, forceGrabSocket.transform.position, Time.deltaTime * slotLerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, forceGrabSocket.transform.rotation, Time.deltaTime * slotLerpSpeed);
            yield return null;
        }

        // Test if we should just snap to the target position
        transform.position = forceGrabSocket.transform.position;
        transform.rotation = forceGrabSocket.transform.rotation;
        yield break;
    }

    public void ReturnToOriginalTransform() {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        rb.isKinematic = originalKinematicEnabled;
        rb.useGravity = originalGravityEnabled;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
