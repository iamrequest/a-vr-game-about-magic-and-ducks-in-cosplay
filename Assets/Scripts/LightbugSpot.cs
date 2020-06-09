using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightbugSpot : MonoBehaviour {
    public ParticleSystem lightBugs;
    public Light lightSource;
    public bool isLightLit;
    public bool isSource;
    public bool becomeLightSourceOnLit;
    public GameObject volumetricLight;

    public UnityEvent onLit;

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
                lightSource.enabled = isLightLit;
            }

            if (volumetricLight != null) {
                volumetricLight.SetActive(isLightLit);
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
                    isLightLit = true;

                    // Transfer the light bugs to the target
                    lightBugs.Play();
                    if (becomeLightSourceOnLit) {
                        isSource = true;
                    } else {
                        lightSpell.StopLightBugs();
                    }

                    if (lightSource != null) {
                        lightSource.enabled = true;
                    }
                    if (volumetricLight != null) {
                        volumetricLight.SetActive(true);
                    }

                    onLit.Invoke();
                }
            }
        }
    }
}
