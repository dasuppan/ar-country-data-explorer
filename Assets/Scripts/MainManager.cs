using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using UnityEngine;
using Utils.GameEvents.Events;
using Random = UnityEngine.Random;

public class MainManager : UnitySingleton<MainManager>
{
    [SerializeField] private List<CountryDefinitionSO> countryDefinitions = new();
    [SerializeField] private CountryDefinitionSO undefinedCountryDefinition;
    [SerializeField] public List<InfoCategoryDefinitionSO> infoCategoryDefinitions = new();
    [SerializeField] public List<InfoCategory> activeInfoCategories = new();

    [SerializeField] private CountryRendererEvent countryRendererEditStartedEvent;

    public readonly List<Country> countries = new();
    public Country undefinedCountry { get; private set; }

    public Country GetCountryByName(string cName)
    {
        return countries.FirstOrDefault(c => c.countryName == cName);
    }

    public readonly List<CountryRenderer> countryRenderers = new();
    private readonly Dictionary<InfoCategory, double> infoCategoryMaxValues = new();

    private List<CountryRelation> countryRelations =>
        countryRenderers
            .SelectMany(cRend => cRend.relations)
            .Distinct().ToList();

    void Start()
    {
        undefinedCountry = new Country(undefinedCountryDefinition.countryName, undefinedCountryDefinition.flagSprite);
        // Instantiate countries
        countries.AddRange(
            countryDefinitions.Select(
                cDef => new Country(
                    cDef.countryName,
                    cDef.flagSprite
                )
            )
        );
        countries.Sort((c1, c2) => String.Compare(c1.countryName, c2.countryName, StringComparison.OrdinalIgnoreCase));

        // Parse CSV data
        foreach (var def in infoCategoryDefinitions)
        {
            foreach (var fileConfig in def.fileConfigs)
            {
                // TODO: Respect max value set by previously processed file configs
                fileConfig.ProcessFile(
                    countries,
                    def.category,
                    out var relevantCountriesMaxValue
                );
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

    public void OnCountryRendererAdded(CountryRenderer countryRenderer)
    {
        countryRenderers.Add(countryRenderer);
        Debug.LogWarning($"New country {countryRenderer.country.countryName} was added!");
        if (countryRenderer.country != undefinedCountry)
        {
            // Renderer was created with predefined country, treat as "change" (does hardly happen)
            OnCountryRendererChanged(countryRenderer);
        }
        else
        {
            // Open country picker!
            countryRendererEditStartedEvent.Raise(countryRenderer);
        }
    }

    private void OnCountryRendererChanged(CountryRenderer countryRenderer)
    {
        countryRenderers.ForEach(cRend => cRend.UpdateRelations());
        countryRelations.ForEach(cRel => cRel.ReEvaluate());
    }

    public void OnCountryRendererRemoved(CountryRenderer countryRenderer)
    {
        countryRenderers.Remove(countryRenderer);
        Debug.LogWarning($"Renderer for country {countryRenderer.country.countryName} was removed!");
        OnCountryRendererChanged(countryRenderer);
    }

    public Country GetRandomMissingCountry()
    {
        var availableCountries = countries.Except(countryRenderers.Select(cRend => cRend.country)).ToList();

        if (availableCountries.Count == 0)
        {
            Debug.LogWarning("No more countries to add!");
            return null;
        }

        return availableCountries[Random.Range(0, availableCountries.Count)];
    }
}