using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity = 100f;
    public float verticalClamp = 70f;
    public float smoothTime = 0.05f;

    private float xRotation = 0f; // pitch
    private float yRotation = 0f; // yaw
    private Vector2 currentMouseDelta;
    private Vector2 currentMouseVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Smooth input
        Vector2 targetMouseDelta = new Vector2(mouseX, mouseY) * mouseSensitivity * Time.deltaTime;
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseVelocity, smoothTime);

        // Vertical rotation (pitch)
        xRotation -= currentMouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        // Horizontal rotation (yaw)
        yRotation += currentMouseDelta.x;

        // Apply local rotation to camera pivot
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    // --- Public accessors for other scripts ---
    public float Yaw => yRotation;
    public Vector3 Forward => new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
    public Vector3 Right => new Vector3(transform.right.x, 0f, transform.right.z).normalized;
}
