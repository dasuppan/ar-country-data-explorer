using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using Utils;

public class CountryConnection : MonoBehaviour
{
    private const float SplineMidPointYOffset = 0.1f;
    private const float DescriptionYOffset = 0.1f;
    private const float SiblingConnectionXOffset = 0.05f;
    private const int SplineFlowIndicatorCount = 5;
    private const float MinTimeToFinishSplineAnimationSeconds = 1f;
    private const float MaxTimeToFinishSplineAnimationSeconds = 5f;

    public CountryRenderer fromCountryRenderer;
    public CountryRenderer toCountryRenderer;
    public InfoCategory infoCategory;
    public Material splineMaterial;
    public GameObject splineFlowIndicatorPrefab;
    public double connectionValue { get; private set; }

    // Spline metas
    private int connectionPositionIndex = 0;
    private int siblingConnectionsCount = 0;
    private float splineAnimationSpeedSeconds = 1.0f;
    private bool splineMetasUpdated;

    private SplineContainer splineContainer;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private SplineExtrude splineExtrude;
    private Spline connectingSpline;
    private TextMeshPro descriptionMesh;

    private readonly List<MoveAlongSpline> splineAnimators = new();
    private Transform fromTransform => fromCountryRenderer == null ? null : fromCountryRenderer.transform;
    private Transform toTransform => toCountryRenderer == null ? null : toCountryRenderer.transform;

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

    private void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        splineExtrude = GetComponent<SplineExtrude>();
        descriptionMesh = GetComponentInChildren<TextMeshPro>();
    }

    public void UpdateSplineMetas(
        int connectionPositionIndex,
        int siblingConnectionsCount,
        double maxSiblingValue
    )
    {
        this.connectionPositionIndex = connectionPositionIndex;
        this.siblingConnectionsCount = siblingConnectionsCount;
        this.splineAnimationSpeedSeconds = Mathf.Lerp(
            MaxTimeToFinishSplineAnimationSeconds, MinTimeToFinishSplineAnimationSeconds,
            (float)(connectionValue / maxSiblingValue)
        );
        splineMetasUpdated = true;
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
        this.connectionValue = value;
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

        descriptionMesh.text = $"{value.ToFormattedString()}";
        descriptionMesh.color = this.splineMaterial.color;

        if (value > 0.1f)
        {
            for (int i = 0; i < SplineFlowIndicatorCount; i++)
            {
                var indicator = Instantiate(splineFlowIndicatorPrefab, transform);
                indicator.GetComponent<MeshRenderer>().material = splineMaterial;
                var animator = indicator.GetComponent<MoveAlongSpline>();
                animator.Init(
                    splineContainer,
                    Mathf.Lerp(0, 1, (float)i / SplineFlowIndicatorCount)
                );
                splineAnimators.Add(animator);
            }
        }

        initCompleted = true;
    }

    private bool initCompleted;

    private void Update()
    {
        if (!initCompleted) return;
        if (!splineMetasUpdated && !toTransform.hasChanged && !fromTransform.hasChanged) return;
        splineMetasUpdated = false;

        var splineStart = fromTransform.position;
        var splineEnd = toTransform.position;
        var splineVector = splineEnd - splineStart;

        var splineRightDirection = Vector3.Cross(splineVector, Vector3.up).normalized;
        var splineUpDirection = Vector3.Cross(splineRightDirection, splineVector).normalized;
        var knot1Pos = splineStart +
                        (splineVector / 2) +
                        (splineUpDirection * SplineMidPointYOffset) +
                        (splineRightDirection * (connectionPositionIndex * SiblingConnectionXOffset));


        // We assume a three knotted spline
        var knot0 = connectingSpline.Knots.ToArray()[0];
        var knot1 = connectingSpline.Knots.ToArray()[1];
        var knot2 = connectingSpline.Knots.ToArray()[2];
        
        knot0.Position = transform.InverseTransformPoint(fromTransform.position);
        connectingSpline.SetKnot(0, knot0);
        knot1.Position = transform.InverseTransformPoint(knot1Pos);
        connectingSpline.SetKnot(1, knot1);
        knot2.Position = transform.InverseTransformPoint(toTransform.position);
        connectingSpline.SetKnot(2, knot2);

        splineAnimators.ForEach(a => a.timeToTravel = splineAnimationSpeedSeconds);

        descriptionMesh.transform.position =
            transform.InverseTransformPoint(knot1Pos + Vector3.up * DescriptionYOffset);
    }
}
