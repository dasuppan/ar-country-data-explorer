using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

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
    private readonly List<Country2> countries = new();

    private readonly Dictionary<Country2, CountryRenderer> instantiatedCountries = new();

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

    public void RegisterCountryRenderer(CountryRenderer countryRenderer, Country2 country)
    {
        instantiatedCountries.Add(country, countryRenderer);
    }
    
    public void DeregisterCountryRenderer(CountryRenderer countryRenderer)
    {
        // TODO: Continue here
        /*var countryKey = instantiatedCountries.FirstOrDefault(pair => pair.Value == countryRenderer);
        
        instantiatedCountries.Remove(
            );*/
    }

    void Update()
    {
    }
}