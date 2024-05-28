using System.Collections.Generic;
using UnityEngine;

public class Country
{
    public Country(string name, Sprite flagSprite, Texture2D trackerTexture, bool isPivot = false)
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
    public readonly Dictionary<Country, Dictionary<InfoCategory, double?>> data = new();
}