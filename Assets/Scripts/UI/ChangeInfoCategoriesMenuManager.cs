using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.GameEvents.Events;

namespace UI
{
    public class ChangeInfoCategoriesMenuManager : MonoBehaviour
    {
        private UIDocument document;
        [SerializeField] private VisualTreeAsset infoCategoryItem;
        [SerializeField] private InfoCategoriesEvent infoCategoriesUpdatedEvent;

        private void Awake()
        {
            document = GetComponent<UIDocument>();
        }
    
        private List<InfoCategory> localActiveInfoCategories = new();

        private void OnToggleValueChanged(ChangeEvent<bool> value, InfoCategory infoCategory)
        {
            if (value.newValue)
            {
                localActiveInfoCategories.Add(infoCategory);
            }
            else
            {
                localActiveInfoCategories.Remove(infoCategory);
            }
            Debug.LogWarning("Updated");

            infoCategoriesUpdatedEvent.Raise(localActiveInfoCategories);
        }

        [ContextMenu("Naturally open!")]
        public void OnInfoCategoriesEditStarted()
        {
            document.enabled = true;
            localActiveInfoCategories.Clear();
            localActiveInfoCategories.AddRange(MainManager.Instance.activeInfoCategories);
        
            Func<VisualElement> makeItem = () => infoCategoryItem.CloneTree().Q<VisualElement>("InfoCategoryItem");
            Action<VisualElement, int> bindItem = (cItem, i) =>
            {
                var iCat = MainManager.Instance.infoCategoryDefinitions[i];
                cItem.Q<VisualElement>("InfoCategoryMaterial").style.backgroundColor = iCat.splineMaterial.color;
                cItem.Q<Label>("InfoCategoryName").text = iCat.categoryName;
                Toggle toggle = cItem.Q<Toggle>("InfoCategoryToggle");
                toggle.showMixedValue = false;
                toggle.value = localActiveInfoCategories.Contains(iCat.category);
                toggle.RegisterValueChangedCallback(value => OnToggleValueChanged(value, iCat.category));
            };
            ListView infoCategoriesList = document.rootVisualElement
                .Q<ListView>("InfoCategoriesList");
            infoCategoriesList.selectionType = SelectionType.None;
            infoCategoriesList.fixedItemHeight = 120;
            infoCategoriesList.makeItem = makeItem;
            infoCategoriesList.bindItem = bindItem;
            infoCategoriesList.itemsSource = MainManager.Instance.infoCategoryDefinitions;

            var closeBtn = document.rootVisualElement.Q<Button>("CloseButton");
            closeBtn.clickable = null;
            closeBtn.clicked += OnCloseBtnClicked;
        }

        private void OnCloseBtnClicked()
        {
            Debug.LogWarning($"Closing info categories menu");
            document.enabled = false;
        }
    }
}