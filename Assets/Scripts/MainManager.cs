using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Countries;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MainManager : UnitySingleton<MainManager>
{
    [SerializeField] private float trackerWidthInMeters = 0.09f;
    [FormerlySerializedAs("countries")] public List<CountryDefinitionSO> countryDefinitions = new();
    private ARTrackedImageManager trackedImageManager;
    [SerializeField]
    private GameObject countryPrefab;

    /*private readonly Dictionary<string, GameObject> instantiatedCountries = new();*/

    void Start()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();

        if (trackedImageManager == null)
        {
            Debug.LogError("No ARTrackedImageManager component found.");
            return;
        }

        /*var library = trackedImageManager.CreateRuntimeLibrary();
        if (library is MutableRuntimeReferenceImageLibrary mutableLibrary)
        {
            foreach (var country in countries)
            {
                Debug.LogWarning(
                    $"Format supported? {mutableLibrary.IsTextureFormatSupported(country.tracker.format)}");
                var imageToAdd = country.tracker;
                mutableLibrary.ScheduleAddImageWithValidationJob(
                    imageToAdd,
                    imageToAdd.name,
                    trackerWidthInMeters);
            }
        }
        else
        {
            Debug.LogError("Can't fill library with images.");
            return;
        }*/

        Debug.Log("Init done.");
    }

    void Update()
    {
    }

    /*void OnEnable() => trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

    void OnDisable() => trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        Debug.LogError($"Change detected");
        foreach (var newImage in eventArgs.added)
        {
            var countryDef =
                countryDefinitions.FirstOrDefault(c => c.trackerTexture.name == newImage.referenceImage.name);
            if (countryDef != null)
            {
                Debug.LogWarning($"Adding country {countryDef.name}");
                if (instantiatedCountries.ContainsKey(countryDef.countryName))
                {
                    return;
                }

                var countryGo = Instantiate(countryPrefab, newImage.transform);
                countryGo.name = countryDef.countryName;
                countryGo.GetComponent<Country>().Init(countryDef);
                instantiatedCountries.Add(countryDef.countryName, countryGo);
            }
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event
        }

        foreach (var removedImage in eventArgs.removed)
        {
            
            var countryDef =
                countryDefinitions.FirstOrDefault(c => c.trackerTexture.name == removedImage.referenceImage.name);
            if (countryDef != null)
            {
                Debug.LogWarning($"Removing country {countryDef.name}");
                if (instantiatedCountries.ContainsKey(countryDef.countryName))
                {
                    var countryGo = instantiatedCountries[countryDef.countryName];
                    if (countryGo != null)
                    {
                        Destroy(countryGo);
                    }

                    instantiatedCountries.Remove(countryDef.countryName);
                }
            }
        }
    }*/
}