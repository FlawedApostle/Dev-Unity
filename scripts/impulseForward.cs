using UnityEngine;

// WE require a rigidBody component to work - this case the player character which has a RB for physics
[RequireComponent(typeof(Rigidbody))]
public class BlastTowardsPlayer : MonoBehaviour
{
    /// These are public unity settings
    public Transform player;       // Assign the player Transform in Inspector
    public float blastForce = 10f; // Impulse strength

    private Rigidbody rb;

    void Start()
    {
        // get player character rb component
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && player != null)          /// spacebar
        {
            // Calculate direction from this object to the player
            // using player transform via rigidBody we can track the player to attack with impulse
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Apply impulse toward the player
            rb.AddForce(directionToPlayer * blastForce, ForceMode.Impulse);
        }
    }
}
