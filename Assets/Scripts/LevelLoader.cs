using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

// Don't forget to set the Collision Layer to "Player Triggers"
// Also, don't forget to set this layer so that it only collides with the player layer
public class LevelLoader : MonoBehaviour {
    private SteamVR_LoadLevel levelLoader;

    private void Start() {
        levelLoader = GetComponent<SteamVR_LoadLevel>();
    }

    private void OnTriggerEnter(Collider other) {
        levelLoader.enabled = true;
    }
}
