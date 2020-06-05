using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class LightSpell : BaseSpell {
    [Header("Main spell")]
    private Animator animator;
    public GameObject lightSphere;
    public float followSpeed;
    public Vector3 offset;

    [Header("Light Bugs")]
    public ParticleSystem lightBugs;
    public bool hasLightBugs {
        get {
            return lightBugs.isPlaying;
        }
    }

    protected override void Start() {
        base.Start();
        animator = GetComponent<Animator>();
    }

    public override void OnSelected(Hand castingHand) {
        base.OnSelected(castingHand);

        lightSphere.transform.position = castingHand.transform.position + offset;
        animator.SetBool("isOpen", true);
        StopLightBugs();
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

            // If the light bugs are swarming around the light spell, then 
            if (lightBugs.isPlaying) {
                lightBugs.transform.position = Vector3.Lerp(lightBugs.transform.position,
                                                            lightSphere.transform.position,
                                                            Time.deltaTime * followSpeed);
            }
        }
    }

    public void StartLightBugs() {
        Debug.Log("Trigger caught");
        // Start playing the particle system
        if (lightBugs.isStopped && isSelected) {
            Debug.Log("Starting");
            lightBugs.Play();
        }
    }

    public void StopLightBugs() {
        // Stop playing the particle system, after the light bugs die off
        lightBugs.Stop();
    }
}
