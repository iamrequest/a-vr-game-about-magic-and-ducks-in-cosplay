using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

// This script ends a dialog if the player is too far from the speaker
// Please make sure that the player's "start dialog" raycast distance is less than maxConversationDistance,
//  plus some extra distance to account for the positional difference between the HMD position and controller position
[RequireComponent(typeof(DialogManager))]
public class EndDialogAtDistance : MonoBehaviour {
    public float maxConversationDistance;
    private DialogManager dialogManager;

    // Start is called before the first frame update
    void Start() {
        dialogManager = GetComponent<DialogManager>();
    }

    // Update is called once per frame
    void Update() {
        Debug.Log("Dialog active: " + dialogManager.isDialogActive);
        if (dialogManager.isDialogActive) {
            Vector3 positionDelta = dialogManager.transform.position - Player.instance.hmdTransform.transform.position;

            // Using sqrmagnitude because square root operations are expensive - magnitude uses square root
            float sqrDistance = Mathf.Abs(positionDelta.sqrMagnitude);
            if (sqrDistance > maxConversationDistance * maxConversationDistance) {
                dialogManager.EndDialogEarly();
            }

            Debug.Log("Distance to dialog manager: " + sqrDistance);
        }
    }
}
