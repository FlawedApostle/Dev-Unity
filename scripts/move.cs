using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Example : MonoBehaviour
{
    private float playerSpeed = 5.0f;
    private float jumpHeight = 1.5f;
    private float gravityValue = -9.81f;

    private CharacterController controller;
    [Header("Physics")]
    private Rigidbody rb;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Input Actions")]
    public InputActionReference moveAction; // expects Vector2
    public InputActionReference jumpAction; // expects Button


    private void Awake()
    {
        controller = gameObject.AddComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Read input
        Vector2 input = moveAction.action.ReadValue<Vector2>();                 // this is using the built in unity system
        Vector3 move = new Vector3(input.x, 0, input.y).normalized;

        move = Vector3.ClampMagnitude(move, 1f);

        // Move
        Vector3 velocity = move * playerSpeed;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);


        if (move != Vector3.zero)
        {
            transform.forward = move;
        }

        // Jump
        if (jumpAction.action.triggered && groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Combine horizontal and vertical movement
        Vector3 finalMove = (move * playerSpeed) + (playerVelocity.y * Vector3.up);
        controller.Move(finalMove * Time.deltaTime);
    }
}