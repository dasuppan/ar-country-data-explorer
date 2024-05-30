using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    private const float MidPointHeight = 0.2f;
    /*private const int SegmentsPerUnit = 24;
    public static float MinSplineThickness = 0.005f;
    public static float MaxSplineThickness = 0.05f;*/


    //private const float defaultSplineThickness = 0.01f;

    private SplineContainer splineContainer;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private SplineExtrude splineExtrude;
    private Spline connectingSpline;

    private readonly List<GameObject> splineFlowIndicators = new();

    private void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        splineExtrude = GetComponent<SplineExtrude>();
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
        float splineThickness = 0.015f
    )
    {
        transform.localPosition = Vector3.zero;

        if (initCompleted)
        {
            Debug.LogWarning("Already initialized. Aborting...");
            return;
        }

        this.fromCountryRenderer = fromCountryRenderer;
        this.toCountryRenderer = toCountryRenderer;
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
        //this.splineThickness = splineThickness;

        // Rendering init
        meshRenderer.material = this.splineMaterial;
        meshFilter.sharedMesh = new Mesh();

        splineExtrude.Container = splineContainer;
        splineExtrude.RebuildOnSplineChange = true;

        // TODO: Calculate how many spline flow indicators we need
        var splineFlowIndicatorCount = 5;
        for (int i = 0; i < splineFlowIndicatorCount; i++)
        {
            var indicator = Instantiate(splineFlowIndicatorPrefab, transform);
            indicator.GetComponent<MeshRenderer>().material = splineMaterial;
            var animator = indicator.GetComponent<SplineAnimate>();
            animator.StartOffset = Mathf.Lerp(0, 1, (float) i / splineFlowIndicatorCount);
            animator.Container = splineContainer;
            animator.Play();
            splineFlowIndicators.Add(indicator);
        }

        //toCountryRenderer.AddIncomingConnection(this);

        initCompleted = true;
    }

    private bool initCompleted;

    private const float SiblingConnectionOffset = 0.05f;

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
        if (siblingConnectionsCount > 0)
        {
            knot1Pos += transform.right * (connectionPositionIndex * SiblingConnectionOffset);
        }

        knot1.Position =
            transform.InverseTransformPoint(knot1Pos);
        knot1.Position.y = MidPointHeight; // TODO: Relative to both transforms?
        connectingSpline.SetKnot(1, knot1);
        knot2.Position = transform.InverseTransformPoint(toTransform.position);
        connectingSpline.SetKnot(2, knot2);
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