using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class killplane : MonoBehaviour {
    public Transform spawnTransform;

    private void OnTriggerEnter(Collider other) {
        if (other == Player.instance.headCollider) {
            Debug.Log(other);
            Player.instance.transform.position = spawnTransform.position;
            Player.instance.transform.rotation = spawnTransform.rotation;

            return;
        } 

        // Test for force grabbable objects
        ForceGrabbable forceGrabbable = other.GetComponent<ForceGrabbable>();
        if (forceGrabbable == null) {
            forceGrabbable = other.GetComponentInParent<ForceGrabbable>();
        }

        if (forceGrabbable != null) {
            forceGrabbable.ReturnToOriginalTransform();
            return;
        }
    }
}
