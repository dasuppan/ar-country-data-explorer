using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CountryRenderer : MonoBehaviour
{
    //private ARTrackedImage trackedImage;
    public Country country { get; private set; }
    [SerializeField]
    private CountryRelation countryRelationPrefab;

    [SerializeField] private CountryDefinitionSO predefinedCountry;
    private SpriteRenderer spriteRenderer;

    //private readonly List<CountryConnection> incomingConnections = new();

    //private readonly List<CountryConnection> outgoingConnections = new();
    /*private readonly Dictionary<InfoCategory, SplineConnection> incomingConnections = new();
    private readonly Dictionary<InfoCategory, SplineConnection> outgoingConnections = new();*/

    public readonly List<CountryRelation> relations = new();

    private List<CountryRenderer> relatedCountryRenderers => relations
        .SelectMany(cRel => new[] { cRel.countryRenderer1, cRel.countryRenderer2 })
        .Where(cRend => cRend != this)
        .ToList();

    public void UpdateRelations()
    {
        var theirsCountryRenderers = MainManager.Instance.countryRenderers;
        var ourCountryRenderers = relatedCountryRenderers;
        var missingRelationsForCountryRenderers = theirsCountryRenderers
            .Except(ourCountryRenderers)
            .Where(cRend => cRend != this).ToList();

        // RELATION ADDING

        var addedRelationsCount = 0;
        foreach (var cRenderer in missingRelationsForCountryRenderers)
        {
            var createdRelation = CreateNewRelation(cRenderer);
            relations.Add(createdRelation);
            cRenderer.relations.Add(createdRelation);
            addedRelationsCount++;
        }
        
        Debug.LogWarning(
            $"Relations updated for cRenderer ({ToString()}):" +
            $"\nRelations added: {addedRelationsCount}"
            /*$"\nRelations removed: {removedConnectionsCount}"*/
        );
    }

    private CountryRelation CreateNewRelation(CountryRenderer toCountryRenderer)
    {
        if (relations.Exists(cRel => cRel.Concerns(this, toCountryRenderer)))
        {
            // TODO: This case will occur often, we should not log this
            Debug.LogWarning(
                $"There already exists a relation that deals with the connections between {country.countryName} and {toCountryRenderer.country}! Aborting...");
            return null;
        }
        
        var countryRelationGo = Instantiate(countryRelationPrefab, Vector3.zero, Quaternion.identity);
        var cRel = countryRelationGo.GetComponent<CountryRelation>();
        cRel.Init(this, toCountryRenderer);
        
        return cRel;
    }

    /*private int GetConnectionsCountTo(CountryRenderer countryRenderer)
    {
        return outgoingConnections
            .Where(conn => conn.toCountryRenderer == countryRenderer)
            .ToList().Count;
    }*/

    void Start()
    {
        if (predefinedCountry != null)
        {
            Debug.LogWarning(
                "$Country Renderer was instantiated with predefined country, skipping reference image comparison...");
            country = MainManager.Instance.GetCountryByName(predefinedCountry.name);
        }
        else
        {
            var trackedImage = GetComponent<ARTrackedImage>();
            country = MainManager.Instance.GetCountryByReferenceImageName(
                trackedImage.referenceImage.name
            );
        }

        if (country != null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sprite = country.flagSprite;
            Debug.LogWarning($"Adding country {country.countryName}");
            Debug.LogWarning($"Country {country.countryName} has {country.data.Count} data points.");
            MainManager.Instance.RegisterCountryRenderer(this);
        }
        else
        {
            // TODO: Should never happen actually
            Debug.LogWarning("No country found for tracker. Renderer is idling...");
        }
    }

    /*private void OnDestroy()
    {
        MainManager.Instance.DeregisterCountryRenderer(this);
    }*/

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.transform.LookAt(Camera.main.transform, Vector3.up);
        // TODO: Respect trackedImage.trackingState?
    }
}