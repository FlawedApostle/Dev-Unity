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

    [Header("Camera Movement Settings")]
    [SerializeField] private Transform cameraTransform;

    // -- private references
    private Rigidbody rb;
    private bool jumpInputPressed = false; // Flag to track jump input
    private bool isGrounded = false;

    void Start()
    {
        /// --- Get Components - Freeze restriction on rigidBody 
        // - edit this into function to call on and off
        rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.freezeRotation = true; // Prevents the capsule from toppling over
        }

        // --- Important Check ---
        // Ask if GroundCheck object is not assigned
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck Transform is not assigned. Please assign it in the Inspector.");
        }
    }

    // Update is called once per frame (good for input)
    void Update()
    {
        // Check for jump input using the Space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInputPressed = true;    
        }
    }

    // FixedUpdate is called at a fixed interval (good for physics)
    void FixedUpdate()
    {
        // --- Ground Check ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        //if (isGrounded == true) { Debug.Log("Touching layer: " + groundLayer); } //else { Debug.Log("Not Touching Layer" + groundLayer); }

        // --- Handle Input ---
        HandleMovement();
        HandleJump();
    }

    /// --- Handle Movement
    void HandleMovement()
    {
        // Get player input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // --- NEW: Handle Movement ---
        // Check if a camera reference exists
        if (cameraTransform != null)
        {
            // Get the camera's forward and right vectors
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            // Flatten the vectors on the y-axis to prevent vertical movement when looking up/down
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // Calculate the new movement direction based on camera orientation
            Vector3 moveDirection = (camForward * moveVertical + camRight * moveHorizontal).normalized;

            // Apply movement force
            if (moveDirection.magnitude > 0)
            {
                rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);
            }
            else // Apply drag if no input is detected
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.95f, rb.linearVelocity.y, rb.linearVelocity.z * 0.95f);
            }
        }
        else // Fallback if no camera reference is found
        {
            // Calculate movement direction using the capsule's own orientation
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

        // --- Optional: Rotate the character to face the direction of movement
        //Vector3 currentVelocity = rb.linearVelocity;
        //currentVelocity.y = 0;
        //if (currentVelocity.sqrMagnitude > 0.1f)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        //}
    }

    /*
        void HandleMovement()
        {
            // Get player input
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            // Calculate movement direction
            Vector3 moveDirection = new Vector3(moveHorizontal, 0, moveVertical);

            // Apply movement force only if there is inpiut
            if (moveDirection.magnitude > 0)
            {
                rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);  // The velocity is kept cleaner by using a ForceMode.Force
            }
            else // Apply drag if no input is detected
            {
                //rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.95f, rb.linearVelocity.y, rb.linearVelocity.z * 0.95f);
                rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.95f, rb.linearVelocity.y, rb.linearVelocity.z * 0.95f);
            }
        }
    */

    void HandleJump()
    {
        // Handle jump logic in FixedUpdate, based on the flag set in Update
        if (jumpInputPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpInputPressed = false; // Reset the flag after performing the jump
        }
        else
        {
            jumpInputPressed = false; // Reset the flag if not grounded, preventing a queued jump mid-air
        }
    }

    /*
    //// Ground check function
    //bool IsGrounded()
    //{
    //    float capsuleHeight = GetComponent<CapsuleCollider>().height;
    //    float capsuleRadius = GetComponent<CapsuleCollider>().radius;
    //    Vector3 capsuleCenter = transform.position + Vector3.up * capsuleHeight / 2f;

    //    // Create a small sphere check at the bottom of the capsule
    //    return Physics.CheckCapsule(capsuleCenter, capsuleCenter + Vector3.down * (capsuleHeight / 2f + 0.1f), capsuleRadius, LayerMask.GetMask("ground"));
    //}
    */

    // --- Visualization for debugging ---
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
