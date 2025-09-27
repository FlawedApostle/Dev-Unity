using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform playerBody;   // drag your Player root here
    public Vector3 cameraOffset = new Vector3(0, 1.6f, 0); // head height offset
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // --- Mouse input ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        float mouseZ = Input.GetAxis("Mouse Z") * mouseSensitivity * Time.deltaTime;

        // --- Vertical rotation (look up/down) ---
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // --- Horizontal rotation (turn body) ---
        playerBody.Rotate(Vector3.up * mouseX);

        // --- Position camera at head height ---
        transform.position = playerBody.position + cameraOffset;
    }
}
