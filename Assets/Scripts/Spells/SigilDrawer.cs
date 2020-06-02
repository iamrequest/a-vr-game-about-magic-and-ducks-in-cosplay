using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(PointSigil))]
public class SigilDrawer : MonoBehaviour {
    public SpellCircle spellPlane;
    public LineRenderer lineRenderer;
    PointSigil sigil;

    [Header("SteamVR")]
    public SteamVR_Action_Boolean createSigilAction;
    Hand hand;

    [Header("Sigil Drawing")]
    public float pointAddDelay;
    private float timeSinceLastPoint;

    // Start is called before the first frame update
    void Start() {
        sigil = GetComponent<PointSigil>();

        hand = GetComponentInParent<Hand>();
        createSigilAction.AddOnUpdateListener(CreateSigil, hand.handType);
    }

    public int dir;
    // Every X frames, add a new point to our sigil.
    private void CreateSigil(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) {
        // User stopped drawing the sigil
        if (!spellPlane.isSpellPlaneActive) {
            sigil.points.Clear();
            lineRenderer.enabled = false;
            return;
        }

        if (newState) {
            timeSinceLastPoint += Time.deltaTime;

            if (timeSinceLastPoint > pointAddDelay) {
                timeSinceLastPoint = 0f;
                sigil.AddPointInWorldSpace(hand.transform.position, spellPlane);
            }
        }

        // Draw the sigil as is if we're currently drawing
        // Once we're done, transform the points to match the scale of the magic circle
        // TODO: Points are in local space by default, so when no transform is applied,
        //  the points won't render properly
        sigil.DrawSigil(spellPlane, lineRenderer, !newState);
    }
}
