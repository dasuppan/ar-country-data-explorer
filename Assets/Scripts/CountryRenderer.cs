using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CountryRenderer : MonoBehaviour
{
    //private ARTrackedImage trackedImage;
    public Country country { get; private set; }

    [SerializeField] private GameObject countryConnectionRendererPrefab;
    [SerializeField] private CountryDefinitionSO predefinedCountry;
    private SpriteRenderer spriteRenderer;

    public bool dirty
    {
        get => Dirty;
        set => Dirty = value;
    }

    private readonly List<CountryConnection> incomingConnections = new();

    private readonly List<CountryConnection> outgoingConnections = new();
    /*private readonly Dictionary<InfoCategory, SplineConnection> incomingConnections = new();
    private readonly Dictionary<InfoCategory, SplineConnection> outgoingConnections = new();*/

    public bool RemoveIncomingConnection(CountryConnection connection)
    {
        return incomingConnections.Remove(connection);
    }

    public void AddIncomingConnection(CountryConnection connection)
    {
        incomingConnections.Add(connection);
    }

    void Start()
    {
        if (predefinedCountry != null)
        {
            Debug.LogWarning(
                "$Country Renderer was instantiated with predefined country, skipping reference image comparison...");
            country = MainManager.Instance.GetCountryByName(predefinedCountry.name);
        }
        else
        {
            var trackedImage = GetComponent<ARTrackedImage>();
            country = MainManager.Instance.GetCountryByReferenceImageName(
                trackedImage.referenceImage.name
            );
        }

        if (country != null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sprite = country.flagSprite;
            Debug.LogWarning($"Adding country {country.countryName}");
            Debug.LogWarning($"Country {country.countryName} has {country.data.Count} data points.");
            MainManager.Instance.RegisterCountryRenderer(this);
        }
        else
        {
            // TODO: Should never happen actually
            Debug.LogWarning("No country found for tracker. Renderer is idling...");
        }
    }

    /*private void OnDestroy()
    {
        MainManager.Instance.DeregisterCountryRenderer(this);
    }*/

    [NonSerialized] private bool Dirty;

    public void ForceEvaluationOnNextFrame()
    {
        Dirty = true;
    }

    private readonly List<InfoCategory> cachedInfoCategories = new();
    private readonly List<CountryRenderer> cachedCountryRenderers = new();

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.transform.LookAt(Camera.main.transform, Vector3.up);
        // TODO: Respect trackedImage.trackingState?
        if (Dirty)
        {
            var currentInfoCategories = MainManager.Instance.activeInfoCategories;
            var currentCountryRenderers = MainManager.Instance.countryRenderers;

            var removedInfoCategories = cachedInfoCategories.Except(currentInfoCategories);
            var removedCountryRenderers = cachedCountryRenderers.Except(currentCountryRenderers);

            // CONNECTION REMOVAL
            // Check if connection removal needed based on removed renderers
            var toRemoveOutgoingConnections1 = outgoingConnections.Where(conn =>
                removedCountryRenderers.Contains(conn.targetCountryRenderer)
            );
            // Check if connection removal needed based on removed infoCategories
            var toRemoveOutgoingConnections2 = outgoingConnections.Where(conn =>
                removedInfoCategories.Contains(conn.infoCategory)
            );

            var toRemoveOutgoingConnections = toRemoveOutgoingConnections1.Union(toRemoveOutgoingConnections2).ToList();
            // Notify connections of their removal
            toRemoveOutgoingConnections.ForEach(conn => conn.RemoveSelf());
            // Remove connections from lists
            toRemoveOutgoingConnections.ForEach(conn => outgoingConnections.Remove(conn));
            var removedConnectionsCount = toRemoveOutgoingConnections.Count;

            // CONNECTION EXISTENCE CHECK & ADDING

            var addedConnectionsCount = 0;
            foreach (var cRenderer in currentCountryRenderers)
            {
                foreach (var iCat in currentInfoCategories)
                {
                    if (outgoingConnections.Exists(conn => conn.Concerns(cRenderer, iCat))) continue;

                    var catData = country.GetDataForCountryInfoCategory(cRenderer.country, iCat);
                    if (catData == null) continue;

                    outgoingConnections.Add(CreateNewOutgoingConnection(iCat, cRenderer, (double)catData));
                    addedConnectionsCount++;
                }
            }

            cachedInfoCategories.Clear();
            cachedInfoCategories.AddRange(currentInfoCategories);
            cachedCountryRenderers.Clear();
            cachedCountryRenderers.AddRange(currentCountryRenderers);

            Debug.LogWarning(
                $"Renderer Evaluation completed ({country.countryName}):" +
                $"\nConnections added: {addedConnectionsCount}" +
                $"\nConnections removed: {removedConnectionsCount}"
            );
            Dirty = false;
        }
    }

    private CountryConnection CreateNewOutgoingConnection(InfoCategory iCat, CountryRenderer cRenderer, double value)
    {
        if (outgoingConnections.Exists(conn => conn.Concerns(cRenderer, iCat)))
        {
            Debug.LogError(
                $"There exists a connection that deals with the combination {cRenderer.country}/{iCat}! Aborting...");
            return null;
        }

        var iCatDef = MainManager.Instance.GetInfoCategoryDefinition(iCat);
        if (iCatDef == null)
        {
            Debug.LogError($"No info category definition found for category {iCat}! Aborting...");
            return null;
        }

        var countryConnectionGo = Instantiate(countryConnectionRendererPrefab, transform);
        countryConnectionGo.name = $"{iCatDef.categoryName} - {cRenderer.country.countryName}";
        var conn = countryConnectionGo.GetComponent<CountryConnection>();

        // Spline thickness calculation
        /*var iCatMaxValue = MainManager.Instance.GetMaxValueForInfoCategory(iCat);
        var splineThickness = Mathf.Lerp(
            SplineConnection.MinSplineThickness,
            SplineConnection.MaxSplineThickness,
            (float)(value / iCatMaxValue)
        );*/
        var splineThickness = 0.025f;

        // TODO: Respect incoming connections in spline curvature

        conn.Init(
            cRenderer,
            iCat,
            iCatDef.splineMaterial,
            splineThickness
        );

        return conn;
    }
}