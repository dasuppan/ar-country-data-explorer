using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using Utils.GameEvents.Events;

public class CountryRenderer : MonoBehaviour
{
    public Country country { get; private set; }
    [SerializeField] private CountryRelation countryRelationPrefab;

    [SerializeField] private CountryDefinitionSO predefinedCountry;

    [SerializeField] private CountryRendererEvent countryRendererAddedEvent;
    [SerializeField] private CountryRendererEvent countryRendererRemovedEvent;
    [SerializeField] private CountryRendererEvent countryRendererEditStartedEvent;
    

    private SpriteRenderer spriteRenderer;

    public readonly List<CountryRelation> relations = new();

    private List<CountryRenderer> relatedCountryRenderers => relations
        .SelectMany(cRel => new[] { cRel.countryRenderer1, cRel.countryRenderer2 })
        .Where(cRend => cRend != this)
        .ToList();

    public void SetCountry(Country country, bool suppressRemovedEvent = false)
    {
        if (this.country == country) return;
        if (!suppressRemovedEvent)
        {
            countryRendererRemovedEvent.Raise(this);
        }
        this.country = country;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = this.country.flagSprite;
        Debug.LogWarning(
            $"Changing renderer to country {this.country.countryName} with {this.country.data.Count} data points.");
        countryRendererAddedEvent.Raise(this);
        /*if (!suppressChangedEvent)
        {
            countryRendererChangedEvent.Raise(this);
        }*/
    }

    public void AddMissingRelations()
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
            $"\nRelations added/retrieved: {addedRelationsCount}"
            /*$"\nRelations removed: {removedConnectionsCount}"*/
        );
    }

    private CountryRelation CreateNewRelation(CountryRenderer toCountryRenderer)
    {
        if (relations.Exists(cRel => cRel.Concerns(this, toCountryRenderer)))
        {
            Debug.LogError(
                $"There already exists a relation that deals with the connections between {country.countryName} and {toCountryRenderer.country}! Aborting...");
            return null;
        }

        var countryRelationGo = Instantiate(countryRelationPrefab, Vector3.zero, Quaternion.identity);
        var cRel = countryRelationGo.GetComponent<CountryRelation>();
        cRel.Init(this, toCountryRenderer);
        return cRel;
    }

    private void Start()
    {
        Country c;
        if (predefinedCountry != null)
        {
            c = MainManager.Instance.GetCountryByName(predefinedCountry.name);
            if (c == null)
            {
                Debug.LogWarning("Predefined country not found. Setting to undefined country...");
                c = MainManager.Instance.undefinedCountry;
            }
        }
        else
        {
            c = MainManager.Instance.undefinedCountry;
        }

        SetCountry(c, true);
    }

    /*private void RemoveAllRelations()
    {
        relations.ForEach(cRel => cRel.RemoveSelf());
        // Next line should theoretically not be necessary, as relation raises relationRemovedEvent,
        // which calls OnCountryRelationRemoved, but we play it safe
        relations.Clear();
    }*/

    public void RemoveSelf()
    {
        /*RemoveAllRelations();*/
        countryRendererRemovedEvent.Raise(this);
        Destroy(gameObject);
    }

    public void OnCountryRelationRemoved(CountryRelation countryRelation)
    {
        relations.Remove(countryRelation);
    }

    public void OnEditButtonPressed()
    {
        countryRendererEditStartedEvent.Raise(this);
    }
}