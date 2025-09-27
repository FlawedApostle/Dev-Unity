using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GroundCheck))]
public class JumpController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;

    private Rigidbody rb;
    private GroundCheck groundCheck;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        groundCheck = GetComponent<GroundCheck>();
    }

    public void HandleJumpInput()
    {
        // Only jump if grounded and space pressed this frame
        if (Input.GetKeyDown(KeyCode.Space) && groundCheck.IsGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
