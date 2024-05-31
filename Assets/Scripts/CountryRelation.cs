using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CountryRelation : MonoBehaviour
{
    [SerializeField] private GameObject countryConnectionRendererPrefab;
    public CountryRenderer countryRenderer1;
    public CountryRenderer countryRenderer2;

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
            toRemoveConnections.ForEach(conn => conn.RemoveSelf());
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
                connections[i].UpdatePositionInfo(i, connections.Count);
            }
            
            Debug.LogWarning(
                $"Relation Re-Evaluation completed ({ToString()}):" +
                $"\nConnections added: {addedConnectionsCount}" +
                $"\nConnections removed: {removedConnectionsCount}"
            );
    }

    private void Update()
    {
        /*if (Dirty)
        {
            var currentInfoCategories = MainManager.Instance.activeInfoCategories;
            var currentCountryRenderers = MainManager.Instance.countryRenderers;

            var removedInfoCategories = cachedInfoCategories.Except(currentInfoCategories);
            var removedCountryRenderers = cachedCountryRenderers.Except(currentCountryRenderers);

            // CONNECTION REMOVAL
            // Check if connection removal needed based on removed renderers
            var toRemoveOutgoingConnections1 = connections.Where(conn =>
                removedCountryRenderers.Contains(conn.toCountryRenderer)
            );
            // Check if connection removal needed based on removed infoCategories
            var toRemoveOutgoingConnections2 = connections.Where(conn =>
                removedInfoCategories.Contains(conn.infoCategory)
            );

            var toRemoveOutgoingConnections = toRemoveOutgoingConnections1.Union(toRemoveOutgoingConnections2).ToList();
            // Notify connections of their removal
            toRemoveOutgoingConnections.ForEach(conn => conn.RemoveSelf());
            // Remove connections from lists
            toRemoveOutgoingConnections.ForEach(conn => connections.Remove(conn));
            var removedConnectionsCount = toRemoveOutgoingConnections.Count;

            // CONNECTION EXISTENCE CHECK & ADDING

            var addedConnectionsCount = 0;
            foreach (var cRenderer in currentCountryRenderers)
            {
                foreach (var iCat in currentInfoCategories)
                {
                    if (connections.Exists(conn => conn.Concerns(cRenderer, iCat))) continue;

                    var catData = country.GetDataForCountryInfoCategory(cRenderer.country, iCat);
                    if (catData == null) continue;

                    connections.Add(CreateNewOutgoingConnection(iCat, cRenderer, (double)catData));
                    addedConnectionsCount++;
                }
            }

            // Update position info of outgoing connections according to incoming connections (to/from same country)
            // TODO: This assumes update completion by every country renderer -> race condition
            foreach (var outConn in connections)
            {
                var incomingConnectionsCount = outConn.toCountryRenderer.GetConnectionsCountTo(this);
            }

            cachedInfoCategories.Clear();
            cachedInfoCategories.AddRange(currentInfoCategories);
            cachedCountryRenderers.Clear();
            cachedCountryRenderers.AddRange(currentCountryRenderers);

            Debug.LogWarning(
                $"Relation Evaluation completed ({ToString()}):" +
                $"\nConnections added: {addedConnectionsCount}" +
                $"\nConnections removed: {removedConnectionsCount}"
            );
            Dirty = false;
        }*/
    }

    private CountryConnection CreateNewConnection(
        CountryRenderer fromCountryRenderer,
        CountryRenderer toCountryRenderer,
        InfoCategory iCat,
        double value // TODO: Use this value?
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
        var splineThickness = 0.025f;

        // TODO: Respect sibling connections in spline curvature

        conn.Init(
            fromCountryRenderer,
            toCountryRenderer,
            iCat,
            iCatDef.splineMaterial,
            value
        );

        return conn;
    }

    public override string ToString()
    {
        if (countryRenderer1 == null || countryRenderer2 == null) return base.ToString();
        return $"Relation {countryRenderer1.country.countryName} - {countryRenderer2.country.countryName}";
    }
}