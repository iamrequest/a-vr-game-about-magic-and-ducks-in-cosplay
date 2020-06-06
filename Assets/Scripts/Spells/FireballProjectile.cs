using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : MonoBehaviour {
    public Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
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
}
