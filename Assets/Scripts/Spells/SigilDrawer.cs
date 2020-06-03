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

    // --------------------------------------------------------------------------------
    // Spline detection
    // --------------------------------------------------------------------------------
    // For each point in our manually-drawn point sigil, compare the distance from this point
    //  to a point on the sigil. 
    // Calculate the distance from 0->t, and from t->0
    bool tmpIsForwardDir, isForwardDir;
    private float CalculateDistance(SplineSigil spline) {
        float distanceForward = 0f;
        float distanceBackwards = 0f;

        for (int i = 0; i < sigil.points.Count; i++) {
            distanceForward += DoDistanceComparison(spline, i, i);
        }
        for (int i = 0; i < sigil.points.Count; i++) {
            distanceBackwards += DoDistanceComparison(spline, i, sigil.points.Count - i - 1);
        }

        Debug.Log("  Forward: " + distanceForward);
        Debug.Log("  Backwards: " + distanceBackwards);

        tmpIsForwardDir = distanceForward < distanceBackwards;
        if (distanceForward < distanceBackwards) return distanceForward;
        else return distanceBackwards;
    }

    private float DoDistanceComparison(SplineSigil spline, int splineSigilIndex, int pointSigilIndex) {
        // Find the point on the pre-defined spell spline
        float splineIndex = 0f;
        if (splineSigilIndex > 0) {
            splineIndex = (float)splineSigilIndex / (float)(sigil.points.Count - 1);
        }

        // Scale the point along the spline, and the current point in our manual sigil
        Vector3 transformedSplinePoint = spline.GetPointOnSpellPlane(splineIndex, spellPlane);
        Vector3 transformedPoint = sigil.TranslatePointToSpellPlane(sigil.points[pointSigilIndex], spellPlane);

        Vector3 pointDelta = transformedSplinePoint - transformedPoint;
        return Mathf.Abs(pointDelta.sqrMagnitude);
    }

    public BaseSpell DetectSpell() {
        if (spellPlane.availableSpells.Count == 0) return null;

        float minDistance = 9999f;
        BaseSpell detectedSpell = spellPlane.availableSpells[0];
        float currentDistance;


        foreach (BaseSpell spell in spellPlane.availableSpells) {
            Debug.Log("-- " + spell.name);
            //Debug.Log(spell.name + " : " + currentDistance);
            currentDistance = CalculateDistance(spell.sigil);

            if (currentDistance < minDistance) {
                isForwardDir = tmpIsForwardDir;
                minDistance = currentDistance;
                detectedSpell = spell;
            }
        }

        return detectedSpell;
    }

    // --------------------------------------------------------------------------------
    // Debug drawing deltas
    // --------------------------------------------------------------------------------
    private void Update() {
        // Draw the sigil bounding box
        if (spellPlane.isSpellPlaneActive && sigil.points.Count > 0) {
            BaseSpell detectedSpell = DetectSpell();
            detectedSpell.sigil.DrawSigil(spellPlane, spellPlane.spellLineRenderer, true);

            DrawSigilDeltas(spellPlane.availableSpells[0], Color.red);
            //DrawSigilDeltas(spellPlane.availableSpells[1], Color.green);
        }
    }

    private void DrawSigilDeltas(BaseSpell spell, Color c) {
        if (isForwardDir) {
            for (int i = 0; i < sigil.points.Count; i++) {
                DoDrawSigilDelta(spell, c, i, i);
            }
        } else {
            for (int i = 0; i < sigil.points.Count; i++) {
                DoDrawSigilDelta(spell, c, i, sigil.points.Count - i - 1);
            }
        }
    }

    private void DoDrawSigilDelta(BaseSpell spell, Color c, int splineSigilIndex, int pointSigilIndex) {
        float splineIndex = 0f;
        if (splineSigilIndex > 0) {
            splineIndex = (float)splineSigilIndex / (float)(sigil.points.Count - 1);
        }

        // Scale the point along the spline, and the current point in our manual sigil
        Vector3 transformedSplinePoint = spell.sigil.GetPointOnSpellPlane(splineIndex, spellPlane);
        Vector3 transformedPoint = sigil.TranslatePointToSpellPlane(sigil.points[pointSigilIndex], spellPlane);

        Debug.DrawLine(spellPlane.transform.position +
                        spellPlane.transform.rotation * transformedSplinePoint,
                       spellPlane.transform.position +
                        spellPlane.transform.rotation * transformedPoint,
                       c);
    }
}
