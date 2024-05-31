using System.Collections.Generic;
using UnityEngine;

public class Country
{
    public Country(string countryName, Sprite flagSprite)
    {
        this.countryName = countryName;
        this.flagSprite = flagSprite;
    }

    public readonly string countryName;
    public readonly Sprite flagSprite;
    public readonly Dictionary<Country, Dictionary<InfoCategory, double?>> data = new();

    public double? GetValueForCountryInfoCategory(Country country, InfoCategory infoCategory)
    {
        return data.ContainsKey(country) // Does this country have data for the param country?
            ? data[country]
                .ContainsKey(infoCategory) // Does this country have the InfoCategory data for the param country?
                ? data[country][infoCategory]
                : null
            : null;
    }
}