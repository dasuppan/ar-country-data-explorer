using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MyRayCastInteractor : MonoBehaviour
{
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        //var screenCenter = mainCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out var hit, 200f))
        {
            if (hit.collider.gameObject.TryGetComponent(out CountryRenderer cRenderer))
            {
                Debug.LogWarning($"Hit country: {cRenderer.name}");
            }
            //Debug.LogWarning($"Hit: {hit.collider.gameObject.name}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (mainCamera == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 200f);
    }
}
