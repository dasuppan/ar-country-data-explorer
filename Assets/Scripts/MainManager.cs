using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using UnityEngine;

public class MainManager : UnitySingleton<MainManager>
{
    [SerializeField] private List<CountryDefinitionSO> countryDefinitions = new();
    [SerializeField] private List<InfoCategoryDefinitionSO> infoCategoryDefinitions = new();
    [SerializeField] public List<InfoCategory> activeInfoCategories = new();

    private readonly List<Country> countries = new();

    public Country GetCountryByName(string cName)
    {
        return countries.FirstOrDefault(c => c.countryName == cName);
    }
    
    public readonly List<CountryRenderer> countryRenderers = new();
    private readonly Dictionary<InfoCategory, double> infoCategoryMaxValues = new();

    //private ARTrackedImageManager trackedImageManager;

    private CountryRenderer pivotCountryRenderer => countryRenderers.FirstOrDefault(c => c.country.isPivot);

    void Start()
    {
        // Instantiate countries
        countries.AddRange(
            countryDefinitions.Select(
                cDef => new Country(
                    cDef.countryName,
                    cDef.flagSprite,
                    cDef.trackerTexture
                )
            )
        );

        // Parse CSV data
        foreach (var def in infoCategoryDefinitions)
        {
            
            foreach (var fileConfig in def.fileConfigs)
            {
                // TODO: Respect max value set by previously processed file configs
                DataFileConfigSO.ProcessFile(fileConfig, countries, fileConfig.country, def.category, out var relevantCountriesMaxValue);
                infoCategoryMaxValues[def.category] = relevantCountriesMaxValue;
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
        Debug.LogWarning($"Renderer for country {country.countryName} was added!");
        countryRenderers.ForEach(cRend => cRend.ForceEvaluationOnNextFrame());
    }

    public void DeregisterCountryRenderer(CountryRenderer countryRenderer)
    {
        // TODO: Call this method
        countryRenderers.Remove(countryRenderer);
        Debug.LogWarning($"Renderer for country {countryRenderer.country.countryName} was removed!");
        countryRenderers.ForEach(cRend => cRend.ForceEvaluationOnNextFrame());
    }
}