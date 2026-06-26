using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    private bool loggedMissingCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null)
        {
            if (!loggedMissingCamera)
            {
                Debug.LogError($"{nameof(FaceCamera)} cannot face the camera because no main camera was found.", this);
                loggedMissingCamera = true;
            }
            return;
        }
        transform.rotation = mainCamera.transform.rotation;
    }
}
