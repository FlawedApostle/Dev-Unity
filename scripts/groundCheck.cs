using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check Settings")]
    public Transform groundCheckPoint;
    public float checkRadius = 0.3f;                            // small sphere radius
    [SerializeField] private LayerMask groundLayer;             /// ground layer for exclusions depending on what the player is wllking on

    public bool IsGrounded { get; private set; }                /// public object usable by other scripts. private set - only this script 'GroundCheck' can set it to true/false
    public Collider GroundCollider { get; private set; }

    void FixedUpdate()
    {
        /// https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Physics.CheckSphere.html
        IsGrounded = Physics.CheckSphere(groundCheckPoint.position, checkRadius, groundLayer);

        if (IsGrounded)
        {
            Debug.Log("isGrounded = " +IsGrounded);         // debug
            Debug.Log("groundLayer = " +groundLayer);         // debug
           
            // Optional: store what we hit
            //Collider[] hits = Physics.OverlapSphere(groundCheckPoint.position, checkRadius, groundLayer);
            //if (hits.Length > 0) GroundCollider = hits[0];
        }
        Debug.Log("isGrounded = True" + IsGrounded);
        // Debug visualization
        Gizmos.color = IsGrounded ? Color.green : Color.red;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, checkRadius);
    }
}
