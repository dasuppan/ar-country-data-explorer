using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MainManager : UnitySingleton<MainManager>
{
    [SerializeField] private CountryDefinitionSO pivotCountryDefinition;
    public List<CountryDefinitionSO> countryDefinitions = new();
    [SerializeField] public List<InfoCategoryDefinitionSO> infoCategoryDefinitions = new();

    private const float minSplineThickness = 0.005f;
    private const float maxSplineThickness = 0.05f;
    //private const float defaultSplineThickness = 0.01f;

    private readonly List<Country> countries = new();
    private readonly List<CountryRenderer> countryRenderers = new();
    private readonly List<SplineConnection> countryConnections = new();
    private readonly List<InfoCategory> activeInfoCategories = new();
    private readonly Dictionary<InfoCategory, double> infoCategoryMaxValues = new();

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
            infoCategoryMaxValues[def.category] = 0;

            string[] data = def.csvFile.text.Split(new[] { ";", "\n" }, StringSplitOptions.None);
            int tableRowCount = data.Length / colCount - 1;
            for (int i = 0; i < tableRowCount; i++)
            {
                string countryName = data[colCount * (i + 1) + countryNameIndex];
                double countryValue = double.Parse(data[colCount * (i + 1) + countryValueIndex]);
                infoCategoryMaxValues[def.category] = Math.Max(countryValue, infoCategoryMaxValues[def.category]);
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

    public Country GetCountryByReferenceImageName(string imgName)
    {
        return countries.FirstOrDefault(c => c.trackerTexture.name == imgName);
    }

    public void RegisterCountryRenderer(CountryRenderer countryRenderer)
    {
        countryRenderers.Add(countryRenderer);
        var country = countryRenderer.country;
        Debug.LogWarning($"Renderer for country {country.name} was added!");
        if (!country.isPivot && pivotCountryRenderer != null)
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

                if (!country.data.ContainsKey(iCat))
                {
                    Debug.LogWarning($"Country has no data for category {iCat}! Skipping spline connection...");
                    continue;
                }

                var countryConnectionGo =
                    new GameObject($"Country Connection - {iCatDef.categoryName} - {country.name}");
                countryConnectionGo.transform.SetParent(transform);

                var conn = countryConnectionGo.AddComponent<SplineConnection>();

                // Spline thickness calculation
                var relCategories = iCatDef.connectionThicknessRelativeTo.Intersect(activeInfoCategories);
                /*var relValues = countries.SelectMany( // For each country
                    c => c.data // Select all data arrays
                        .Where(c => relCategories.Contains(c.Key)) // that hold data for a relevant infoCat
                        .Select(pair => pair.Value) // and get the values from those data arrays
                ).ToList();*/
                var relMaxValues = infoCategoryMaxValues
                    .Where(pair => relCategories.Contains(pair.Key))
                    .Select(pair => pair.Value);
                var maxValue = relMaxValues.Max();
                var splineThickness = Mathf.Lerp(
                    minSplineThickness,
                    maxSplineThickness,
                    (float)(countryRenderer.country.data[iCat] / maxValue)
                );

                conn.Init(
                    iCatDef.type == CategoryType.TO_PIVOT
                        ? countryRenderer.transform
                        : pivotCountryRenderer.transform,
                    iCatDef.type == CategoryType.TO_PIVOT
                        ? pivotCountryRenderer.transform
                        : countryRenderer.transform,
                    iCatDef.splineMaterial,
                    splineThickness
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
        connectionsToRemove.ForEach(conn => Destroy(conn.gameObject));
    }

    void Update()
    {
    }
}