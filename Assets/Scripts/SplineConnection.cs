using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class SplineConnection : MonoBehaviour
{
    public Transform fromTransform;
    public Transform toTransform;
    public Material splineMaterial;
    public float thickness = 1.0f;
    
    private const float MidPointHeight = 0.2f;
    
    private SplineContainer splineContainer;
    private SplineExtrude splineExtrude;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Spline connectingSpline;
    

    private void Start()
    {
        splineContainer = gameObject.AddComponent<SplineContainer>();
        
        // Spline init
        connectingSpline = splineContainer.AddSpline();
        connectingSpline.AddRange(new[]
        {
            new BezierKnot(),
            new BezierKnot(),
            new BezierKnot()
        });
        
        // Rendering init
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = splineMaterial;
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = new Mesh();
        
        splineExtrude = gameObject.AddComponent<SplineExtrude>();
        splineExtrude.Container = splineContainer;
        splineExtrude.Radius = thickness;
        splineExtrude.RebuildOnSplineChange = true;
    }

    private void Update()
    {
        if (toTransform == null || fromTransform == null) return;
        splineExtrude.Radius = thickness;
        
        // We assume a three knotted spline
        var knot0 = connectingSpline.Knots.ToArray()[0];
        var knot1 = connectingSpline.Knots.ToArray()[1];
        var knot2 = connectingSpline.Knots.ToArray()[2];

        knot0.Position = transform.InverseTransformPoint(fromTransform.position);
        connectingSpline.SetKnot(0, knot0);
        knot1.Position =
            transform.InverseTransformPoint((fromTransform.position +
                                             (toTransform.position - fromTransform.position) / 2));
        knot1.Position.y = MidPointHeight;
        connectingSpline.SetKnot(1, knot1);
        knot2.Position = transform.InverseTransformPoint(toTransform.position);
        connectingSpline.SetKnot(2, knot2);
    }
}