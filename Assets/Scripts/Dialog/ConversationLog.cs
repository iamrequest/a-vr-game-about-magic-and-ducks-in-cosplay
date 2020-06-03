using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationLog : MonoBehaviour {
    public static ConversationLog instance;
    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }
}
