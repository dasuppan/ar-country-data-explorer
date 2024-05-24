using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Country : MonoBehaviour
{
    void Start()
    {
        var countryDef =
            MainManager.Instance.countryDefinitions.FirstOrDefault(c =>
                c.trackerTexture.name == GetComponent<ARTrackedImage>().referenceImage.name
            );
        if (countryDef != null)
        {
            GetComponent<SpriteRenderer>().sprite = countryDef.flagSprite;
            Debug.LogWarning($"Adding country {countryDef.name}");
        }
        else
        {
            Debug.LogWarning("Tracker is not a country.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Scoobidoobidoo.");
        transform.LookAt(Camera.main.transform, Vector3.up);
    }
}