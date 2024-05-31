using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Splines;

public class CountryConnection : MonoBehaviour
{
    public CountryRenderer fromCountryRenderer;
    public CountryRenderer toCountryRenderer;
    public InfoCategory infoCategory;

    public Material splineMaterial;

    //public float splineThickness = 0.03f;
    public GameObject splineFlowIndicatorPrefab;
    private int connectionPositionIndex = 0;
    private int siblingConnectionsCount = 0;

    /*public bool Concerns(Country country, InfoCategory infoCategory)
    {
        return toCountryRenderer.country == country && this.infoCategory == infoCategory;
    }*/

    public bool Concerns(
        CountryRenderer fromCountryRenderer,
        CountryRenderer toCountryRenderer,
        InfoCategory infoCategory
    )
    {
        return this.fromCountryRenderer == fromCountryRenderer &&
               this.toCountryRenderer == toCountryRenderer &&
               this.infoCategory == infoCategory;
    }

    private Transform fromTransform => fromCountryRenderer == null ? null : fromCountryRenderer.transform;
    private Transform toTransform => toCountryRenderer == null ? null : toCountryRenderer.transform;

    private const float SplineMidPointYOffset = 0.2f;
    private const float DescriptionYOffset = 0.1f;

    private const float SiblingConnectionXOffset = 0.05f;
    /*private const int SegmentsPerUnit = 24;
    public static float MinSplineThickness = 0.005f;
    public static float MaxSplineThickness = 0.05f;*/


    //private const float defaultSplineThickness = 0.01f;

    private SplineContainer splineContainer;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private SplineExtrude splineExtrude;
    private Spline connectingSpline;
    private TextMeshPro descriptionMesh;

    private readonly List<SplineAnimate> splineAnimators = new();

    private void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        splineExtrude = GetComponent<SplineExtrude>();
        descriptionMesh = GetComponentInChildren<TextMeshPro>();
    }

    public void UpdatePositionInfo(int connectionPositionIndex, int siblingConnectionsCount)
    {
        this.connectionPositionIndex = connectionPositionIndex;
        this.siblingConnectionsCount = siblingConnectionsCount;
    }

    public void Init(
        CountryRenderer fromCountryRenderer,
        CountryRenderer toCountryRenderer,
        InfoCategory infoCategory,
        Material splineMaterial,
        double value
    )
    {
        if (initCompleted)
        {
            Debug.LogWarning("Already initialized. Aborting...");
            return;
        }

        this.fromCountryRenderer = fromCountryRenderer;
        this.toCountryRenderer = toCountryRenderer;
        this.infoCategory = infoCategory;
        this.splineMaterial = splineMaterial;
        transform.localPosition = Vector3.zero;

        // Spline init
        splineContainer.Splines = new List<Spline>();
        connectingSpline = splineContainer.AddSpline();
        connectingSpline.AddRange(new[]
        {
            new BezierKnot(),
            new BezierKnot(),
            new BezierKnot()
        });
        connectingSpline.SetTangentMode(TangentMode.AutoSmooth);

        // Rendering init
        meshRenderer.material = this.splineMaterial;
        meshFilter.sharedMesh = new Mesh();

        splineExtrude.Container = splineContainer;
        splineExtrude.RebuildOnSplineChange = true;

        descriptionMesh.text = $"{value}";
        descriptionMesh.color = this.splineMaterial.color;

        // TODO: Calculate how many spline flow indicators we need
        var splineFlowIndicatorCount = 5;
        for (int i = 0; i < splineFlowIndicatorCount; i++)
        {
            var indicator = Instantiate(splineFlowIndicatorPrefab, transform);
            indicator.GetComponent<MeshRenderer>().material = splineMaterial;
            var animator = indicator.GetComponent<SplineAnimate>();
            animator.StartOffset = Mathf.Lerp(0, 1, (float)i / splineFlowIndicatorCount);
            animator.Container = splineContainer;
            animator.Play();
            splineAnimators.Add(animator);
        }

        //toCountryRenderer.AddIncomingConnection(this);

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
        var knot1Pos = (fromTransform.position +
                        (toTransform.position - fromTransform.position) / 2);
        knot1Pos += transform.right * (connectionPositionIndex * SiblingConnectionXOffset);
        knot1Pos.y = SplineMidPointYOffset; // TODO: Relative to both transforms?
        knot1.Position = transform.InverseTransformPoint(knot1Pos);
        connectingSpline.SetKnot(1, knot1);
        knot2.Position = transform.InverseTransformPoint(toTransform.position);
        connectingSpline.SetKnot(2, knot2);

        descriptionMesh.transform.position =
            transform.InverseTransformPoint(knot1Pos + Vector3.up * DescriptionYOffset);
        /*splineAnimators.ForEach(sA => sA.gameObject.SetActive(true));
        splineAnimators.ForEach(sA => sA.enabled = true);
        splineAnimators.ForEach(sA => sA.Play());
        Debug.LogWarning("IsPlaying");
        Debug.LogWarning(splineAnimators.Select(fI => fI.GetComponent<SplineAnimate>().IsPlaying).ToList().Stringify().ToString());
        Debug.LogWarning("Active");
        Debug.LogWarning(splineAnimators.Select(fI => fI.GetComponent<SplineAnimate>().isActiveAndEnabled).ToList().Stringify().ToString());*/
    }

    public void RemoveSelf()
    {
        /*if (toCountryRenderer != null) // GameObject might already be destroyed
        {
            toCountryRenderer.RemoveIncomingConnection(this);
        }*/

        Destroy(gameObject);
    }
}