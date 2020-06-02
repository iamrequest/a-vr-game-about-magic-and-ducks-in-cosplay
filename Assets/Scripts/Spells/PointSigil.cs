using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSigil : BaseSigil {
    [Tooltip("A set of points that approximate the shape of a sigil, in local space (-inf, inf)")]
    public List<Vector3> points;

    private void Start() {
        CalculateBoundingBox();
    }

    // When transformSigilToSpellPlane is true, the sigil will be scaled and centered
    //  When scaled, it'll be scaled uniformly towards spellPlane.diameter
    public override void DrawSigil(SpellCircle spellPlane, LineRenderer lineRenderer, bool transformSigil) {
        if (points.Count < 2) return;

        lineRenderer.enabled = true;

        lineRenderer.positionCount = points.Count;
        for(int i = 0; i < lineRenderer.positionCount; i++) {
            if (transformSigil) {
                lineRenderer.SetPosition(i, TranslatePointToSpellPlane(points[i], spellPlane));
            } else {
                lineRenderer.SetPosition(i, points[i]);
            }
        }

        // TODO: Snap point to hand if currently drawing
    }

    protected override void CalculateBoundingBox() {
        if (points.Count < 1) return;

        // Reset
        boundingBox[0] = points[0];
        boundingBox[1] = points[0];

        foreach (Vector3 point in points) {
            UpdateBoundingBox(point);
        }
    }

    // --------------------------------------------------------------------------------
    // Point Manipulation
    // --------------------------------------------------------------------------------
    public void AddPointRaw(Vector3 point, SpellCircle spellPlane) {
        points.Add(point);

        // -- Update the bounding box
        if (points.Count == 1) {
            boundingBox[0] = point;
            boundingBox[1] = point;
        } else {
            UpdateBoundingBox(point);
        }
    }
    public void AddPointInWorldSpace(Vector3 pointInWorldSpace, SpellCircle spellPlane) {
        // Project the point onto the spell plane
        // For some reason, I need to use spellPlane.transform.forward as the up dir. Not sure why.
        Vector3 newPoint = Vector3.ProjectOnPlane(pointInWorldSpace, spellPlane.transform.forward);

        // Translate to local space, (-inf, inf, z)
        newPoint -= spellPlane.transform.position;

        // Undo any rotation on the spell plane
        newPoint = Quaternion.Inverse(spellPlane.transform.rotation) * newPoint;

        // Throw away the z value - having this populated causes issues when we start to rotate
        newPoint.z = 0f;

        points.Add(newPoint);

        // -- Update the bounding box
        if (points.Count == 1) {
            boundingBox[0] = newPoint;
            boundingBox[1] = newPoint;
        } else {
            UpdateBoundingBox(newPoint);
        }
    }
}
