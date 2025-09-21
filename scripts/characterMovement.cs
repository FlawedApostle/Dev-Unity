
using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Player movement")]
    public float moveSpeed = 5f;                // set in unity
    public float rotationSpeed = 10f;           // set in unity
    Vector3 velocity;
    [Header("Rigidbody Settings")]
    private Rigidbody rb;
    private Vector3 moveDirection;
    [Header("Animation Settings")]
    [SerializeField] private bool isWalking = false;
    [SerializeField] private float speed = 0.5f;
    private Animator animatorPlayer;                        /// getting the animator component

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        /// Animator component
        animatorPlayer = GetComponent<Animator>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // normalzied for smoother movement, also for 'direction facing' when char is moving
        moveDirection = new Vector3(h, 0f, v).normalized;
        velocity = moveDirection * moveSpeed;
        // Accessing the variables set in Unity Animator panel
        //animatorPlayer.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        //Vector3 velocity = moveDirection * moveSpeed;
        // Move
        if (velocity.sqrMagnitude > 0.0001f & moveDirection.sqrMagnitude > 0.0001f)
        {
            animatorPlayer.SetBool("isWalking", true);
            velocity = moveDirection * moveSpeed;
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

            // Rotate to face movement direction
            // Checking the rotational movement of object. checking a very small  number helps avoid 
            // We are just comparing lengths - we do not need an accurate distance number for use. again we are just comparing
            // therfore using .sqrMagnitude removes extra math on the cpu by skipping the square root function, instead; x*x + y*y + z*z
            // in this case I am checking for small arbitrarty movement in any direction on the interpolated 'length[movement]' vector
            //if (moveDirection.sqrMagnitude > 0.001f)
            //{
                // defining what dir the quat to look in, this case moveDir , and constraining the up vector
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
           // }

            // Euler locks out 'specific' axis choice. in this case only rotating on the y axis
            Vector3 euler = rb.rotation.eulerAngles;
            rb.MoveRotation(Quaternion.Euler(0f, euler.y, 0f));
        }
        else
        {
            animatorPlayer.SetBool("isWalking", false);
        }
    }
}


