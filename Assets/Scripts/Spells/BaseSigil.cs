using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSigil : MonoBehaviour {
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

    // --------------------------------------------------------------------------------
    // Sigil rendering
    // --------------------------------------------------------------------------------
    public abstract void DrawSigil(SpellCircle spellPlane, LineRenderer lineRenderer, bool transformToSpellPlane);

    public Vector3 TranslatePointToSpellPlane(Vector3 point, SpellCircle spellPlane) {
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

        // -- Center the scaled sigil
        if (applyTranslation) {
            point.x -= sigilWidth / 2 * scaleDelta;
            point.y -= sigilHeight / 2* scaleDelta;
        }

        return point;
    }

    // --------------------------------------------------------------------------------
    // Sigil size
    // --------------------------------------------------------------------------------
    protected abstract void CalculateBoundingBox();
    protected virtual void UpdateBoundingBox(Vector3 point) {
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
        lineRenderer.SetPosition(0, TranslatePointToSpellPlane(new Vector3(boundingBox[0].x, boundingBox[0].y, 0), spellPlane));
        lineRenderer.SetPosition(1, TranslatePointToSpellPlane(new Vector3(boundingBox[1].x, boundingBox[0].y, 0), spellPlane));
        lineRenderer.SetPosition(2, TranslatePointToSpellPlane(new Vector3(boundingBox[1].x, boundingBox[1].y, 0), spellPlane));
        lineRenderer.SetPosition(3, TranslatePointToSpellPlane(new Vector3(boundingBox[0].x, boundingBox[1].y, 0), spellPlane));
    }
}
