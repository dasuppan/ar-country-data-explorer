using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MainManager : UnitySingleton<MainManager>
{
    [SerializeField] private CountryDefinitionSO pivotCountryDefinition;
    public List<CountryDefinitionSO> countryDefinitions = new();
    [SerializeField] public List<InfoCategoryDefinitionSO> infoCategoryDefinitions = new();

    private const float defaultSplineThickness = 0.01f;

    private readonly List<Country> countries = new();
    private readonly List<CountryRenderer> countryRenderers = new();
    private readonly List<SplineConnection> countryConnections = new();
    private readonly List<InfoCategory> activeInfoCategories = new();

    //private ARTrackedImageManager trackedImageManager;

    private CountryRenderer pivotCountryRenderer => countryRenderers.FirstOrDefault(c => c.country.isPivot);

    // CSV Constants
    // Structure: AT, Austria, Value
    private const int colCount = 3;
    private const int countryCodeIndex = 0;
    private const int countryNameIndex = 1;
    private const int countryValueIndex = 2;

    void Start()
    {
        // Instantiate countries
        countries.AddRange(
            countryDefinitions.Select(
                cDef => new Country(
                    cDef.countryName,
                    cDef.flagSprite,
                    cDef.trackerTexture,
                    cDef == pivotCountryDefinition
                )
            )
        );

        // Parse CSV data
        foreach (var def in infoCategoryDefinitions)
        {
            string[] data = def.csvFile.text.Split(new[] { ";", "\n" }, StringSplitOptions.None);
            int tableRowCount = data.Length / colCount - 1;
            for (int i = 0; i < tableRowCount; i++)
            {
                string countryName = data[colCount * (i + 1) + countryNameIndex];
                double countryValue = double.Parse(data[colCount * (i + 1) + countryValueIndex]);
                Country country = countries.FirstOrDefault(c => c.name == countryName);
                if (country == null) continue;
                country.data[def.category] = countryValue;
            }
        }

        // By default, activate all infoCategories
        activeInfoCategories.AddRange(
            infoCategoryDefinitions.Select(iCat => iCat.category)
        );

        Debug.LogWarning("MainManager initialized.");
    }

    // Tracked Image Manager Callbacks init

    /*protected override void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable() => trackedImageManager.trackedImagesChanged += OnImagesChanged;

    void OnDisable() => trackedImageManager.trackedImagesChanged -= OnImagesChanged;

    void OnImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var image in eventArgs.added)
        {
            var associatedCountry = GetCountryByReferenceImageName(image.referenceImage.name);
            Debug.LogWarning($"Tracker for {associatedCountry.name} was added.");
            // Handle added event
        }

        foreach (var image in eventArgs.updated)
        {
            var associatedCountry = GetCountryByReferenceImageName(image.referenceImage.name);
            Debug.LogWarning(
                $"Tracker for {associatedCountry.name} was updated. Tracking state: {image.trackingState}");
            if (image.trackingState == TrackingState.Limited)
            {
                Destroy(image.gameObject);
            }
            // Handle updated event
        }

        foreach (var image in eventArgs.removed)
        {
            var associatedCountry = GetCountryByReferenceImageName(image.referenceImage.name);
            Debug.LogWarning($"Tracker for {associatedCountry.name} was removed.");
            // Handle removed event
        }
    }*/

    public Country GetCountryByReferenceImageName(string imgName)
    {
        return countries.FirstOrDefault(c => c.trackerTexture.name == imgName);
    }

    public void RegisterCountryRenderer(CountryRenderer countryRenderer)
    {
        countryRenderers.Add(countryRenderer);
        Debug.LogWarning($"Renderer for country {countryRenderer.country.name} was added!");
        if (!countryRenderer.country.isPivot && pivotCountryRenderer != null)
        {
            // Draw spline connection(s)
            foreach (var iCat in activeInfoCategories)
            {
                var iCatDef = infoCategoryDefinitions.FirstOrDefault(def => def.category == iCat);
                if (iCatDef == null)
                {
                    Debug.LogError($"No info category definition found for category {iCat}!");
                    continue;
                }

                var conn = gameObject.AddComponent<SplineConnection>();
                conn.Init(
                    iCatDef.type == CategoryType.TO_PIVOT
                        ? countryRenderer.transform
                        : pivotCountryRenderer.transform,
                    iCatDef.type == CategoryType.TO_PIVOT
                        ? pivotCountryRenderer.transform
                        : countryRenderer.transform,
                    iCatDef.splineMaterial,
                    defaultSplineThickness
                );
                countryConnections.Add(conn);
            }
        }
    }

    public void DeregisterCountryRenderer(CountryRenderer countryRenderer)
    {
        countryRenderers.Remove(countryRenderer);
        Debug.LogWarning($"Renderer for country {countryRenderer.country.name} was removed!");

        var connectionsToRemove = countryConnections.Where(conn =>
            conn.fromTransform == countryRenderer.transform ||
            conn.toTransform == countryRenderer.transform
        ).ToList();
        connectionsToRemove.ForEach(conn => countryConnections.Remove(conn));
        connectionsToRemove.ForEach(Destroy);
    }

    void Update()
    {
    }
}