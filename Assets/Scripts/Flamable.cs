using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamable : MonoBehaviour {
    public ParticleSystem[] fireParticleSystem;
    public bool isFireLit;

    private void Start() {
        foreach (ParticleSystem particleSystem in fireParticleSystem) {
            if (isFireLit) {
                particleSystem.Play();
            } else {
                particleSystem.Stop();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        // If this flamable object isn't already lit, then light it when it collides with a fireball
        if (!isFireLit) {
            if (other.TryGetComponent(out FireballProjectile fireball)) {
                isFireLit = true;
                fireball.rb.isKinematic = true;
                fireball.DestroyAfterLifespan(1f);

                foreach (ParticleSystem particleSystem in fireParticleSystem) {
                    particleSystem.Play();
                }
            }
        }
    }
}
