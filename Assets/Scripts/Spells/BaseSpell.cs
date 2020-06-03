using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineSigil))]
public class BaseSpell : MonoBehaviour {
    public SplineSigil sigil;

    void Start() {
        sigil = GetComponent<SplineSigil>();
    }

    // Called when the sigil is detected, and the magic circle goes away
    public void OnSelected() {
    }

    // Called when the magic circle comes back up
    public void OnDeselected() {
    }
}
