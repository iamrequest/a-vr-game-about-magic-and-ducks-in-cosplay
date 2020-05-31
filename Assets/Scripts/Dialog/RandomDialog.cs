﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDialog : BaseDialog {
    [SerializeField]
    public List<Conversation> conversations;


    // Initialize our list of sentences
    //  Since it's not a monobehaviour, we have to do this ourselves
    //  Do a nullcheck first, since Reset() will wipe out our dialog if we're not careful
    public void Reset() {
        if (conversations == null) conversations = new List<Conversation>();
    }

    public override void StartDialog() {
        // Pick a random dialog to say
        int convoIndex = Random.Range(0, conversations.Count - 1);

        dialogManager.StartDialog(conversations[convoIndex].sentences, true);
    }
}