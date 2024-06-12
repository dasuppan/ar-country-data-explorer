using System;
using GameDesignGame.Utility.GameEvents.Events;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    private UIDocument document;
    [SerializeField] private VoidEvent resetRequestedEvent;
    [SerializeField] private VoidEvent infoCategoriesEditStartedEvent;
    
    private void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    private void Start()
    {
        document.enabled = true;
        document.rootVisualElement.Q<Button>("ResetButton").clicked += () => resetRequestedEvent.Raise();
        document.rootVisualElement.Q<Button>("DataButton").clicked += () => infoCategoriesEditStartedEvent.Raise();
    }

    /*private CountryRenderer currentCountryRenderer;

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
            var btn = cItem.Q<Button>("CountryItem");
            /*btn.clickable = null;#1#
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
    }*/
}