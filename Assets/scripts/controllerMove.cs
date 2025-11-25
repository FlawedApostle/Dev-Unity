using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSmoothTime = 0.1f;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float turnSmoothVelocity;

    [Header("Jump Settings")]
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float jumpBufferTime = 0.1f;   // coyote time buffer
    public float coyoteTime = 0.1f;       // grace period after leaving ground
    public float fallMultiplier = 2.5f;   // faster fall
    public float lowJumpMultiplier = 2f;  // shorter jump if button released early                                   
    private float lastGroundedTime;
    private float lastJumpPressedTime;

    private PlayerOrientation orientation;



    void Start()
    {
        controller = GetComponent<CharacterController>();
        orientation = GetComponent<PlayerOrientation>();
    }

    void Update()
    {
     
        HandleMovement(); // Your movement still uses transform.forward, so W goes where you face
        HandleGravity();
        //HandleJump();
    }

    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(x, 0f, z).normalized;

        if (direction.sqrMagnitude >= 0.1f)
        {
            // Calculate target angle relative to camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            // Smooth rotation only while moving
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move in facing direction
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
        else
        {
            // No input → do not touch rotation
            turnSmoothVelocity = 0f; // reset smoothing so it doesn’t drift
        }

    }

    void HandleGravity()
    {
        isGrounded = controller.isGrounded;
        // if vel is less tha zero set it to a specfic value
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;                   

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleJump()
    {
        // Track when grounded
        if (isGrounded)
            lastGroundedTime = Time.time;

        // Track when jump pressed
        if (Input.GetButtonDown("Jump"))
            lastJumpPressedTime = Time.time;

        // Jump if within buffer + coyote window
        if (Time.time - lastGroundedTime <= coyoteTime &&
            Time.time - lastJumpPressedTime <= jumpBufferTime)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            lastJumpPressedTime = -999f; // reset so it doesn’t double fire
        }

        // Variable jump height
        if (velocity.y > 0 && !Input.GetButton("Jump"))
        {
            velocity.y += gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y += gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
    }
}

