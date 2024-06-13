using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
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
            List<Country> availableCountries = new();
            if (currentCountryRenderer.country != MainManager.Instance.undefinedCountry)
            {
                availableCountries.Add(countryRenderer.country);
            }
            availableCountries.AddRange(MainManager.Instance.availableCountries);
            Func<VisualElement> makeItem = () => countryItem.CloneTree().Q<VisualElement>("CountryItem");
            Action<VisualElement, int> bindItem = (cItem, i) =>
            {
                var country = availableCountries[i];
                cItem.Q<VisualElement>("CountryFlag").style.backgroundImage = new StyleBackground(country.flagSprite);
                cItem.Q<Label>("CountryName").text = country.countryName;
                var btn = cItem.Q<Button>("CountryItem");
                /*btn.clickable = null;*/
                btn.clicked += () => OnCountryClicked(country);
            };
            ListView countryList = document.rootVisualElement
                .Q<ListView>("CountryList");
            countryList.selectionType = SelectionType.None;
            countryList.fixedItemHeight = 120;
            countryList.makeItem = makeItem;
            countryList.bindItem = bindItem;
            countryList.itemsSource = availableCountries;
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
}