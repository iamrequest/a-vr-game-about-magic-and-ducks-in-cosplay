using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

// TODO: This can cause issues where the player walks away mid-dialog.
//  How to test which dialog manager we're looking at? Multiple dialog managers (multi-person convo?)
public class DialogInteractor : MonoBehaviour {
    public SteamVR_Action_Boolean advanceTextAction;
    public SteamVR_Action_Boolean skipSentenceAction;
    public DialogManager activeDialogManager;

    // Start is called before the first frame update
    void Start() {
        advanceTextAction.AddOnStateUpListener(AdvanceText, SteamVR_Input_Sources.Any);
        skipSentenceAction.AddOnStateUpListener(SkipSentence, SteamVR_Input_Sources.Any);

        ControllerButtonHints.ShowButtonHint(Player.instance.rightHand, advanceTextAction);
        ControllerButtonHints.ShowButtonHint(Player.instance.rightHand, skipSentenceAction);
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void SkipSentence(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (activeDialogManager != null) {
            activeDialogManager.SkipCurrentSentence();
        }
    }

    private void AdvanceText(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (activeDialogManager != null) {
            activeDialogManager.DisplayNextSentence();
        }
    }
}
