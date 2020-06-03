using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BezierSpline))]
public class SplineSigil : BaseSigil {
    //public SpellCircle spellPlane;
    private BezierSpline sigil;
    public int numPointsInSigil;

    public bool manuallySetBoundingBox;

    // Start is called before the first frame update
    void Start() {
        sigil = GetComponent<BezierSpline>();
        CalculateBoundingBox();
    }

    // --------------------------------------------------------------------------------
    // Sigil position
    // --------------------------------------------------------------------------------
    public Vector3 GetPointOnSpellPlane(float t, SpellCircle spellPlane) {
        return TranslatePointToSpellPlane(sigil.GetPointLocalSpace(t), spellPlane);
    }

    // --------------------------------------------------------------------------------
    // Sigil rendering
    // --------------------------------------------------------------------------------
    public override void DrawSigil(SpellCircle spellPlane, LineRenderer lineRenderer, bool transformToSpellPlane) {
        if (numPointsInSigil < 1) {
            Debug.LogError("Not enough points given to draw this spline!");
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.positionCount = numPointsInSigil;
        Vector3 point = sigil.GetPointLocalSpace(0f);

        if (transformToSpellPlane) {
            lineRenderer.SetPosition(0, TranslatePointToSpellPlane(point, spellPlane));
        } else {
            lineRenderer.SetPosition(0, point);
        }

        for (int i = 1; i < numPointsInSigil; i++) {
            float splineIndex = (float) i / (numPointsInSigil - 1);
            point = sigil.GetPointLocalSpace(splineIndex);

            if (transformToSpellPlane) {
                lineRenderer.SetPosition(i, TranslatePointToSpellPlane(point, spellPlane));
            } else {
                lineRenderer.SetPosition(i, point);
            }
        }
    }

    // --------------------------------------------------------------------------------
    // Sigil size
    // --------------------------------------------------------------------------------
    // Cheap hack to estimate the min/max x/y values:
    //  Query the spline at each control point for its x/y values. 
    //  Also query the halfway points between control points
    protected override void CalculateBoundingBox() {
        if (manuallySetBoundingBox) return;

        for (int cp = 0; cp < sigil.numControlPoints; cp++) {
            float sigilIndex = (float)cp / (float)(sigil.numControlPoints - 1);
            Vector3 point = sigil.GetPointLocalSpace(sigilIndex);

            // Reset
            if (cp == 0) {
                boundingBox[0] = point;
                boundingBox[1] = point;
            } else {
                UpdateBoundingBox(point);

                // Test the halfway point between this control point, and the previous one
                sigilIndex = ((float)cp - 0.5f) / (float)(sigil.numControlPoints - 1);
                point = sigil.GetPointLocalSpace(sigilIndex);

                UpdateBoundingBox(point);
            }
        }
    }
}
