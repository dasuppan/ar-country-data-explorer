using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CountryRenderer : MonoBehaviour
{
    public Country2 country { get; private set; }

    void Start()
    {
        country = MainManager.Instance.GetCountryByReferenceImageName(
            GetComponent<ARTrackedImage>().referenceImage.name
        );
        if (country != null)
        {
            GetComponent<SpriteRenderer>().sprite = country.flagSprite;
            Debug.LogWarning($"Adding country {country.name}");
            Debug.LogWarning($"Is country pivot? {country.isPivot}");
            Debug.LogWarning($"Country {country.name} has {country.data.Count} data points.");
        }
        else
        {
            Debug.LogWarning("Tracker is not a country.");
        }

        MainManager.Instance.RegisterCountryRenderer(this);
    }

    private void OnDestroy()
    {
        MainManager.Instance.DeregisterCountryRenderer(this);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Scoobidoobidoo.");
        transform.LookAt(Camera.main.transform, Vector3.up);
    }
}