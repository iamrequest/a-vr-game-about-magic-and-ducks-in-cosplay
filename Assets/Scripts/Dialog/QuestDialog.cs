using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Quick spaghetti that maintains a "quest" system.
//  User has a list of objectives. For each objective, there's start/finish dialog that must be played to advance.
//  During the objective, the NPC will loop through the same convo until the objective is marked as complete
public class QuestDialog : BaseDialog {
    public List<QuestObjective> objectives;
    public int activeObjectiveIndex;

    public Conversation allObjectivesCompleteDialog;

    public override void StartDialog() {
        // No quests exist
        if (objectives.Count == 0) {
            dialogManager.StartDialog(this, allObjectivesCompleteDialog, true);
            return;
        }

        // -- Always play the initial dialog
        // TODO: All quests complete dialog isn't playing
        if (!objectives[activeObjectiveIndex].initialDialogComplete) {
            dialogManager.StartDialog(this, objectives[activeObjectiveIndex].initialDialog, true);
            return;
        }
        
        if (IsReadyToAdvanceToNextObjective()) {
            if (IsFinalObjective()) {
                // -- No more objectives - tell the player to finish some other quest
                dialogManager.StartDialog(this, allObjectivesCompleteDialog, true);
                return;
            } else {
                // -- Prepare the next objective, if one exists
                activeObjectiveIndex++;

                // Initial dialog for the new objective
                if (!objectives[activeObjectiveIndex].initialDialogComplete) {
                    dialogManager.StartDialog(this, objectives[activeObjectiveIndex].initialDialog, true);
                    return;
                } else {
                    Debug.LogError("Objective " + activeObjectiveIndex + " is null!");
                }
            }
        }

        // -- Dialog for the current objective
        if (objectives[activeObjectiveIndex] != null) {
            if (objectives[activeObjectiveIndex].isComplete) {
                // -- Objective complete
                dialogManager.StartDialog(this, objectives[activeObjectiveIndex].onCompleteDialog, true);
            } else {
                if (!objectives[activeObjectiveIndex].initialDialogComplete) {
                    // -- First dialog after the objective began
                    dialogManager.StartDialog(this, objectives[activeObjectiveIndex].initialDialog, true);
                } else {
                    // -- N'th dialog after the objective began
                    dialogManager.StartDialog(this, objectives[activeObjectiveIndex].repeatDialog, true);
                }
            }
        } else {
            Debug.LogError("Objective " + activeObjectiveIndex + " is null!");
        }
    }

    public override void OnDialogEnd(bool wasDialogFullyCompleted) {
        // -- Only advance objective dialog when the conversation ended completely
        if (wasDialogFullyCompleted) {
            if (objectives[activeObjectiveIndex] == null) {
                Debug.LogError("Objective " + activeObjectiveIndex + " is null!");
                return;
            }
            
            // -- Test the initial dialog for completion
            if (!objectives[activeObjectiveIndex].initialDialogComplete) {
                objectives[activeObjectiveIndex].initialDialogComplete = true;
                objectives[activeObjectiveIndex].onObjectiveStart.Invoke();
                return;
            }

            // If you've finished talking to the NPC, and the quest has been completed, advance to the next quest
            if (objectives[activeObjectiveIndex].isComplete) {
                objectives[activeObjectiveIndex].finalDialogComplete = true;
                objectives[activeObjectiveIndex].onObjectiveComplete.Invoke();
            }
        }
    }

    // Current objective is complete, and the final dialog for this quest has been said
    private bool IsReadyToAdvanceToNextObjective() {
        return objectives[activeObjectiveIndex].isComplete 
            && objectives[activeObjectiveIndex].finalDialogComplete;
    }

    private bool IsFinalObjective() {
        return activeObjectiveIndex == objectives.Count - 1;
    }
}
