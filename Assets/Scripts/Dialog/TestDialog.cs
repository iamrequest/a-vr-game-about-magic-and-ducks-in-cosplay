using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TestDialog : BaseDialog {
    [SerializeField]
    public List<Sentence> startingDialog;

    public SteamVR_Action_Boolean startDialogAction;

    // Start is called before the first frame update
    void Start() {
        startDialogAction.AddOnStateUpListener(StartDialog, SteamVR_Input_Sources.Any);
    }

    void TalkedTo() {
        // Enque start of convo
        dialogManager.StartDialog(startingDialog, true);
        dialogManager.StartDialog(new Sentence(AnimationState.Idle, DialogSpeaker.Wizard, "yo"), true);

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
