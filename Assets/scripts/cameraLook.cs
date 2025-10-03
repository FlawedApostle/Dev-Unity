using UnityEngine;

/// <summary>
///  Attaching this to an empty parent of the character object we can manipulate the camera.
///  The camera is separate from the RigidBody components for independent movement.
/// </summary>
public class CameraLook : MonoBehaviour
{
    [Header("Look Sensitivity")]
    [Tooltip("Overall sensitivity for mouse look.")]
    public float lookSensitivity = 100f;
    [Tooltip("Multiplier for vertical sensitivity. Adjust to balance with horizontal.")]
    public float verticalSensitivityMultiplier = 1f;

    [Header("Look Constraints")]
    public float lookUpLimit = -90f;
    public float lookDownLimit = 90f;

    [Header("Smoothing")]
    [Tooltip("Enables smoothing for horizontal and vertical camera movement.")]
    public bool enableSmoothing = true;
    [Range(1f, 30f)]
    [Tooltip("Adjusts how quickly the camera catches up to the target rotation. Lower values are smoother.")]
    public float smoothSpeed = 15f;

    private float xRotation = 0f;
    public float mouseXInput { get; private set; }

    private Quaternion targetRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get raw mouse input, adjusted by overall and vertical sensitivity.
        // DO NOT multiply mouse input by Time.deltaTime.
        mouseXInput = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity * verticalSensitivityMultiplier;

        // Apply mouse input to camera rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, lookUpLimit, lookDownLimit);

        targetRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply smoothing to the vertical camera rotation
        if (enableSmoothing)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoothSpeed * Time.deltaTime);
        }
        else
        {
            transform.localRotation = targetRotation;
        }
    }
}
