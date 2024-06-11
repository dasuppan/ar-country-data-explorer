using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.GameEvents.Events;

public class ChangeCountryMenuManager : MonoBehaviour
{
    private UIDocument document;
    [SerializeField] private VisualTreeAsset countryItem;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    private CountryRenderer currentCountryRenderer;

    public void OnCountryRendererEditStarted(CountryRenderer countryRenderer)
    {
        currentCountryRenderer = countryRenderer;
        document.enabled = true;
        Func<VisualElement> makeItem = () => countryItem.CloneTree().Q<VisualElement>("CountryItem");
        Action<VisualElement, int> bindItem = (cItem, i) =>
        {
            var country = MainManager.Instance.countries[i];
            cItem.Q<VisualElement>("CountryFlag").style.backgroundImage = new StyleBackground(country.flagSprite);
            cItem.Q<Label>("CountryName").text = country.countryName;
            // This is cursed, when the re-binding occurs the old event handlers will still be there
            var btn = cItem.Q<Button>("CountryItem");
            btn.clickable = null;
            btn.clicked += () => OnCountryClicked(country);
        };
        ListView countryList = document.rootVisualElement
            .Q<ListView>("CountryList");
        countryList.selectionType = SelectionType.None;
        countryList.fixedItemHeight = 120;
        countryList.makeItem = makeItem;
        countryList.bindItem = bindItem;
        countryList.itemsSource = MainManager.Instance.countries;
        if (currentCountryRenderer.country != MainManager.Instance.undefinedCountry)
        {
            countryList.selectedIndex =  MainManager.Instance.countries.IndexOf(currentCountryRenderer.country);
        }
        //countryList.selectionChanged += OnSelectionChanged;
    }

    private void OnCountryClicked(Country country)
    {
        Debug.LogWarning($"Selection changed to ${country.countryName}");
        document.enabled = false;
        currentCountryRenderer.SetCountry(country);
    }
}