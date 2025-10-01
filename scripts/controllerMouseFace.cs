using UnityEngine;

public class PlayerOrientation : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; // Assign your camera's transform in the inspector

    [Header("Settings")]
    public bool lockPitch = true; // Prevent vertical tilt from affecting player rotation

    void Update()
    {
        AlignWithCamera();
    }

    public void AlignWithCamera()
    {
        if (cameraTransform == null) return;

        Vector3 forward = cameraTransform.forward;

        //if (lockPitch)
        //{
            forward.y = 0f;
            forward.Normalize();
        //}

        Quaternion targetRotation = Quaternion.LookRotation(forward);
        transform.rotation = targetRotation;
    }
}
