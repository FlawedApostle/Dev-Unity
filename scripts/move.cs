using System;
using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GroundCheck))]
// Player Move Script


public class MovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody playerRB;
    private Animator playerAnimator;
    private Vector3 playerMoveDirection;
    private bool isWalking;

    private GroundCheck groundCheck;                        /// calling - groundCheck Raycast script

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        groundCheck = GetComponent<GroundCheck>();          /// calling - groundCheck Raycast script
    }

    void FixedUpdate()
    {
        // Input + animation
        AxisInput();
        AnimateWalking();

        // Movement
        if (playerMoveDirection.sqrMagnitude > 0.01f)
        {
            isWalking = true;
            Debug.Log("check - isWalking: " + isWalking);               // debug
            /// --- Constrain the X & Z axis for upright movment
            //playerRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            /// --- Move player ---
            Vector3 velocity = playerMoveDirection * moveSpeed;
            playerRB.MovePosition(playerRB.position + velocity * Time.fixedDeltaTime);
            /// --- Rotate player to face movment direction ---
            Quaternion targetRotation = Quaternion.LookRotation(playerMoveDirection, Vector3.up);
            playerRB.MoveRotation(Quaternion.Slerp(playerRB.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
        else
        {
            isWalking = false;
            Debug.Log("check - isWalking: " + isWalking);               // debug
        }
    }

    // --- Functions ---
    private void AxisInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        playerMoveDirection = new Vector3(h, 0f, v).normalized;
    }

    private void AnimateWalking()
    {
        playerAnimator.SetBool("isWalking", playerMoveDirection.sqrMagnitude > 0.01f);
    }

    private bool IsUpright()
    {
        float uprightDot = Vector3.Dot(transform.up, Vector3.up);
        return uprightDot > 0.9f;
    }

    private void KeepUpright()
    {
        Quaternion current = transform.rotation;
        Quaternion target = Quaternion.Euler(0, current.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(current, target, 10f * Time.deltaTime);
    }
}
