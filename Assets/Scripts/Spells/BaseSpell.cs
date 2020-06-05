using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(SplineSigil))]
public class BaseSpell : MonoBehaviour {
    public SplineSigil sigil;

    protected Hand castingHand;
    public bool isSelected {
        get; protected set;
    }

    protected virtual void Start() {
        sigil = GetComponent<SplineSigil>();
    }

    // Called when the sigil is detected, and the magic circle goes away
    public virtual void OnSelected(Hand castingHand) {
        isSelected = true;
        this.castingHand = castingHand;
    }

    // Called when the magic circle comes back up
    public virtual void OnDeselected() {
        isSelected = false;
        //castingHand = null;
    }
}
