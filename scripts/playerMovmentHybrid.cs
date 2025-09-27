using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class HybridMovementController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Knockdown Settings")]
    public float knockdownForce = 10f;
    public float recoveryTime = 2f;
    [Header("RigidBody Settings")]
    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isKnockedDown = false;
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //LockUpright(); // Freeze before physics starts
        // Add this line to get the Animator component
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //if (isKnockedDown) return; // Ignore input while knocked down

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(h, 0f, v).normalized;

        // Calculate speed based on Rigidbody velocity
        float speed = rb.linearVelocity.magnitude;

        // Pass the speed value to the animator
        animator.SetFloat("Speed", speed);
    }

    void FixedUpdate()
    {
        if (!isKnockedDown)
        {
            // --- Standing upright & moving ---
            Vector3 velocity = moveDirection * moveSpeed;
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

            if (moveDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
            }
        }
        else
        {

            // --- Knocked down ---
            // Do nothing here — physics handles the fall
            // We just wait for recoveryTime in the coroutine
        }
    }

    public void ApplyHit(Vector3 hitDirection)
    {
        if (isKnockedDown) return;

        isKnockedDown = true;

        // Remove ALL constraints for ragdoll effect
        rb.constraints = RigidbodyConstraints.None;

        // Apply knockback force
        rb.AddForce(hitDirection.normalized * knockdownForce, ForceMode.Impulse);

        // Optional: Add some spin for realism
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

        StartCoroutine(RecoverAfterDelay());
    }

    private IEnumerator RecoverAfterDelay()
    {
        yield return new WaitForSeconds(recoveryTime);

        isKnockedDown = false;
        LockUpright();
        // Snap upright
        Vector3 euler = rb.rotation.eulerAngles;
        rb.rotation = Quaternion.Euler(0f, euler.y, 0f);
        LockUpright();

    }


    /// This works, Lock Upright supposed to be standing upright and tall and not falling over, 'moving as normal'
    private void LockUpright()
    {
        rb.constraints =  RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;  //RigidbodyConstraints.FreezeRotationY
    }

    /// This is supposed to unlock ALL axis of rotation to ALLOW 'rag doll' physics
    private void UnlockRotation()
    {
        rb.constraints = RigidbodyConstraints.None;
    }
}


/*
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class HybridMovementController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Knockdown Settings")]
    public float knockdownForce = 10f;
    public float recoveryTime = 2f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isKnockedDown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        LockUpright();
    }

    void Update()
    {
        if (isKnockedDown) return; // No input while knocked down

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(h, 0f, v).normalized;
    }

    void FixedUpdate()
    {
        if (isKnockedDown) return;

        // Move
        Vector3 velocity = moveDirection * moveSpeed;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        // Rotate to face movement direction -> this needs to change , logic; if what ?
        // if player is not moving;stand upright
        // if player is not hit; stand upright
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    public void ApplyHit(Vector3 hitDirection)
    {
        if (isKnockedDown) return;

        isKnockedDown = true;
        UnlockRotation(); // Let physics take over
        rb.AddForce(hitDirection.normalized * knockdownForce, ForceMode.Impulse);

        StartCoroutine(RecoverAfterDelay());
    }

    private IEnumerator RecoverAfterDelay()
    {
        yield return new WaitForSeconds(recoveryTime);

        // Snap upright
        Vector3 euler = rb.rotation.eulerAngles;
        rb.rotation = Quaternion.Euler(0f, euler.y, 0f);

        LockUpright();
        isKnockedDown = false;
    }

    private void LockUpright()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void UnlockRotation()
    {
        rb.constraints = RigidbodyConstraints.None;
    }
}
*/