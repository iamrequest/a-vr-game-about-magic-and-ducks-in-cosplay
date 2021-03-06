﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class BGMSource : MonoBehaviour {
    public AudioClip audioClip;
    private bool isTriggered;
    public UnityEvent onBGMStart;

    private void OnTriggerEnter(Collider other) {
        if (!isTriggered) {
            if (other.TryGetComponent(out VRAudioSource vrAudioSource)) {
                vrAudioSource.audioSource.clip = audioClip;
                vrAudioSource.audioSource.Play();

                // Cheap hack to get the first NPC to chat with the player when the player spawns
                onBGMStart.Invoke();
                isTriggered = true;
            }
        }
    }
}
