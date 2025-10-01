using UnityEngine;

public class CameraController : MonoBehaviour
{
    // --- Public Inspector Variables ---
    [Header("Look Sensitivity")]
    [SerializeField] private float lookSensitivity = 100f;

    [Header("Look Constraints")]
    [SerializeField] private float lookUpLimit = -90f;
    [SerializeField] private float lookDownLimit = 90f;

    // --- Private References ---
    private Rigidbody playerRigidbody;
    private float xRotation = 0f;
    private float mouseXInput = 0f;
    private float mouseYInput = 0f;

    void Start()
    {
        playerRigidbody = GetComponentInParent<Rigidbody>();
        if (playerRigidbody == null)
        {
            Debug.LogError("Player's Rigidbody not found in parent hierarchy.");
            enabled = false;
            return;
        }

        // Hide and lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Capture input in Update()
    void Update()
    {
        // Get mouse input and apply sensitivity
        mouseXInput = Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
        mouseYInput = Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;
    }

    // Handle physics and rotation in FixedUpdate()
    void FixedUpdate()
    {
        HandleCameraLook();
    }

    private void HandleCameraLook()
    {
        // Vertical camera rotation (looking up and down)
        xRotation -= mouseYInput;
        xRotation = Mathf.Clamp(xRotation, lookUpLimit, lookDownLimit);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal player body rotation using MoveRotation for smoother physics
        Quaternion newRotation = playerRigidbody.rotation * Quaternion.Euler(Vector3.up * mouseXInput);
        playerRigidbody.MoveRotation(newRotation);
    }
}
