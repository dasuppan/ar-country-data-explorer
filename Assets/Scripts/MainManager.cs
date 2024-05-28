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
    [SerializeField] private List<CountryDefinitionSO> countryDefinitions = new();
    [SerializeField] private List<InfoCategoryDefinitionSO> infoCategoryDefinitions = new();
    [SerializeField] public List<InfoCategory> activeInfoCategories = new();

    private readonly List<Country> countries = new();
    public readonly List<CountryRenderer> countryRenderers = new();
    public List<Country> GetActiveCountries() => countryRenderers.Select(cRend => cRend.country).ToList();
    //private readonly List<SplineConnection> countryConnections = new();
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

        Debug.LogWarning("MainManager initialized.");
    }

    public InfoCategoryDefinitionSO GetInfoCategoryDefinition(InfoCategory infoCategory)
    {
        return infoCategoryDefinitions.FirstOrDefault(def => def.category == infoCategory);
    }

    public double GetMaxValueForInfoCategory(InfoCategory infoCategory)
    {
        var iCatDef = GetInfoCategoryDefinition(infoCategory);
        var relCategories = iCatDef.connectionThicknessRelativeTo.Intersect(activeInfoCategories);
        var relMaxValues = infoCategoryMaxValues
            .Where(pair => relCategories.Contains(pair.Key))
            .Select(pair => pair.Value);
        return relMaxValues.Max();
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
        /*TODO OnCountryRenderersUpdated();*/
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
        // TODO: Update to use UpdateCountryRelations
    }

    private void OnCountryRenderersUpdated(List<CountryRenderer> updatedCountryRenderers)
    {
        // Check for relations to be removed
        // TODO
        if (pivotCountryRenderer == null)
        {
            // TODO: Remove all relations, return afterwards -> nothing to add
        }
        else
        {
            /*var removedCountries = countryRenderers
                .Where(rend => !updatedCountryRenderers.Contains(rend))
                .Select(rend => rend.country);
            var toRemoveRelations = countryRelations.Where(
                rel => removedCountries
                );

            foreach (var country in removedCountries)
            {
                Destroy(connections[iCat].gameObject);
                connections.Remove(iCat); // TODO: COntinue here, uncomment
            }*/
        }

        // if (pivotCountryRenderer == null) return;
        // Check for relations to be added
        foreach (var cRenderer in countryRenderers)
        {
            if (cRenderer == pivotCountryRenderer) continue;
            var relation = countryRelations.FirstOrDefault(r => r.ConcernsCountry(cRenderer.country));
            if (relation == null)
            {
                var relationGo =
                    new GameObject($"Relation {cRenderer.country.name} - {pivotCountryRenderer.country.name}");
                relationGo.transform.SetParent(transform);
                relation = relationGo.AddComponent<CountryRelation>();
                relation.DataCountryRenderer = cRenderer;
                relation.PivotCountryRenderer = pivotCountryRenderer;
                // TODO: Pass active info categories
                countryRelations.Add(relation);
            }
        }
    }

    void Update()
    {
    }
}