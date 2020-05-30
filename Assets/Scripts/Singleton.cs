using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour {
    public static Singleton instance;
    // Start is called before the first frame update
    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }
}
