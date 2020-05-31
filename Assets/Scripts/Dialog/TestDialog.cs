using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TestDialog : BaseDialog {
    public List<Sentence> startingDialog;

    // Initialize our list of sentences
    //  Since it's not a monobehaviour, we have to do this ourselves
    //  Do a nullcheck first, since Reset() will wipe out our dialog if we're not careful
    public void Reset() {
        if (startingDialog == null) startingDialog = new List<Sentence>();
    }

    public override void StartDialog() {
        // Enque start of convo
        dialogManager.StartDialog(startingDialog, true);

    }
}
