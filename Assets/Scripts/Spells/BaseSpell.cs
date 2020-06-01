using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BezierSpline))]
public class BaseSpell : MonoBehaviour {
    //public SpellCircle spellPlane;
    private BezierSpline sigil;

    public bool manuallySetBoundingBox;
    public Vector3[] boundingBox = new Vector3[] {Vector3.zero, Vector3.zero};
    public float sigilWidth {
        get {
            return Mathf.Abs(boundingBox[1].x - boundingBox[0].x);
        }
    }
    public float sigilHeight {
        get {
            return Mathf.Abs(boundingBox[1].y - boundingBox[0].y);
        }
    }

    [Header("DEBUG")]
    public bool applyScaling;
    public bool applyTranslation;

    // Start is called before the first frame update
    void Start() {
        sigil = GetComponent<BezierSpline>();
        EstimateBoundingBox();
    }

    // --------------------------------------------------------------------------------
    // Sigil rendering
    // --------------------------------------------------------------------------------
    // TODO: Translate this to worldspace
    public void DrawSigil(SpellCircle spellPlane, LineRenderer lineRenderer, int numPoints) {
        lineRenderer.enabled = true;
        if (numPoints < 1) {
            Debug.LogError("Not enough points given to draw this spline!");
            return;
        }

        lineRenderer.positionCount = numPoints;
        //lineRenderer.SetPosition(0, LocalPointToSpellPlane(sigil.GetPoint(0f)));
        lineRenderer.SetPosition(0, translatePointToSpellPlane(sigil.GetPointLocalSpace(0f), spellPlane));

        for (int i = 1; i < numPoints; i++) {
            float splineIndex = (float) i / (numPoints - 1);
            //lineRenderer.SetPosition(i, LocalPointToSpellPlane(sigil.GetPoint(splineIndex)));
            lineRenderer.SetPosition(i, translatePointToSpellPlane(sigil.GetPointLocalSpace(splineIndex), spellPlane));
        }

    }

    //private Vector3 LocalPointToSpellPlane(Vector3 point) {
    //    // TODO: Center the point
    //    // Apply the spell plane's rotation and position offsets
    //    point = spellPlane.transform.rotation * point;
    //    point += spellPlane.transform.position;

    //    // Apply manual offset
    //    point += spellPlane.manualSigilPositionOffset;

    //    return point;
    //}

    private Vector3 translatePointToSpellPlane(Vector3 point, SpellCircle spellPlane) {
        // -- Scale the sigil to match the size of the spell plane
        // We scale x/y by the same amount, to avoid spline distortion
        //  Scale both so that they fit in the spell plane
        float scaleDelta = 1f;
        if (applyScaling) {
            if (sigilWidth > sigilHeight) {
                scaleDelta = spellPlane.diameter / sigilWidth;
            } else {
                scaleDelta = spellPlane.diameter / sigilHeight;
            }

            // Shift from (-inf, inf) to (0, inf)
            point.x -= boundingBox[0].x;
            point.y -= boundingBox[0].y;

            // Apply scaling
            point *= scaleDelta;
        }

        if (applyTranslation) {
            point.x -= sigilWidth / 2 * scaleDelta;
            point.y -= sigilHeight / 2* scaleDelta;
        }

        // -- Center the sigil
        return point;
    }

    // --------------------------------------------------------------------------------
    // Sigil size
    // --------------------------------------------------------------------------------
    // Cheap hack to estimate the min/max x/y values:
    //  Query the spline at each control point for its x/y values. 
    //  Also query the halfway points between control points
    private void EstimateBoundingBox() {
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

    private void UpdateBoundingBox(Vector3 point) {
        // Min x/y
        if (boundingBox[0].x > point.x) {
            boundingBox[0].x = point.x;
        }
        if (boundingBox[0].y > point.y) {
            boundingBox[0].y = point.y;
        }

        // Max x/y
        if (boundingBox[1].x < point.x) {
            boundingBox[1].x = point.x;
        }
        if (boundingBox[1].y < point.y) {
            boundingBox[1].y = point.y;
        }
    }

    public void DrawBoundingBox(SpellCircle spellPlane, LineRenderer lineRenderer) {
        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(0, translatePointToSpellPlane(new Vector3(boundingBox[0].x, boundingBox[0].y, 0), spellPlane));
        lineRenderer.SetPosition(1, translatePointToSpellPlane(new Vector3(boundingBox[1].x, boundingBox[0].y, 0), spellPlane));
        lineRenderer.SetPosition(2, translatePointToSpellPlane(new Vector3(boundingBox[1].x, boundingBox[1].y, 0), spellPlane));
        lineRenderer.SetPosition(3, translatePointToSpellPlane(new Vector3(boundingBox[0].x, boundingBox[1].y, 0), spellPlane));
    }
}
