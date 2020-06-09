using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using TMPro;

public class Book : MonoBehaviour {
    private Animator animator;

    private Hand hand;
    public SteamVR_Action_Boolean openBookAction;

    public List<string> bookTitles;
    public TextMeshProUGUI bookTitleUI;

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
        hand = GetComponentInParent<Hand>();

        openBookAction.AddOnChangeListener(OpenCloseBook, hand.handType);
        animator.SetBool("isBookOpen", false);
    }

    private void OpenCloseBook(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) {
        animator.SetBool("isBookOpen", newState);

        // If we're opening the book, give it a random title
        if (newState) {
            int titleIndex = Random.Range(0, bookTitles.Count);
            bookTitleUI.text = bookTitles[titleIndex];
        }
    }
}
