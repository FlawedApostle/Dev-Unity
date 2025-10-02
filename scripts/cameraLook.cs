using UnityEngine;
/// <summary>
///  Attaching this to a empty parent of the character object we can manipulate the cam
///  the camera had to be seperate from the rigidb Body components
///  Notice this is a modulare, seperate block of code
///  Meaning; it seperates the movement from rigidbody, therefore both are idnependant.
///  primarly so we can get independant camera movment.
/// </summary>

public class CameraLook : MonoBehaviour
{
    [Header("Look Sensitivity")]
    public float lookSensitivity = 100f;

    [Header("Look Constraints")]
    public float lookUpLimit = -90f;
    public float lookDownLimit = 90f;

    [Header("Smoothing (Optional)")]
    [Tooltip("Check to enable smooth camera movement.")]
    public bool enableSmoothing = false;
    [Range(1f, 30f)]
    [Tooltip("Adjusts how quickly the camera catches up to the target rotation. Lower values are smoother.")]
    public float smoothSpeed = 15f;

    private float xRotation = 0f;
    public float mouseXInput { get; private set; }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // REMOVED Time.deltaTime from mouse input. This is the primary fix.
        mouseXInput = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // Apply mouse input to camera rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, lookUpLimit, lookDownLimit);

        Quaternion targetRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply optional smoothing
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
