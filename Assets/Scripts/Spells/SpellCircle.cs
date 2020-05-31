using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SpellCircle : MonoBehaviour {
    public SteamVR_Input_Sources handType;
    private Hand hand;
    public SteamVR_Action_Boolean drawMagicCircleAction;

    private DialogInteractor dialogInteractor;

    public SpriteRenderer magicCircleSpriteRenderer;
    public float spawnDistance;

    // Start is called before the first frame update
    void Start() {
        if (handType == SteamVR_Input_Sources.LeftHand) {
            hand = Player.instance.leftHand;
        } else {
            hand = Player.instance.rightHand;
        }
        drawMagicCircleAction.AddOnChangeListener(DrawMagicCircle, hand.handType);

        dialogInteractor = hand.GetComponentInChildren<DialogInteractor>();
    }

    private void DrawMagicCircle(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) {
        // Always allow us to stop showing the magic circle
        if (!newState) {
            magicCircleSpriteRenderer.enabled = false;
            return;
        }

        // Only draw the magic circle if we're not in the middle of a conversation
        if (dialogInteractor.activeDialogManager == null || !dialogInteractor.activeDialogManager.isDialogActive) {
            magicCircleSpriteRenderer.enabled = true;

            Vector3 dirFromEyeToHand = hand.transform.position - Player.instance.hmdTransform.transform.position;
            transform.position = hand.transform.position + (dirFromEyeToHand.normalized * spawnDistance);
            transform.LookAt(Player.instance.hmdTransform, Vector3.up);
        }
    }
}
