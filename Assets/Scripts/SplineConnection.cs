using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class SplineConnection : MonoBehaviour
{
    public Transform fromTransform;
    public Transform toTransform;
    public Material splineMaterial;
    /*{
        private get => meshRenderer.material;
        set => meshRenderer.material = value;
    }*/

    public float splineThickness = 0.03f;
    /*{
        private get => splineExtrude.Radius;
        set => splineExtrude.Radius = value;
    }*/

    private const float MidPointHeight = 0.02f;

    private SplineContainer splineContainer;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private SplineExtrude splineExtrude;
    
    private Spline connectingSpline;

    public void Init(Transform fromTransform, Transform toTransform, Material splineMaterial, float splineThickness)
    {
        if (initCompleted)
        {
            Debug.LogWarning("Already initialized. Aborting...");
            return;
        }
        
        this.toTransform = toTransform;
        this.fromTransform = fromTransform;
        
        splineContainer = gameObject.AddComponent<SplineContainer>();

        // Spline init
        connectingSpline = splineContainer.AddSpline();
        connectingSpline.AddRange(new[]
        {
            new BezierKnot(),
            new BezierKnot(),
            new BezierKnot()
        });
        
        this.splineMaterial = splineMaterial;
        this.splineThickness = splineThickness;

        // Rendering init
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = this.splineMaterial;
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = new Mesh();

        splineExtrude = gameObject.AddComponent<SplineExtrude>();
        splineExtrude.Radius = this.splineThickness;
        splineExtrude.Container = splineContainer;
        splineExtrude.RebuildOnSplineChange = true;
            
        initCompleted = true;
    }

    private bool initCompleted;

    private void Update()
    {
        if (!initCompleted) return;

        // We assume a three knotted spline
        var knot0 = connectingSpline.Knots.ToArray()[0];
        var knot1 = connectingSpline.Knots.ToArray()[1];
        var knot2 = connectingSpline.Knots.ToArray()[2];

        knot0.Position = transform.InverseTransformPoint(fromTransform.position);
        connectingSpline.SetKnot(0, knot0);
        knot1.Position =
            transform.InverseTransformPoint((fromTransform.position +
                                             (toTransform.position - fromTransform.position) / 2));
        knot1.Position.y = MidPointHeight; // TODO: Relative to both transforms?
        connectingSpline.SetKnot(1, knot1);
        knot2.Position = transform.InverseTransformPoint(toTransform.position);
        connectingSpline.SetKnot(2, knot2);
    }
}