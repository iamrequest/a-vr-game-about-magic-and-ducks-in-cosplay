using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SpellCircle : MonoBehaviour {
    private DialogInteractor dialogInteractor;
    public bool isSpellPlaneActive {
        get {
            return magicCircleSpriteRenderer.enabled;
        }
    }

    [Header("SteamVR")] 
    public SteamVR_Input_Sources handType;
    private Hand hand;

    public SteamVR_Action_Boolean drawMagicCircleAction;

    [Header("Circle sprite")] 
    public SpriteRenderer magicCircleSpriteRenderer;
    public float spawnDistance;
    public float diameter;

    [Header("Sigils")]
    public List<BaseSpell> availableSpells;
    private LineRenderer spellLineRenderer;

    [Tooltip("When drawing the spell sigil's spline, how many points should we use to render it?")]
    public Vector3 manualSigilPositionOffset;

    // Start is called before the first frame update
    void Start() {
        if (handType == SteamVR_Input_Sources.LeftHand) {
            hand = Player.instance.leftHand;
        } else {
            hand = Player.instance.rightHand;
        }
        drawMagicCircleAction.AddOnChangeListener(DrawMagicCircle, hand.handType);

        dialogInteractor = hand.GetComponentInChildren<DialogInteractor>();
        spellLineRenderer = GetComponent<LineRenderer>();
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

    private void Update() {
        // Draw the sigil bounding box
        if (handType == SteamVR_Input_Sources.LeftHand) {
            availableSpells[0].sigil.DrawSigil(this, spellLineRenderer, true);
        } else {
            availableSpells[1].sigil.DrawSigil(this, spellLineRenderer, true);
        }
    }
}
