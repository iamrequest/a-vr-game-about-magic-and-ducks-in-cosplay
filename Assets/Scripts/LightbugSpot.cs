using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightbugSpot : MonoBehaviour {
    public ParticleSystem lightBugs;
    public Light lightSource;
    public bool isSource;
    public bool isLit {
        get {
            return lightBugs.isPlaying;
        }
    }

    private void Start() {
        if (isSource) {
            lightBugs.Play();
        } else {
            lightBugs.Stop();

            if (lightSource != null) {
                lightSource.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        LightSpell lightSpell = other.GetComponentInParent<LightSpell>();
        if (lightSpell != null) {
            if (isSource) {
                // If this is an area that spawns light bugs, then start spawning them at the light spell
                // ie: start playing the particle system
                lightSpell.StartLightBugs();
            } else {
                // If this is a place where we can drop off lightbugs, then do so here
                if (lightSpell.isSelected && lightSpell.hasLightBugs && lightBugs.isStopped) {
                    // Transfer the light bugs to the target
                    lightBugs.Play();
                    lightSpell.StopLightBugs();

                    if (lightSource != null) {
                        lightSource.enabled = true;
                    }
                }
            }
        }
    }
}
