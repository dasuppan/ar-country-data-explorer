using System.Collections.Generic;
using UnityEngine;

public class Country
{
    public Country(string countryName, Sprite flagSprite, Texture2D trackerTexture)
    {
        this.countryName = countryName;
        this.flagSprite = flagSprite;
        this.trackerTexture = trackerTexture;
    }

    public readonly string countryName;
    public readonly Sprite flagSprite;
    public readonly Texture2D trackerTexture;
    public readonly bool isPivot;
    public readonly Dictionary<Country, Dictionary<InfoCategory, double?>> data = new();

    public double? GetDataForCountryInfoCategory(Country country, InfoCategory infoCategory)
    {
        return data.ContainsKey(country) // Does this country have data for the param country?
            ? data[country]
                .ContainsKey(infoCategory) // Does this country have the InfoCategory data for the param country?
                ? data[country][infoCategory]
                : null
            : null;
    }
}