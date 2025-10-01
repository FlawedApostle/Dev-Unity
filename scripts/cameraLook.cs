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

    private float xRotation = 0f;
    public float mouseXInput { get; private set; }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        mouseXInput = Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, lookUpLimit, lookDownLimit);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
