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

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }
}
