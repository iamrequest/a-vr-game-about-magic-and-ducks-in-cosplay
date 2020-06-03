using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Book : MonoBehaviour {
    private Animator animator;

    private Hand hand;
    public SteamVR_Action_Boolean openBookAction;

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
        hand = GetComponentInParent<Hand>();

        openBookAction.AddOnChangeListener(OpenCloseBook, hand.handType);
    }

    private void OpenCloseBook(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) {
        animator.SetBool("isBookOpen", newState);
    }
}
