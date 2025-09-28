using UnityEngine;


///  Raycast setting testing the ground if the player is touching the ground
///  next is collision raycast


public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check Settings")]
    public Transform groundCheckPoint;
    public float checkRadius = 0.1f;                            // small sphere radius
    [SerializeField] private LayerMask groundLayer;             /// ground layer for exclusions depending on what the player is wllking on

    public bool IsGrounded { get; private set; }                /// public object usable by other scripts. private set - only this script 'GroundCheck' can set it to true/false
    public Collider GroundCollider { get; private set; }

    void FixedUpdate()
    {
        /// https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Physics.CheckSphere.html
        IsGrounded = Physics.CheckSphere(groundCheckPoint.position, checkRadius, groundLayer);
        //debug_isGrounded();

        Gizmos.color = IsGrounded ? Color.green : Color.red;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, checkRadius);
    }

    public void debug_isGrounded()
    {
        if (IsGrounded == true)
        {
            Debug.Log("groundCheck script fixedUpdate()  = " + IsGrounded);                 // debug
        }
        else if (!IsGrounded)
        {
            Debug.Log("groundCheck script fixedUpdate()  = " + IsGrounded);                 // debug
        }
    }
}

