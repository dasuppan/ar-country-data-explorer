using System;
using UnityEngine;
using UnityEngine.UI;

public class CountryListPanel : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject listEntryPrefab;
    
    private void OnEnable()
    {
        ClearScrollContent();
        FillScrollContent();
    }

    private void FillScrollContent()
    {
        foreach (Country country in MainManager.Instance.countries)
        {
            // TODO:Only show possible ones 
            GameObject listEntryGo = Instantiate(listEntryPrefab, scrollRect.content);
            listEntryGo.name = $"{country.countryName}";
            CountryListEntry listEntry = listEntryGo.GetComponent<CountryListEntry>();
            listEntry.country = country;
            listEntry.Image.sprite = country.flagSprite;
            listEntry.Title.SetText(country.countryName);
        }
    }

    [NonSerialized]
    public CountryRenderer currentCountryRenderer;

    /*public void OnCountryRendererEditStarted(CountryRenderer countryRenderer)
    {
        currentCountryRenderer = countryRenderer;
        SetPanelActive(true);
    }*/

    public void OnCountryPicked(Country country)
    {
        Debug.LogWarning($"Picked country {country.countryName}!");
        if (currentCountryRenderer == null)
        {
            Debug.LogError($"Current cRenderer is null! Ignoring country pick...");
            gameObject.SetActive(false);
            return;
        }

        currentCountryRenderer.SetCountry(country);
        currentCountryRenderer = null;
        
        gameObject.SetActive(false);
    }
    
    private void ClearScrollContent()
    {
        Transform content = scrollRect.content;

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Optionally reset the content position to the top
        scrollRect.normalizedPosition = Vector2.up;
    }
}
