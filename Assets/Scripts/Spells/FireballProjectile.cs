using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : MonoBehaviour {
    public Animator animator;
    public Rigidbody rb;

    private void Start() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public void DestroyAfterLifespan(float lifespan) {
        StartCoroutine(SetAnimatorDestroyAfterLifespan(lifespan - 1));
        StartCoroutine(DoDestroyAfterLifetime(lifespan));
    }

    private IEnumerator SetAnimatorDestroyAfterLifespan(float lifespan) {
        yield return new WaitForSeconds(lifespan);
        animator.SetTrigger("isDestroying");
    }
    private IEnumerator DoDestroyAfterLifetime(float lifespan) {
        yield return new WaitForSeconds(lifespan);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        rb.isKinematic = true;
        animator.SetTrigger("isDestroying");
        DoDestroyAfterLifetime(1f);
    }
}
