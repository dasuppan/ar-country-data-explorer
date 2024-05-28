using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;
using Utils;

public class CountryRenderer : MonoBehaviour
{
    private ARTrackedImage trackedImage;
    public Country country { get; private set; }

    public bool dirty
    {
        get => Dirty;
        set => Dirty = value;
    }

    private readonly List<SplineConnection> incomingConnections = new();
    private readonly List<SplineConnection> outgoingConnections = new();
    /*private readonly Dictionary<InfoCategory, SplineConnection> incomingConnections = new();
    private readonly Dictionary<InfoCategory, SplineConnection> outgoingConnections = new();*/

    public bool RemoveIncomingConnection(SplineConnection connection)
    {
        return incomingConnections.Remove(connection);
    }

    void Start()
    {
        trackedImage = GetComponent<ARTrackedImage>();
        country = MainManager.Instance.GetCountryByReferenceImageName(
            trackedImage.referenceImage.name
        );
        if (country != null)
        {
            GetComponent<SpriteRenderer>().sprite = country.flagSprite;
            Debug.LogWarning($"Adding country {country.name}");
            Debug.LogWarning($"Is country pivot? {country.isPivot}");
            Debug.LogWarning($"Country {country.name} has {country.data.Count} data points.");
        }
        else
        {
            // TODO: Should never happen actually
            Debug.LogWarning("Tracker is not a country.");
        }

        /*MainManager.Instance.RegisterCountryRenderer(this);*/
    }

    /*private void OnDestroy()
    {
        MainManager.Instance.DeregisterCountryRenderer(this);
    }*/

    [NonSerialized] public bool Dirty;

    public void OnCountryRendererRemoved(CountryRenderer countryRenderer)
    {
    }

    private List<InfoCategory> cachedInfoCategories; // TODO: Need initial fill
    private List<CountryRenderer> cachedCountryRenderers; // TODO: Need initial fill

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Scoobidoobidoo.");
        transform.LookAt(Camera.main.transform, Vector3.up);
        // TODO: Continue here: trackedImage.trackingState
        if (Dirty)
        {
            var currentInfoCategories = MainManager.Instance.activeInfoCategories;
            var currentCountryRenderers = MainManager.Instance.countryRenderers;

            var removedInfoCategories = cachedInfoCategories.Except(currentInfoCategories);
            var removedCountryRenderers = cachedCountryRenderers.Except(currentCountryRenderers);
            /*var addedInfoCategories = currentInfoCategories.Except(cachedInfoCategories);
            var addedCountryRenderers = currentCountryRenderers.Except(cachedCountryRenderers);*/

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

            // CONNECTION EXISTENCE CHECK & ADDING

            foreach (var cRenderer in currentCountryRenderers)
            {
                foreach (var iCat in currentInfoCategories)
                {
                    if (outgoingConnections.Exists(conn => conn.Concerns(cRenderer, iCat))) continue;

                    var catData = country.GetDataForCountryInfoCategory(cRenderer.country, iCat);
                    if (catData == null) continue;

                    outgoingConnections.Add(CreateNewOutgoingConnection(iCat, cRenderer, (double)catData));
                }
            }
        }
    }

    private SplineConnection CreateNewOutgoingConnection(InfoCategory iCat, CountryRenderer cRenderer, double value)
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

        var countryConnectionGo =
            new GameObject($"{iCatDef.categoryName} - {cRenderer.country.name}");
        countryConnectionGo.transform.SetParent(transform);
        var conn = countryConnectionGo.AddComponent<SplineConnection>();

        // Spline thickness calculation
        var iCatMaxValue = MainManager.Instance.GetMaxValueForInfoCategory(iCat);
        var splineThickness = Mathf.Lerp(
            SplineConnection.MinSplineThickness,
            SplineConnection.MaxSplineThickness,
            (float)(value / iCatMaxValue)
        );
        
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