using System;
using ScriptableObjects.Countries;
using UnityEngine;
using UnityEngine.UI;

public class InfoCategoryListPanel : MonoBehaviour
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
        foreach (InfoCategoryDefinitionSO catDef in MainManager.Instance.infoCategoryDefinitions)
        {
            GameObject listEntryGo = Instantiate(listEntryPrefab, scrollRect.content);
            listEntryGo.name = $"{catDef.categoryName}";
            InfoCategoryListEntry listEntry = listEntryGo.GetComponent<InfoCategoryListEntry>();
            listEntry.Title.SetText(catDef.categoryName);
            listEntry.InfoCategory = catDef.category;
            listEntry.Image.color = catDef.splineMaterial.color;
        }
    }

    /*public void OnCountryPicked(Country country)
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
    }*/
    
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
