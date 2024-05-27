using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CountryRelation : MonoBehaviour
{
    public CountryRenderer PivotCountryRenderer;
    public CountryRenderer DataCountryRenderer;
    
    public Country PivotCountry => PivotCountryRenderer == null ? null : PivotCountryRenderer.country;
    public Country DataCountry => DataCountryRenderer == null ? null : DataCountryRenderer.country;

    private readonly Dictionary<InfoCategory, SplineConnection> connections = new();

    public void UpdateActiveInfoCategories(List<InfoCategory> activeInfoCategories)
    {
        foreach (var iCat in activeInfoCategories)
        {
            if (connections.ContainsKey(iCat)) continue;
            // SplineConnection does not yet exist, create it
            var iCatDef = MainManager.Instance.GetInfoCategoryDefinition(iCat);
            if (iCatDef == null)
            {
                Debug.LogError($"No info category definition found for category {iCat}!");
                continue;
            }
            
            if (!DataCountry.data.ContainsKey(iCat))
            {
                Debug.LogWarning($"Country ${DataCountry.name} does not provide data for category {iCat}! Skipping...");
                continue;
            }

            var countryConnectionGo =
                new GameObject($"Country Connection - {iCatDef.categoryName} - {DataCountry.name}");
            countryConnectionGo.transform.SetParent(transform);
            var conn = countryConnectionGo.AddComponent<SplineConnection>();
            
            // Spline thickness calculation
            var iCatMaxValue = MainManager.Instance.GetMaxValueForInfoCategory(iCat);
            var splineThickness = Mathf.Lerp(
                SplineConnection.MinSplineThickness,
                SplineConnection.MaxSplineThickness,
                (float)(DataCountry.data[iCat] / iCatMaxValue)
            );

            conn.Init(
                iCatDef.type == CategoryType.TO_PIVOT
                    ? DataCountryRenderer.transform
                    : PivotCountryRenderer.transform,
                iCatDef.type == CategoryType.TO_PIVOT
                    ? PivotCountryRenderer.transform
                    : DataCountryRenderer.transform,
                iCatDef.splineMaterial,
                splineThickness
            );
            
            connections.Add(iCat, conn);
        }

        var toRemoveConnectionCategories = connections.Keys.Where(
            iCat => !activeInfoCategories.Contains(iCat)
        );
        foreach (var iCat in toRemoveConnectionCategories)
        {
            Destroy(connections[iCat].gameObject);
            connections.Remove(iCat);
        }
    }

    public bool ConcernsCountry(Country country)
    {
        return country == PivotCountry || country == DataCountry;
    }
}