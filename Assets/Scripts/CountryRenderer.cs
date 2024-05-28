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

    private readonly Dictionary<InfoCategory, SplineConnection> incomingConnections = new();
    private readonly Dictionary<InfoCategory, SplineConnection> outgoingConnections = new();

    public void RemoveIncomingConnection(SplineConnection connection)
    {
        incomingConnections.RemoveByValue(connection);
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

    public void OnCountryRenderersChanged(CountryRenderersChangedEventArgs eventArgs)
    {
        foreach (var renderer in eventArgs.added)
        {
        }

        foreach (var renderer in eventArgs.removed)
        {
        }
    }

    public void OnActiveInfoCategoriesChanged(ActiveInfoCategoriesChangedEventArgs eventArgs)
    {
        foreach (var infoCategory in eventArgs.added)
        {
        }

        foreach (var infoCategory in eventArgs.removed)
        {
        }
    }

    public void OnCountryRendererAdded(CountryRenderer countryRenderer)
    {
        foreach (var iCat in MainManager.Instance.activeInfoCategories)
        {
            if (connections.ContainsKey(iCat)) continue;
            // SplineConnection does not yet exist, create it
            var iCatDef = MainManager.Instance.GetInfoCategoryDefinition(iCat);
            if (iCatDef == null)
            {
                Debug.LogError($"No info category definition found for category {iCat}!");
                continue;
            }

            if (!DataCountry.data.ContainsKey(iCat))
            {
                Debug.LogWarning($"Country ${DataCountry.name} does not provide data for category {iCat}! Skipping...");
                continue;
            }

            var countryConnectionGo =
                new GameObject($"Country Connection - {iCatDef.categoryName} - {DataCountry.name}");
            countryConnectionGo.transform.SetParent(transform);
            var conn = countryConnectionGo.AddComponent<SplineConnection>();

            // Spline thickness calculation
            var iCatMaxValue = MainManager.Instance.GetMaxValueForInfoCategory(iCat);
            var splineThickness = Mathf.Lerp(
                SplineConnection.MinSplineThickness,
                SplineConnection.MaxSplineThickness,
                (float)(DataCountry.data[iCat] / iCatMaxValue)
            );

            conn.Init(
                iCatDef.type == CategoryType.TO_PIVOT
                    ? DataCountryRenderer.transform
                    : PivotCountryRenderer.transform,
                iCatDef.type == CategoryType.TO_PIVOT
                    ? PivotCountryRenderer.transform
                    : DataCountryRenderer.transform,
                iCatDef.splineMaterial,
                splineThickness
            );

            connections.Add(iCat, conn);
        }
    }

    [NonSerialized] public bool Dirty;

    public void OnCountryRendererRemoved(CountryRenderer countryRenderer)
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Scoobidoobidoo.");
        transform.LookAt(Camera.main.transform, Vector3.up);
        // TODO: Continue here: trackedImage.trackingState
        if (Dirty)
        {
            var activeInfoCategories = MainManager.Instance.activeInfoCategories;
            var activeCountryRenderers = MainManager.Instance.countryRenderers;

            // Check if connection removal needed based on removed infoCategories
            var removedInfoCategories = outgoingConnections.Keys.Except(activeInfoCategories);
            var toRemoveOutgoingConnections1 = removedInfoCategories.Select(iCat => outgoingConnections[iCat]);
            /*var toRemoveOutgoingConnections1 = outgoingConnections
                .Where(conn => !activeInfoCategories.Contains(conn.Key))
                .Select(pair => pair.Value);*/

            // Check if connection removal needed based on removed renderers
            var removedCountryRenderers = outgoingConnections.Values
                .Select(cRend => cRend.targetCountryRenderer)
                .Except(activeCountryRenderers)
                .ToList();
            var toRemoveOutgoingConnections2 = outgoingConnections
                .Where(pair => removedCountryRenderers.Contains(pair.Value.targetCountryRenderer))
                .Select(pair => pair.Value);
            
            /*var toRemoveOutgoingConnections2 = outgoingConnections
                .Where(conn => !activeCountryRenderers.Contains(conn.Value.targetCountryRenderer))
                .Select(pair => pair.Value);*/

            // Notify connections of their removal
            var toRemoveOutgoingConnections = toRemoveOutgoingConnections1.Union(toRemoveOutgoingConnections2).ToList();
            toRemoveOutgoingConnections.ForEach(conn => conn.RemoveSelf());

            // Remove connections
            toRemoveOutgoingConnections.ForEach(conn => outgoingConnections.RemoveByValue(conn));

            // Check if connection adding is needed based on added infoCategories
            var currentOutgoingConnectionCategories = outgoingConnections.Select(conn => conn.Key).ToList();
            var toAddInfoCategoryConnections = activeInfoCategories.Except(currentOutgoingConnectionCategories);

            // Check if connection adding is needed based on added renderers
            // TODO: So help me jesus christ the saviour, continue here!
        }
    }
}