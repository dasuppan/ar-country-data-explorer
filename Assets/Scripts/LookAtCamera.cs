using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Camera cameraToLookAt;

    private void Start()
    {
        if (cameraToLookAt == null) cameraToLookAt = Camera.main;
    }

    void Update()
    {
        /*someTextMesh.transform.rotation = Camera.main.transform.rotation;
        transform.LookAt(cameraToLookAt.transform, Vector3.up);*/
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}