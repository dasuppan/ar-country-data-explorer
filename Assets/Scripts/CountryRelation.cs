using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.GameEvents.Events;

public class CountryRelation : MonoBehaviour
{
    public CountryRenderer countryRenderer1;
    public CountryRenderer countryRenderer2;
    [SerializeField] private GameObject countryConnectionRendererPrefab;
    [SerializeField] private CountryRelationEvent countryRelationRemovedEvent; 

    private readonly List<CountryConnection> connections = new();

    public bool Concerns(
        CountryRenderer countryRenderer1,
        CountryRenderer countryRenderer2
    )
    {
        return (this.countryRenderer1 == countryRenderer1 &&
                this.countryRenderer2 == countryRenderer2) ||
               (this.countryRenderer2 == countryRenderer1 &&
                this.countryRenderer1 == countryRenderer2);
    }

    // Order does not matter
    public void Init(CountryRenderer countryRenderer1, CountryRenderer countryRenderer2)
    {
        this.countryRenderer1 = countryRenderer1;
        this.countryRenderer2 = countryRenderer2;
        gameObject.name = ToString();
    }

    public void ReEvaluate()
    {
        var theirsInfoCategories = MainManager.Instance.activeInfoCategories;
            var oursInfoCategories =
                connections.Select(c => c.infoCategory).Distinct()
                    .ToList(); // Distinct should not be necessary, but we play it safe

            // REMOVAL
            var removedInfoCategories = oursInfoCategories.Except(theirsInfoCategories).ToList();
            var toRemoveConnections = connections.Where(conn =>
                removedInfoCategories.Contains(conn.infoCategory)
            ).ToList();
            // Notify connections of their removal
            toRemoveConnections.ForEach(conn => Destroy(conn.gameObject));
            // Remove connections from lists
            toRemoveConnections.ForEach(conn => connections.Remove(conn));
            var removedConnectionsCount = toRemoveConnections.Count;

            // ADDING
            var missingConnectionsForInfoCategories = theirsInfoCategories.Except(oursInfoCategories).ToList();
            var addedConnectionsCount = 0;
            foreach (var rendererPair in new[]
                     {
                         (countryRenderer1, countryRenderer2),
                         (countryRenderer2, countryRenderer1)
                     })
            {
                var fromCountryRenderer = rendererPair.Item1;
                var toCountryRenderer = rendererPair.Item2;
                foreach (var iCat in missingConnectionsForInfoCategories)
                {
                    var catValue =
                        fromCountryRenderer.country.GetValueForCountryInfoCategory(toCountryRenderer.country, iCat);
                    if (catValue == null) continue;

                    connections.Add(CreateNewConnection(fromCountryRenderer, toCountryRenderer, iCat,
                        (double)catValue));
                    addedConnectionsCount++;
                }
            }
            
            // POSITION UPDATE
            
            for (var i = 0; i < connections.Count; i++)
            {
                connections[i].UpdateSplineMetas(
                    i, 
                    connections.Count,
                    connections.Select(c => c.connectionValue).Max()
                );
            }
            
            Debug.LogWarning(
                $"Relation Re-Evaluation completed ({ToString()}):" +
                $"\nConnections added: {addedConnectionsCount}" +
                $"\nConnections removed: {removedConnectionsCount}"
            );
    }

    /*public void OnCountryRendererChanged(CountryRenderer countryRenderer)
    {
        if (countryRenderer1 == countryRenderer || countryRenderer2 == countryRenderer)
        {
            Debug.LogWarning(
                $"Relation {gameObject.name} invalidated due to renderer change! Removing...");
            countryRelationRemovedEvent.Raise(this);
            Destroy(gameObject);
        }
    }*/

    public void OnCountryRendererRemoved(CountryRenderer countryRenderer)
    {
        if (countryRenderer1 == countryRenderer || countryRenderer2 == countryRenderer)
        {
            Debug.LogWarning($"Removing relation {gameObject.name} due to removed renderer for country {countryRenderer.country.countryName}!");
            countryRelationRemovedEvent.Raise(this);
            Destroy(gameObject);
        }
    }

    private CountryConnection CreateNewConnection(
        CountryRenderer fromCountryRenderer,
        CountryRenderer toCountryRenderer,
        InfoCategory iCat,
        double value
    )
    {
        if (connections.Exists(conn => conn.Concerns(fromCountryRenderer, toCountryRenderer, iCat)))
        {
            Debug.LogError(
                $"There already exists a connection that deals with the visualization of {iCat}: {fromCountryRenderer.country} - {toCountryRenderer.country}! Aborting...");
            return null;
        }

        var iCatDef = MainManager.Instance.GetInfoCategoryDefinition(iCat);
        if (iCatDef == null)
        {
            Debug.LogError($"No info category definition found for category {iCat}! Aborting...");
            return null;
        }

        var countryConnectionGo = Instantiate(countryConnectionRendererPrefab, transform);
        countryConnectionGo.name = $"{iCatDef.categoryName}: {fromCountryRenderer.country.countryName} - {toCountryRenderer.country.countryName}";
        var conn = countryConnectionGo.GetComponent<CountryConnection>();

        // Spline thickness calculation
        /*var iCatMaxValue = MainManager.Instance.GetMaxValueForInfoCategory(iCat);
        var splineThickness = Mathf.Lerp(
            SplineConnection.MinSplineThickness,
            SplineConnection.MaxSplineThickness,
            (float)(value / iCatMaxValue)
        );*/

        conn.Init(
            fromCountryRenderer,
            toCountryRenderer,
            iCat,
            iCatDef.splineMaterial,
            value
        );

        return conn;
    }

    public void RemoveSelf()
    {
        countryRelationRemovedEvent.Raise(this);
        Destroy(gameObject);
    }

    public override string ToString()
    {
        if (countryRenderer1 == null || countryRenderer2 == null) return base.ToString();
        return $"Relation {countryRenderer1.country.countryName} - {countryRenderer2.country.countryName}";
    }
}