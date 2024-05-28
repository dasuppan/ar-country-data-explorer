using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using UnityEngine;

public enum DataInterpretationMode
{
    COUNTRY_TO_MANY,
    MANY_TO_COUNTRY,
    MANY_TO_MANY // country == null
}

[CreateAssetMenu(fileName = "DataFileConfigSO", menuName = "ScriptableObjects/DataFileConfigSO", order = 3)]
public class DataFileConfigSO : ScriptableObject
{
    public CountryDefinitionSO country;
    public TextAsset csvFile;
    public DataInterpretationMode interpretationMode;

    // CSV Constants
    // Structure: AT, Austria, Value
    private const int colCount = 3;
    private const int countryCodeIndex = 0;
    private const int countryNameIndex = 1;
    private const int countryValueIndex = 2;


    public static void ProcessFile(DataFileConfigSO fileConfig, List<Country> countries,
        CountryDefinitionSO targetCountryDef,
        InfoCategory targetInfoCategory, out double relevantCountriesMaxValue)
    {
        relevantCountriesMaxValue = 0;
        // Assumes same format for all file types
        string[] data = fileConfig.csvFile.text.Split(new[] { ";", "\n" }, StringSplitOptions.None);
        int tableRowCount = data.Length / colCount - 1;
        Country targetCountry = countries.FirstOrDefault(c => c.countryName == targetCountryDef.name);
        if (targetCountry == null)
        {
            Debug.LogError($"Could not find instantiated country {targetCountryDef.name}! Aborting...");
            return;
        }

        for (int i = 0; i < tableRowCount; i++)
        {
            string countryName = data[colCount * (i + 1) + countryNameIndex];
            double countryValue = double.Parse(data[colCount * (i + 1) + countryValueIndex]);

            Country dataCountry = countries.FirstOrDefault(c => c.countryName == countryName);
            if (dataCountry == null) continue;
            relevantCountriesMaxValue = Math.Max(relevantCountriesMaxValue, countryValue);

            if (fileConfig.interpretationMode ==
                DataInterpretationMode.COUNTRY_TO_MANY) // e.g. EXPORTS FROM AUSTRIA
            {
                if (dataCountry == targetCountry)
                {
                    Debug.LogWarning(
                        $"File ${fileConfig.csvFile.name} in mode ${fileConfig.interpretationMode} contains target country ${targetCountry.countryName}!");
                }
                
                if (!targetCountry.data.ContainsKey(dataCountry))
                {
                    targetCountry.data[dataCountry] = new();
                }

                targetCountry.data[dataCountry][targetInfoCategory] = countryValue;
            }
            else if (fileConfig.interpretationMode == DataInterpretationMode.MANY_TO_COUNTRY) // e.g. IMPORTS TO AUSTRIA
            {
                if (dataCountry == targetCountry)
                {
                    Debug.LogWarning(
                        $"File ${fileConfig.csvFile.name} in mode ${fileConfig.interpretationMode} contains target country ${targetCountry.countryName}!");
                }

                if (!dataCountry.data.ContainsKey(targetCountry))
                {
                    dataCountry.data[targetCountry] = new();
                }

                dataCountry.data[targetCountry][targetInfoCategory] = countryValue;
            }
            else if (fileConfig.interpretationMode == DataInterpretationMode.MANY_TO_MANY) // e.g. REFUGEE MOVEMENTS
            {
                Debug.LogError($"Mode ${fileConfig.interpretationMode} is currently not supported! Aborting...");
                return;
            }

            //infoCategoryMaxValues[def.category] = Math.Max(countryValue, infoCategoryMaxValues[def.category]);
        }

        //infoCategoryMaxValues[def.category] = 0;
    }
}