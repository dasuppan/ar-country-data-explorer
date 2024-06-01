using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using UnityEngine;
using Utils.GameEvents.Events;

public class CountryRenderer : MonoBehaviour
{
    public Country country { get; private set; }
    [SerializeField]
    private CountryRelation countryRelationPrefab;

    [SerializeField] private CountryDefinitionSO predefinedCountry;
    
    [SerializeField]
    private CountryRendererEvent countryRendererAddedEvent;
    [SerializeField]
    private CountryRendererEvent countryRendererRemovedEvent;
    
    private SpriteRenderer spriteRenderer;

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
            
            /*var trackedImage = GetComponent<ARTrackedImage>();
            country = MainManager.Instance.GetCountryByReferenceImageName(
                trackedImage.referenceImage.name
            );*/
            country = MainManager.Instance.GetRandomMissingCountry();
            // TODO: Handle null case
        }

        if (country != null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sprite = country.flagSprite;
            Debug.LogWarning($"Adding country {country.countryName}");
            Debug.LogWarning($"Country {country.countryName} has {country.data.Count} data points.");
            countryRendererAddedEvent.Raise(this);
        }
        else
        {
            // TODO: Should never happen actually
            Debug.LogWarning("No country found for tracker. Renderer is idling...");
        }
    }
    
    public void RemoveSelf()
    {
        countryRendererRemovedEvent.Raise(this);
        Destroy(gameObject);
    }
    
    public void OnCountryRelationRemoved(CountryRelation countryRelation)
    {
        relations.Remove(countryRelation);
    }
}