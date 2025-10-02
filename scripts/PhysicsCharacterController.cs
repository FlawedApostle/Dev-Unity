using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PhysicsCharacterController : MonoBehaviour
{
    [Header("Movement and character variables")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // The field for the camera's Transform, serialized so you can assign it in the Inspector.
    [Header("Camera Movement Settings")]
    [SerializeField] private Transform cameraTransform;

    // private references
    private Rigidbody rb;
    private bool jumpInputPressed = false;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true; // Prevents the capsule from toppling over
        }

        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck Transform is not assigned. Please assign it in the Inspector.");
        }

        // Find the main camera if the reference isn't set, making the script more robust.
        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogError("No main camera found or assigned! Movement will not be relative to the camera.");
            }
        }
    }

    // Capture input in Update()
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInputPressed = true;
        }
    }

    // FixedUpdate for physics
    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        // Get player input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (cameraTransform != null)
        {
            /// Get the camera's forward and right vectors
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            /// Flatten the vectors on the y-axis and normalize them
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // Calculate the new movement direction based on camera orientation
            Vector3 moveDirection = (camForward * moveVertical + camRight * moveHorizontal);

            /// Apply movement force
            if (moveDirection.magnitude > 0)
            {
                rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);
            }
            else // Apply drag if no input is detected
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.95f, rb.linearVelocity.y, rb.linearVelocity.z * 0.95f);
            }
        }
        else /// Fallback if no camera reference is found
        {
            Vector3 moveDirection = new Vector3(moveHorizontal, 0, moveVertical);
            if (moveDirection.magnitude > 0)
            {
                rb.AddForce(transform.TransformDirection(moveDirection) * moveSpeed, ForceMode.Force);
            }
            else
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.95f, rb.linearVelocity.y, rb.linearVelocity.z * 0.95f);
            }
        }

        /// Optional: Rotate the character to face the direction of movement
        //Vector3 currentVelocity = rb.linearVelocity;
        //currentVelocity.y = 0;
        //if (currentVelocity.magnitude > 0.1f)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        //}
    }

    void HandleJump()
    {
        if (jumpInputPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpInputPressed = false;
        }
        else
        {
            jumpInputPressed = false;
        }
    }

    // Visualization for debugging
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
