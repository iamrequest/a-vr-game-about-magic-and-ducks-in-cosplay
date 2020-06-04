using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class LightSpell : BaseSpell {
    private Animator animator;
    public GameObject lightSphere;
    public float followSpeed;
    public Vector3 offset;

    protected override void Start() {
        base.Start();
        animator = GetComponent<Animator>();
    }

    public override void OnSelected(Hand castingHand) {
        base.OnSelected(castingHand);

        lightSphere.transform.position = castingHand.transform.position + offset;
        animator.SetBool("isOpen", true);
    }

    public override void OnDeselected() {
        base.OnDeselected();

        animator.SetBool("isOpen", false);
    }

    private void Update() {
        if (isSelected) {
            // Lerp towards the player hand
            lightSphere.transform.position = Vector3.Lerp(lightSphere.transform.position, 
                                                          castingHand.transform.position + castingHand.transform.rotation * offset, 
                                                          Time.deltaTime * followSpeed);
        }
    }
}
