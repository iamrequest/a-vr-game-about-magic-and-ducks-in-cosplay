using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestDialog))]
public class BaseDialogInspector : Editor {
    private bool expandSentences = false;
    private Sentence sentenceToDelete;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        TestDialog td = target as TestDialog;
        AddSentenceEditor(td.startingDialog, "Starting Dialog: " + td.startingDialog.Count + " lines");
    }

    private void AddSentenceEditor(List<Sentence> sentences, string label) {
        expandSentences = EditorGUILayout.Foldout(expandSentences, label);

        if (expandSentences) {
            sentenceToDelete = null;

            foreach (Sentence s in sentences) {
                if (s == null) break;

                // -- Horizontal section
                EditorGUILayout.BeginHorizontal();
    
                EditorGUILayout.LabelField("Speaker:", GUILayout.MaxWidth(60));
                s.currentSpeaker = (DialogSpeaker) EditorGUILayout.EnumPopup(s.currentSpeaker);
    
                EditorGUILayout.LabelField("Animation:", GUILayout.MaxWidth(60));
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
                sentences.Remove(sentenceToDelete);
            }
    
            if (GUILayout.Button("Add Sentence")) {
                sentences.Add(new Sentence());
            }
        }
    }
}
