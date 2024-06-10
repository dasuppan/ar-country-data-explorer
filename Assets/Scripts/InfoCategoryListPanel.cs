using System;
using System.Linq;
using ScriptableObjects.Countries;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utils.GameEvents.Events;

public class InfoCategoryListPanel : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject listEntryPrefab;
    [SerializeField] private InfoCategoriesEvent infoCategoriesPickedEvent;
    
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
            listEntry.SetTicked(MainManager.Instance.activeInfoCategories.Contains(catDef.category));
        }
    }

    public void ApplyChanges()
    {
        var infoCategories = GetComponentsInChildren<InfoCategoryListEntry>()
            .Where(e => e.Ticked)
            .Select(e => e.InfoCategory)
            .ToList();
        Debug.LogWarning($"Picked info categories {infoCategories.ToArray()}!");
        infoCategoriesPickedEvent.Raise(infoCategories);
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
