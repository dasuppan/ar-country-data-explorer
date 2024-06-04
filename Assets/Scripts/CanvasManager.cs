using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject countryListPanelGo;
    [SerializeField] private GameObject infoCategoryListPanelGo;
    
    public void OnCountryRendererEditStarted(CountryRenderer countryRenderer)
    {
        countryListPanelGo.SetActive(true);
        // TODO: Pretty sketchy to do this after activation
        countryListPanelGo.GetComponent<CountryListPanel>().currentCountryRenderer = countryRenderer;
    }

    public void OnInfoCategoriesEditStarted()
    {
        infoCategoryListPanelGo.SetActive(true);
    }
}