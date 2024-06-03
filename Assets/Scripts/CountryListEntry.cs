using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.GameEvents.Events;

public class CountryListEntry : MonoBehaviour
{
    public Image Image;

    public TextMeshProUGUI Title;

    public Country country;

    public CountryEvent countryPickedEvent;

    public void OnButtonClick()
    {
        Debug.LogWarning($"Country {country.countryName} was clicked!");
        countryPickedEvent.Raise(country);
    }
}
