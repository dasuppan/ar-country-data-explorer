using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class CountryConnection : MonoBehaviour
{
    public CountryRenderer targetCountryRenderer;
    public InfoCategory infoCategory;

    public bool Concerns(Country country, InfoCategory infoCategory)
    {
        return targetCountryRenderer.country == country && this.infoCategory == infoCategory;
    }
    
    public bool Concerns(CountryRenderer countryRenderer, InfoCategory infoCategory)
    {
        return countryRenderer == targetCountryRenderer && this.infoCategory == infoCategory;
    }
    
    private Transform targetTransform => targetCountryRenderer == null ? null : targetCountryRenderer.transform;
    public Material splineMaterial;
    public float splineThickness = 0.03f;

    private const float MidPointHeight = 0.2f;
    private const int SegmentsPerUnit = 24;
    public static float MinSplineThickness = 0.005f;
    public static float MaxSplineThickness = 0.05f;

    
    //private const float defaultSplineThickness = 0.01f;

    private SplineContainer splineContainer;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private SplineExtrude splineExtrude;
    private Spline connectingSpline;

    private void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        splineExtrude = GetComponent<SplineExtrude>();
    }

    public void Init(CountryRenderer targetCountryRenderer, InfoCategory infoCategory, Material splineMaterial, float splineThickness)
    {
        transform.localPosition = Vector3.zero;
        if (initCompleted)
        {
            Debug.LogWarning("Already initialized. Aborting...");
            return;
        }

        this.targetCountryRenderer = targetCountryRenderer;
        this.infoCategory = infoCategory;

        //splineContainer = gameObject.AddComponent<SplineContainer>();
        splineContainer.Splines = new List<Spline>();

        // Spline init
        connectingSpline = splineContainer.AddSpline();
        connectingSpline.AddRange(new[]
        {
            new BezierKnot(),
            new BezierKnot(),
            new BezierKnot()
        });
        connectingSpline.SetTangentMode(TangentMode.AutoSmooth);

        this.splineMaterial = splineMaterial;
        this.splineThickness = splineThickness;

        // Rendering init
        //meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = this.splineMaterial;
        //meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = new Mesh();

        /*splineExtrude = gameObject.AddComponent<SplineExtrude>();
        splineExtrude.Radius = this.splineThickness;
        splineExtrude.SegmentsPerUnit = SegmentsPerUnit;*/
        splineExtrude.Container = splineContainer;
        splineExtrude.RebuildOnSplineChange = true;

        targetCountryRenderer.AddIncomingConnection(this);

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

        knot0.Position = transform.InverseTransformPoint(transform.position); // TODO: Redundant
        connectingSpline.SetKnot(0, knot0);
        knot1.Position =
            transform.InverseTransformPoint((transform.position +
                                             (targetTransform.position - transform.position) / 2));
        knot1.Position.y = MidPointHeight; // TODO: Relative to both transforms?
        connectingSpline.SetKnot(1, knot1);
        knot2.Position = transform.InverseTransformPoint(targetTransform.position);
        connectingSpline.SetKnot(2, knot2);
    }

    public void RemoveSelf()
    {
        if (targetCountryRenderer != null) // GameObject might already be destroyed
        {
            targetCountryRenderer.RemoveIncomingConnection(this);
        }
        Destroy(gameObject);
    }
}