using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ManyToManyDataFileConfigSO", menuName = "ScriptableObjects/ManyToManyDataFileConfigSO",
    order = 5)]
public class ManyToManyDataFileConfigSO : DataFileConfigSO
{
    // CSV Constants for one sided files
    // Structure: Angola, Austria, Value
    private const int colCount = 3;
    private const int country1NameIndex = 0;
    private const int country2NameIndex = 1;
    private const int countryValueIndex = 2;

    public override void ProcessFile(
        List<Country> countries,
        InfoCategory targetInfoCategory,
        out double relevantCountriesMaxValue
    )
    {
        relevantCountriesMaxValue = 0;
        // Assumes same format for all file types
        string[] data = csvFile.text.Split(new[] { ";", "\n" }, StringSplitOptions.None);
        int tableRowCount = data.Length / colCount - 1;
        for (int i = 0; i < tableRowCount; i++)
        {
            string sourceCountryName = data[colCount * country1NameIndex];
            Country sourceCountry = countries.FirstOrDefault(c => c.countryName == sourceCountryName);
            if (sourceCountry == null) continue;
            string destCountryName = data[colCount * (i + country2NameIndex)];
            Country destCountry = countries.FirstOrDefault(c => c.countryName == destCountryName);
            if (destCountry == null) continue;
            double countryValue = double.Parse(data[colCount * (i + countryValueIndex)]);
            
            relevantCountriesMaxValue = Math.Max(relevantCountriesMaxValue, countryValue);
            
            if (sourceCountry == destCountry)
            {
                Debug.LogWarning(
                    $"File ${csvFile.name} makes country self-reference (${sourceCountry.countryName})! Ignoring...");
                continue;
            }

            if (!sourceCountry.data.ContainsKey(destCountry))
            {
                sourceCountry.data[destCountry] = new();
            }

            sourceCountry.data[destCountry][targetInfoCategory] = countryValue;
        }
    }
}