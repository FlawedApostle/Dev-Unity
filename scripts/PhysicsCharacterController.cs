// New and Corrected Script - Overwrite your existing file with this code
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PhysicsCharacterController : MonoBehaviour
{
    [Header("Movement and character variables")]
    public float moveSpeed = 10f; // Adjusted for a snappier feel
    public float jumpForce = 15f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Camera Movement Settings")]
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private bool jumpInputPressed = false;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck Transform is not assigned.");
        }

        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogError("No main camera found or assigned!");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInputPressed = true;
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = Vector3.zero;

        if (cameraTransform != null)
        {
            Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;

            moveDirection = (camForward * moveVertical + camRight * moveHorizontal).normalized;
        }
        else
        {
            moveDirection = new Vector3(moveHorizontal, 0, moveVertical).normalized;
        }

        Vector3 newVelocity = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);
    }

    void HandleJump()
    {
        if (jumpInputPressed && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            jumpInputPressed = false;
        }
        else
        {
            jumpInputPressed = false;
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
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