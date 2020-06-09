using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Same as QuestObjective, but this objective only gets marked as complete if all subobjectives are finished
public class QuestSubObjectives : QuestObjective {
    private int numSubObjectivesComplete = 0;
    public int totalSubObjectives;

    public void CompleteSubObjective() {
        numSubObjectivesComplete++;

        if (numSubObjectivesComplete >= totalSubObjectives) {
            CompleteObjective();
        }
    }
}
