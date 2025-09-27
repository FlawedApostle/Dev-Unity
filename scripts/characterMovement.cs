
using System;
using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Player movement")]
    public float moveSpeed = 5f;                // set in unity
    public float rotationSpeed = 10f;           // set in unity
    
    Vector3 velocity;                           // Player Velocity
    float h, v;                                 // horizontal & vertical 
    private Vector3 moveDirection;
  
    [Header("Rigidbody Settings")]
    private Rigidbody rb;
  
    [Header("Animation Settings")]
    //[SerializeField] private float speed = 0.5f;
    [SerializeField] private bool isWalking = false;
    private Animator animatorPlayer;                        /// getting the animator component
    
    [Header("Recovery Settings")]
    public float recoveryDelay = 0.5f;
    private bool isFallen = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        animatorPlayer = GetComponent<Animator>();                      /// Animator component

        /// starting upright
        rb_constraintsXZ();
    }

    void Update()
    {
        // get user input controlls from horzontal , vertical input
        movingControls(); 
        moveDirection = new Vector3(h, 0f, v).normalized;               // normalzied for smoother movement, also for 'direction facing' when char is moving
        velocity = moveDirection * moveSpeed;
    }



    /// this needs fixing,
    /* 
    //we need a ground check raycast - if on ground character can move , if not it takes 2 seconds to get up
    // we need to check if the char is moving
    // if the char is moving it muse remain upright unless it is hit then 'ragdoll' constraints(none on x,y,z axis) apply
    // the char must face direction of movement
    // sprint and jump will be added along with animation
    */
    void FixedUpdate()
    {
        if (isFallen) return;       // skipping movement if in 'ragdoll'

        // This is ugly
        // check first to see if on ground - ground raycast
        if (velocity.sqrMagnitude > 0.0001f)  // velocity.sqrMagnitude > 0.0001f & moveDirection.sqrMagnitude > 0.0001f
        {
            isWalking = true;
            animatorPlayer.SetBool("isWalking", true);

            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

            //rb_constraintsXZ();                 // sets constraints on X,Y
            //velocity = moveDirection * moveSpeed;
           
            /*
            // Rotate to face movement direction
            // Checking the rotational movement of object. checking a very small  number helps avoid 
            // We are just comparing lengths - we do not need an accurate distance number for use. again we are just comparing
            // therfore using .sqrMagnitude removes extra math on the cpu by skipping the square root function, instead; x*x + y*y + z*z
            // in this case I am checking for small arbitrarty movement in any direction on the interpolated 'length[movement]' vector
            //if (moveDirection.sqrMagnitude > 0.001f)
            //
            */
            // rotate player to face dir of movement
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

            // Keeps player upright
            Vector3 euler = rb.rotation.eulerAngles;
            rb.MoveRotation(Quaternion.Euler(0f, euler.y, 0f));
        }
        else
        {
            isWalking = false;
            animatorPlayer.SetBool("isWalking", false);
        }
    }

    
    
    private void movingControls()
    {

        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
    }
    /// Freezing upright (player char)
    private void rb_constraintsXZ()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
    /// Releasing constraints on playter char for 'ragdoll' physics to take effect for collisions
    private void rb_releaseConstraints()
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    /// 'Ragdoll' Collision (player char)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block") && !isFallen)
        {
            // Enter ragdoll state
            isFallen = true;
            rb_releaseConstraints();
            animatorPlayer.SetBool("isWalking", false);

            // Start recovery
            StartCoroutine(RecoverAfterDelay());
        }
    }

    /// 'Ragdoll' Recovery after 'ragdoll' (player char)
    private System.Collections.IEnumerator RecoverAfterDelay()
    {
        yield return new WaitForSeconds(recoveryDelay);

        // Stand upright again
        rb_constraintsXZ();
        rb.rotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y, 0f);

        isFallen = false;
    }
}

