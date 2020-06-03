using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

// TODO: This can cause issues where the player walks away mid-dialog.
//  How to test which dialog manager we're looking at? Multiple dialog managers (multi-person convo?)
public class DialogInteractor : MonoBehaviour {
    [Header("SteamVR")]
    public SteamVR_Action_Boolean skipSentenceAction;
    public SteamVR_Action_Boolean dialogInteractAction;
    private Hand hand;

    // -- Dialog management
    public DialogManager activeDialogManager;
    private DialogInteractor otherDialogInteractor;

    [Header("Raycast Interactions")]
    public LayerMask startDialogRaycastLayerMask;
    public float raycastDistance;
    private LineRenderer lineRenderer;
    private Animator lineRendererAnimator;

    public SpellCircle spellPlane;

    // Start is called before the first frame update
    void Start() {
        hand = GetComponentInParent<Hand>();
        otherDialogInteractor = hand.otherHand.GetComponentInChildren<DialogInteractor>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRendererAnimator = GetComponent<Animator>();

        skipSentenceAction.AddOnStateUpListener(SkipSentence, hand.handType);
        dialogInteractAction.AddOnStateUpListener(DialogInteract, hand.handType);
    }

    private void SkipSentence(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (activeDialogManager != null) {
            activeDialogManager.SkipCurrentSentence();
        }
    }

    private void DialogInteract(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        // If we have the spell plane open, end the conversation
        if (spellPlane.isSpellPlaneActive) {
            lineRenderer.enabled = false;
            if (activeDialogManager != null) {
                activeDialogManager.EndDialogEarly();
            }
            return;
        }

        lineRenderer.enabled = true;
        lineRendererAnimator.SetTrigger("fire");

        // Raycast forward from the hand
        // If we collide with something that we can chat with, then start that conversation
        RaycastHit hit;
        if (Physics.Raycast(hand.transform.position, hand.transform.forward * raycastDistance, out hit, startDialogRaycastLayerMask)) {
            if (hit.collider.TryGetComponent(out BaseDialog dialog)) {
                // If we aren't already in a conversation with this character, then start a new convo
                if (dialog.dialogManager != activeDialogManager || !activeDialogManager.isDialogActive) {
                    // Raycast to the collision point
                    lineRenderer.SetPosition(0, hand.transform.position);
                    lineRenderer.SetPosition(1, hit.point);
                    lineRendererAnimator.SetTrigger("fire");

                    // -- Phase out of the old dialog
                    //  WIP
                    if (activeDialogManager != null) {
                        activeDialogManager.EndDialogEarly();
                    }

                    // -- Start a new convo
                    activeDialogManager = dialog.dialogManager;
                    otherDialogInteractor.activeDialogManager = dialog.dialogManager;
                    dialog.StartDialog();

                    return;
                }
            }
        } else {
            // Raycast forward
            lineRenderer.SetPosition(0, hand.transform.position);
            lineRenderer.SetPosition(1, hand.transform.position + hand.transform.forward * raycastDistance);
            lineRendererAnimator.SetTrigger("fire");
        }

        // -- Next dialog line
        if (activeDialogManager != null) {
            activeDialogManager.DisplayNextSentence();
        }
    }
}
