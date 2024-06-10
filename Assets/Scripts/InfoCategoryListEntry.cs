using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utils.GameEvents.Events;

public class InfoCategoryListEntry : MonoBehaviour
{
    public Image Image;

    public TextMeshProUGUI Title;

    public InfoCategory InfoCategory;

    public Image TickedImage;

    public bool Ticked;

    /*public CountryEvent countryPickedEvent;*/

    public void OnButtonClick()
    {
        Debug.LogWarning($"Info category {(InfoCategory)} was clicked!");
        /*countryPickedEvent.Raise(country);*/
    }

    public void SetTicked(bool ticked)
    {
        Ticked = ticked;
        TickedImage.enabled = Ticked;
    }

    public void ToggleTick()
    {
        SetTicked(!Ticked);
    }
}
