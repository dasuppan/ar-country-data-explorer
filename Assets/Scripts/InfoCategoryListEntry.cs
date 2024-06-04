using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.GameEvents.Events;

public class InfoCategoryListEntry : MonoBehaviour
{
    public Image Image;

    public TextMeshProUGUI Title;

    public InfoCategory InfoCategory;

    /*public CountryEvent countryPickedEvent;*/

    public void OnButtonClick()
    {
        Debug.LogWarning($"Info category {(InfoCategory)} was clicked!");
        /*countryPickedEvent.Raise(country);*/
    }
}
