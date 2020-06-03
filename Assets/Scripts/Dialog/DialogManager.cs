using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Valve.VR.InteractionSystem;

public class DialogManager : MonoBehaviour {
    private Queue<Sentence> sentences;
    private bool skipCurrentSentence;
    private Sentence currentSentence;

    public bool isDialogActive { get; private set; }

    [Header("UI Elements")]
    public CanvasCharacter canvasCharacter;
    public TextMeshProUGUI textUI;
    public SpriteRenderer dialogBG;
    public SpriteRenderer nameboxBG;
    public TextMeshProUGUI nameboxTextUI;
    public Animator animator;

    [Header("Writing dialog to UI")]
    public float dialogSpeed;
    private float timeSinceLastLetterTyped;
    private bool completedCurrentSentence;

    public AudioSource audioSource;
    public AudioClip typingAudio;

    void Awake() {
        sentences = new Queue<Sentence>();
        completedCurrentSentence = true;
    }

    public void StartDialog(Sentence newSentence, bool clearExistingDialog) {
        animator.SetBool("isDialogBoxOpen", true);
        isDialogActive = true;

        if (clearExistingDialog) {
            sentences.Clear();
            completedCurrentSentence = true;
        }

        sentences.Enqueue(newSentence);

        if (clearExistingDialog) {
            DisplayNextSentence();
        }
    }
    public void StartDialog(List<Sentence> newSentences, bool clearExistingDialog) {
        animator.SetBool("isDialogBoxOpen", true);
        isDialogActive = true;

        if (clearExistingDialog) {
            sentences.Clear();
            completedCurrentSentence = true;
        }

        foreach (Sentence s in newSentences) {
            sentences.Enqueue(s);
        }

        if (clearExistingDialog) {
            DisplayNextSentence();
        }
    }

    public void SkipCurrentSentence() {
        skipCurrentSentence = true;
    }
    public void DisplayNextSentence() {
        if (!completedCurrentSentence) {
            //skipCurrentSentence = true;
            return;
        }

        if (sentences.Count == 0) {
            EndDialog();
            return;
        }

        // Prepare the dialog
        currentSentence = sentences.Dequeue();
        ConversationLog.instance.AddSentence(currentSentence);

        // Apply the animation state, prepare the textbox
        canvasCharacter.SetAnimationState(currentSentence.animationState);
        ConfigureTextboxImages();

        // Start typing
        StartCoroutine(TypeSentence());
    }

    public void EndDialogEarly() {
        sentences.Clear();
        EndDialog();
    }
    private void EndDialog() {
        textUI.text = "";
        isDialogActive = false;
        completedCurrentSentence = true;
        animator.SetBool("isDialogBoxOpen", false);
    }

    private IEnumerator TypeSentence() {
        textUI.text = "";
        skipCurrentSentence = false;
        timeSinceLastLetterTyped = 0f;
        completedCurrentSentence = false;

        // TODO: Wait for opening animation
        foreach (char letter in currentSentence.text.ToCharArray()) {
            // If the dialog ended early for some reason, stop this typing coroutine
            //  This stops a bug where old text can bleed into a new convo when the player ends dialog early
            //  and then starts a new dialog quickly.
            if (!isDialogActive) {
                yield break;
            }

            // If the player hit the (figurative) B button, skip the dialog to the end.
            if (skipCurrentSentence) {
                // -- Skip through all the audio
                textUI.text = currentSentence.text;
                audioSource.PlayOneShot(typingAudio);

                completedCurrentSentence = true;
                yield break;
            }

            // Wait for the next character to be typed
            do {
                timeSinceLastLetterTyped += Time.deltaTime;
                yield return null;
            } while (dialogSpeed > timeSinceLastLetterTyped);

            // -- Type a single character
            timeSinceLastLetterTyped = 0f;
            textUI.text += letter;
            audioSource.PlayOneShot(typingAudio);

            yield return null;
        }

        completedCurrentSentence = true;
    }

    private void ConfigureTextboxImages() {
        // -- Set namebox color and text
        nameboxBG.color = SpeakerManager.instance.GetSpeakerColor(currentSentence.currentSpeaker);
        nameboxTextUI.text = currentSentence.currentSpeaker.ToString();

        // -- Set dialog box color and bg image
        dialogBG.color = SpeakerManager.instance.GetSpeakerColor(currentSentence.currentSpeaker);

        if (currentSentence.currentSpeaker == DialogSpeaker.Me) {
            dialogBG.sprite = SpeakerManager.instance.playerTextbox;
        } else {
            dialogBG.sprite = SpeakerManager.instance.npcTextbox;
        }
    }
}
