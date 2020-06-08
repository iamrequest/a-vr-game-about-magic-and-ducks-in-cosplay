using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

// -- TODO: Add "return to original position" mode
[RequireComponent(typeof(Rigidbody))]
public class ForceGrabbable : MonoBehaviour {
    public Rigidbody rb;

    private bool wasKinematicEnabled;
    private bool wasGravityEnabled;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    public void OnGrab() {
        wasKinematicEnabled = rb.isKinematic;
        wasGravityEnabled = rb.useGravity;

        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = false;
    }

    public void OnRelease() {
        rb.isKinematic = wasKinematicEnabled;
        rb.useGravity = wasGravityEnabled;
    }
}
