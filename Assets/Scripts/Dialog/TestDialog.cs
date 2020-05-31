using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TestDialog : BaseDialog {
    public List<Sentence> startingDialog;

    public SteamVR_Action_Boolean startDialogAction;

    // Initialize our list of sentences
    //  Since it's not a monobehaviour, we have to do this ourselves
    //  Do a nullcheck first, since Reset() will wipe out our dialog if we're not careful
    public void Reset() {
        if (startingDialog == null) startingDialog = new List<Sentence>();
    }

    // Start is called before the first frame update
    void Start() {
        startDialogAction.AddOnStateUpListener(StartDialog, SteamVR_Input_Sources.Any);
    }

    void TalkedTo() {
        // Enque start of convo
        dialogManager.StartDialog(startingDialog, true);
        // dialogManager.StartDialog(new Sentence(AnimationState.Idle, DialogSpeaker.Wizard, "yo"), false);

        // some condition
        if (true) {
            Sentence s = new Sentence(AnimationState.Anger,
                DialogSpeaker.Wizard,
                "I see you lit the bonfire");
            dialogManager.StartDialog(s, false);

            Sentence s2 = new Sentence(AnimationState.Anger,
                DialogSpeaker.Me,
                "it's lit af fam");
            dialogManager.StartDialog(s2, false);
        } else {
        }
    }

    private void StartDialog(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            TalkedTo();
    }
}
