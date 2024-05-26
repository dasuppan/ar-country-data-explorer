using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public enum InfoCategory
{
    IMPORT,
    EXPORT
}

public enum CategoryType
{
    INGOING,
    OUTGOING
}

public class Country2
{
    public Country2(string name, Sprite flagSprite, Texture2D trackerTexture, bool isPivot = false)
    {
        this.name = name;
        this.flagSprite = flagSprite;
        this.trackerTexture = trackerTexture;
        this.isPivot = isPivot;
    }

    public readonly string name;
    public readonly Sprite flagSprite;
    public readonly Texture2D trackerTexture;
    public readonly bool isPivot;
    public readonly Dictionary<InfoCategory, double?> data = new();
};

public class MainManager : UnitySingleton<MainManager>
{
    [SerializeField] private CountryDefinitionSO pivotCountryDefinition;
    public List<CountryDefinitionSO> countryDefinitions = new();

    // Structure: AT, Austria, Value
    private const int colCount = 3;
    private const int countryCodeIndex = 0;
    private const int countryNameIndex = 1;
    private const int countryValueIndex = 2;
    [SerializeField] public InfoCategoryTextAssetDictionary countryInfo = new();

    [SerializeField] private Material defaultSplineMaterial;
    private const float defaultSplineThickness = 0.01f;
    
    private readonly List<Country2> countries = new();
    private readonly List<CountryRenderer> instantiatedCountries = new();

    private CountryRenderer pivotCountryRenderer => instantiatedCountries.FirstOrDefault(c => c.country.isPivot);

    void Start()
    {
        // Instantiate countries
        countries.AddRange(
            countryDefinitions.Select(
                cDef => new Country2(
                    cDef.countryName,
                    cDef.flagSprite,
                    cDef.trackerTexture,
                    cDef == pivotCountryDefinition
                )
            )
        );

        // Parse CSV data
        foreach (var (category, file) in countryInfo)
        {
            string[] data = file.text.Split(new[] { ";", "\n" }, StringSplitOptions.None);
            int tableRowCount = data.Length / colCount - 1;
            for (int i = 0; i < tableRowCount; i++)
            {
                string countryName = data[colCount * (i + 1) + countryNameIndex];
                double countryValue = double.Parse(data[colCount * (i + 1) + countryValueIndex]);
                Country2 country = countries.FirstOrDefault(c => c.name == countryName);
                if (country == null) continue;
                country.data[category] = countryValue;
            }
        }

        Debug.LogWarning("Countries initialized.");
    }

    public Country2 GetCountryByReferenceImageName(string imgName)
    {
        return countries.FirstOrDefault(c => c.trackerTexture.name == imgName);
    }

    private readonly List<SplineConnection> countryConnections = new();

    public void RegisterCountryRenderer(CountryRenderer countryRenderer)
    {
        instantiatedCountries.Add(countryRenderer);
        Debug.LogWarning($"Renderer for country {countryRenderer.country.name} was added!");
        if (!countryRenderer.country.isPivot && pivotCountryRenderer != null)
        {
            // Draw spline connection
            var countryConnection = gameObject.AddComponent<SplineConnection>();
            countryConnection.fromTransform = countryRenderer.transform;
            countryConnection.toTransform = pivotCountryRenderer.transform;
            countryConnection.thickness = defaultSplineThickness;
            countryConnection.splineMaterial = defaultSplineMaterial;
            countryConnections.Add(countryConnection);
        }
    }

    public void DeregisterCountryRenderer(CountryRenderer countryRenderer)
    {
        instantiatedCountries.Remove(countryRenderer);
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