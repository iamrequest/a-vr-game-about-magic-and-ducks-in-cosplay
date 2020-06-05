using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Calls a unity event for trigger events
public class OnTriggerListener : MonoBehaviour {
    public UnityEvent onTriggerEnter;

    private void OnTriggerEnter(Collider other) {
        onTriggerEnter.Invoke();
    }
}
