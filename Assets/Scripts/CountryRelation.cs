using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class CountryRelation : MonoBehaviour
{
    public Country Country1;
    public Country Country2;
    
    public Dictionary<InfoCategory, SplineConnection> connections = new();

    /*public void Init(Country country1, Country country2)
    {
        this.Country1 = country1;
        this.Country2 = country2;
    }*/

    public void UpdateActiveInfoCategories(IEnumerable<InfoCategory> activeInfoCategories)
    {
       // TODO 
    }

    public bool ConcernsCountry(Country country)
    {
        return country == Country1 || country == Country2;
    }
    
}
