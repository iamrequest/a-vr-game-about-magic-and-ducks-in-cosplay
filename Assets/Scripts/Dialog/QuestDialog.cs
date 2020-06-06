using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDialog : BaseDialog {
    public bool hasFinishedInitialDialog = false;
    public List<Sentence> initialDialog;
    public List<Sentence> followingDialog;

    private void Start() {
        if (initialDialog.Count == 0) {
            hasFinishedInitialDialog = true;
        }
    }

    public override void StartDialog() {
        if (!hasFinishedInitialDialog) {
            dialogManager.StartDialog(this, initialDialog, true);
        } else {
            dialogManager.StartDialog(this, followingDialog, true);
        }
    }

    public override void OnDialogEnd(bool wasDialogFullyCompleted) {
        if (wasDialogFullyCompleted) {
            hasFinishedInitialDialog = true;
        }
    }
}
