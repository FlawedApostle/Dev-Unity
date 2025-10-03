using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Look Settings")]
    public float mouseSensitivity = 2f;
    public float verticalClamp = 80f;       // up/down limit
    public float horizontalClamp = 90f;     // left/right limit
    
    [Header("Look smoothing Settings")]
    public float lookSmoothSpeed = 10f;     // smoothing factor
    private float smoothX, smoothY;         // smoothed values
    
    [Header("Camera FPS Settings")]
    private float xRotation = 0f;           // pitch
    private float yRotation = 0f;           // yaw
    private Camera cam;

    public Vector3 Forward => cam != null ? cam.transform.forward : transform.forward;
    public Vector3 Right => cam != null ? cam.transform.right : transform.right;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        if (cam == null)
            Debug.LogError("⚠ No Camera found under " + gameObject.name);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (cam == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Accumulate rotations
        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp like a real head
        //xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        //yRotation = Mathf.Clamp(yRotation, -horizontalClamp, horizontalClamp);

        // Smoothly interpolate toward target
        smoothX = Mathf.LerpAngle(smoothX, xRotation, Time.deltaTime * lookSmoothSpeed);
        smoothY = Mathf.LerpAngle(smoothY, yRotation, Time.deltaTime * lookSmoothSpeed);

        // Apply yaw to the parent (HeadAnchor)
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        // Apply pitch to the Camera child
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
