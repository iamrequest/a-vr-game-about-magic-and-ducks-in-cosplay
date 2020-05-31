using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomDialog))]
public class RandomDialogInspector : Editor {
    private List<bool> expandSentences;
    private List<string> convoLabels;

    public void Reset() {
        if (expandSentences == null) {
            expandSentences = new List<bool>();
        }
        if (convoLabels == null) {
            convoLabels = new List<string>();
        }
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        Reset();
        EditorGUILayout.LabelField("Conversations", EditorStyles.boldLabel);

        RandomDialog dialog = target as RandomDialog;
        for (int i = 0; i < dialog.conversations.Count; i++) {
            AddSentenceEditor(i);

            if (expandSentences[i]) {
                EditorGUILayout.Space();
            }
        }

        if (GUILayout.Button("Add Conversation")) {
            Conversation newConvo = new Conversation();
            newConvo.sentences.Add(new Sentence());
            dialog.conversations.Add(newConvo);

            expandSentences.Add(true);
            convoLabels.Add("New Conversation");
        }
    }

    private void AddSentenceEditor(int i) {
        RandomDialog dialog = target as RandomDialog;

        Sentence sentenceToDelete;
        expandSentences[i] = EditorGUILayout.Foldout(expandSentences[i], convoLabels[i]);
        EditorGUI.indentLevel += 1;

        if (expandSentences[i]) {
            sentenceToDelete = null;

            // -- Editor-only field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Convo topic:", GUILayout.MaxWidth(100));
            convoLabels[i] = EditorGUILayout.TextField(convoLabels[i]);
            EditorGUILayout.EndHorizontal();

            foreach (Sentence s in dialog.conversations[i].sentences) {
                if (s == null) break;

                // -- Horizontal section
                EditorGUILayout.BeginHorizontal();
    
                EditorGUILayout.LabelField("Speaker:", GUILayout.MaxWidth(70));
                s.currentSpeaker = (DialogSpeaker) EditorGUILayout.EnumPopup(s.currentSpeaker);
    
                EditorGUILayout.LabelField("Animation:", GUILayout.MaxWidth(70));
                s.animationState = (AnimationState) EditorGUILayout.EnumPopup(s.animationState);
    
                if (GUILayout.Button("-")) {
                    // Can't remove an element in the middle of a foreach loop
                    sentenceToDelete = s;
                }
                EditorGUILayout.EndHorizontal();
    
                // -- Text field
                s.text = EditorGUILayout.TextField(s.text);
            }

            if (sentenceToDelete != null) {
                dialog.conversations[i].sentences.Remove(sentenceToDelete);
            }
    
            // -- Add sentence / remove convo
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Sentence")) {
                dialog.conversations[i].sentences.Add(new Sentence());
            }

            if (GUILayout.Button("Delete conversation")) {
                dialog.conversations.RemoveAt(i);
                expandSentences.RemoveAt(i);
                convoLabels.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel -= 1;
    }
}
