using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSigil : BaseSigil {
    [Tooltip("A set of points that approximate the shape of a sigil, in local space (-inf, inf)")]
    public List<Vector3> points;

    private LineRenderer lineRenderer;
    public SpellCircle spellPlane;

    private void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        CalculateBoundingBox();
    }

    public override void DrawSigil(SpellCircle spellPlane, LineRenderer lineRenderer) {
        if (points.Count < 2) return;

        lineRenderer.positionCount = points.Count;
        for(int i = 0; i < lineRenderer.positionCount; i++) {
            lineRenderer.SetPosition(i, TranslatePointToSpellPlane(points[i], spellPlane));
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
    public void AddPointInWorldSpace(Vector3 pointInWorldSpace, SpellCircle spellPlane) {
        // Project the point onto the spell plane
        // For some reason, I need to use spellPlane.transform.forward as the up dir. Not sure why.
        Vector3 newPoint = Vector3.ProjectOnPlane(pointInWorldSpace, spellPlane.transform.forward);

        // Translate to local space, (-inf, inf)
        newPoint -= spellPlane.transform.position;

        // Undo any rotation on the spell plane
        newPoint = Quaternion.Inverse(spellPlane.transform.rotation) * newPoint;

        points.Add(newPoint);

        // -- Update the bounding box
        if (points.Count == 1) {
            boundingBox[0] = newPoint;
            boundingBox[1] = newPoint;
        } else {
            UpdateBoundingBox(newPoint);
        }
    }

    private void Update() {
        CalculateBoundingBox();
        DrawSigil(spellPlane, lineRenderer);
    }
}
