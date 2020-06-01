using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineSigil))]
public class BaseSpell : MonoBehaviour {
    public SplineSigil sigil;
    void Start() {
        sigil = GetComponent<SplineSigil>();
    }

}
