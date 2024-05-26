using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.XR.ARFoundation;

public enum InfoCategory
{
    IMPORT,
    EXPORT
}

public class CountryRenderer : MonoBehaviour
{
    void Start()
    {
        Country2 country = MainManager.Instance.GetCountryByReferenceImageName(
            GetComponent<ARTrackedImage>().referenceImage.name
        );
        if (country != null)
        {
            GetComponent<SpriteRenderer>().sprite = country.flagSprite;
            Debug.LogWarning($"Adding country {country.name}");
            Debug.LogWarning($"Is country pivot? {country.isPivot}");
            Debug.LogWarning($"Country {country.name} has {country.data.Count} data points.");
            MainManager.Instance.RegisterCountryRenderer(this, country);
        }
        else
        {
            Debug.LogWarning("Tracker is not a country.");
        }
    }

    private void Awake()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Scoobidoobidoo.");
        transform.LookAt(Camera.main.transform, Vector3.up);
    }
}