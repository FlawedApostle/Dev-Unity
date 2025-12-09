using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Look Settings")]
    public float mouseSensitivity = 2f;
    ///public float verticalClamp = 80f;       /// up/down limit    -> turned off in Update
    ///public float horizontalClamp = 90f;     /// left/right limit -> turned off in Update

    [Header("Look smoothing Settings")]
    public float lookSmoothSpeed = 10f;     // smoothing factor
    private float smoothX, smoothY;         // smoothed values
    
    [Header("Camera FPS Settings")]
    private float xRotation = 0f;           // pitch
    private float yRotation = 0f;           // yaw
    private Camera cam;

    public Vector3 Forward => cam != null ? cam.transform.forward : transform.forward;
    public Vector3 Right => cam != null ? cam.transform.right : transform.right;

    /// GameManager
    GameMenuManager gameMenuManager;

    /// Pause Menu - Can I look if im in the pause menu ? (short answer is NO !)
    public bool CanLook { get; set; } = true; // default true

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        if (cam == null)
            Debug.LogError("⚠ No Camera found under " + gameObject.name);
    }

    void Start()
    {
        /// Only lock cursor if the game has begun
        if (GameMenuManager.GameIsActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        /// Stop camera rotation + unlock cursor while in menu
        if (!GameMenuManager.GameIsActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (cam == null) return;

        if (!CanLook) return;   // skip rotation if we cannot look

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Accumulate rotations
        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp like a real head -> (removed)
        ///xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        ///yRotation = Mathf.Clamp(yRotation, -horizontalClamp, horizontalClamp);

        // Smoothly interpolate toward target
        smoothX = Mathf.LerpAngle(smoothX, xRotation, Time.deltaTime * lookSmoothSpeed);
        smoothY = Mathf.LerpAngle(smoothY, yRotation, Time.deltaTime * lookSmoothSpeed);

        // Apply yaw to the parent (HeadAnchor)
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        // Apply pitch to the Camera child
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
