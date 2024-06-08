using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using UnityEngine;

namespace ScriptableObjects.InfoCategoryDataFileSO
{
    public enum DataInterpretationMode
    {
        COUNTRY_TO_MANY,
        MANY_TO_COUNTRY
    }

    [CreateAssetMenu(fileName = "OneToManyDataFileConfigSO", menuName = "ScriptableObjects/OneToManyDataFileConfigSO",
        order = 6)]
    public class OneToManyDataFileConfigSO : DataFileConfigSO
    {
        // CSV Constants for one sided files
        // Structure: AT, Austria, Value
        private const int colCount = 3;
        private const int countryCodeIndex = 0;
        private const int countryNameIndex = 1;
        private const int countryValueIndex = 2;

        public CountryDefinitionSO country;
        public DataInterpretationMode interpretationMode;

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
            Country targetCountry = countries.FirstOrDefault(c => c.countryName == country.name);
            if (targetCountry == null)
            {
                Debug.LogError($"Could not find instantiated country {country.name}! Aborting...");
                return;
            }

            for (int i = 0; i < tableRowCount; i++)
            {
                string countryName = data[colCount * (i + 1) + countryNameIndex];
                double countryValue = double.Parse(data[colCount * (i + 1) + countryValueIndex]);

                Country dataCountry = countries.FirstOrDefault(c => c.countryName == countryName);
                if (dataCountry == null) continue;
                relevantCountriesMaxValue = Math.Max(relevantCountriesMaxValue, countryValue);

                if (interpretationMode ==
                    DataInterpretationMode.COUNTRY_TO_MANY) // e.g. EXPORTS FROM AUSTRIA
                {
                    if (dataCountry == targetCountry)
                    {
                        Debug.LogWarning(
                            $"File ${csvFile.name} in mode ${interpretationMode} contains target country ${targetCountry.countryName}!");
                    }

                    if (!targetCountry.data.ContainsKey(dataCountry))
                    {
                        targetCountry.data[dataCountry] = new();
                    }

                    targetCountry.data[dataCountry][targetInfoCategory] = countryValue;
                }
                else if (interpretationMode ==
                         DataInterpretationMode.MANY_TO_COUNTRY) // e.g. IMPORTS TO AUSTRIA
                {
                    if (dataCountry == targetCountry)
                    {
                        Debug.LogWarning(
                            $"File ${csvFile.name} in mode ${interpretationMode} contains target country ${targetCountry.countryName}!");
                    }

                    if (!dataCountry.data.ContainsKey(targetCountry))
                    {
                        dataCountry.data[targetCountry] = new();
                    }

                    dataCountry.data[targetCountry][targetInfoCategory] = countryValue;
                }
                else
                {
                    Debug.LogError(
                        $"Interpretation mode {interpretationMode} is not supported! Aborting...");
                    return;
                }
            }
        }
    }
}