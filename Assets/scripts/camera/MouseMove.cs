using UnityEngine;

public class MouseMove : MonoBehaviour
{
    // Speed of which we will turn
    [SerializeField] float turnSpeed = 90f;
    // How far can the head tilt, measured in angles - measured from dead-level
    // Must be higher than headLowerAngleLimit
    [SerializeField] float headUpperAngleLimit = 85f;
    // How far tilt down
    [SerializeField] float headLowerAngleLimit = -80f;

    // Start Rotation - in Degrees
    float yaw = 0f;
    float pitch = 0f;

    // Store head and body orientation at game start
    // We'll derive new orientations by combining these with yaw & pitch
    Quaternion bodyStartOrientation;
    Quaternion headStartOrientation;

    // A reference to the head object to rotate up and down
    /// The body is a current object, no ned for a variable to store reference to it
    /// Rather we will use a Camera Child object to figure out where ' were looking '
    Transform head;

    /// <summary>
    // Game Starts Perform Initial Set-up
    /// </summary>
    private void Start()
    {
        /// Get the object - in this case the head
        head = GetComponentInChildren<Camera>().transform;
        /// Cache the orientation of the body and head
        bodyStartOrientation = transform.localRotation;
        headStartOrientation = head.transform.localRotation;
        /// Lock & hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        // Read the horizontal movment, and scale
        // based on the amount of time thats elapsed and the movment speed
        var horizontal = Input.GetAxis("Mouse X") * Time.deltaTime * turnSpeed;
        var vertical = Input.GetAxis("Mouse Y") * Time.deltaTime * turnSpeed * -1;          // inverting mouseLook (down is down & up is up)

        // Update Yaw and Pitch values
        yaw += horizontal;
        pitch += vertical;

        // Clamp pitch so that we can't look directly up or down
        pitch = Mathf.Clamp(pitch, headLowerAngleLimit, headUpperAngleLimit);

        /// Compute A rotation for the body by rotating around the y-axis
        /// by the number of YAW degrees
        /// same for the head with PITCH degrees
        var bodyRotation = Quaternion.AngleAxis(yaw, Vector3.up);
        var headRotation = Quaternion.AngleAxis(pitch, Vector3.right);

        // Create new rotation for the body and head by combining them
        // with their START rotations
        transform.localRotation = bodyRotation * bodyStartOrientation;
        head.localRotation = headRotation * headStartOrientation;

    }



}
