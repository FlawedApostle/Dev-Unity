/*
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform door; // Assign the movable door part
    public float slideDistance;
    public float slideSpeed;

    [Header("Slide Direction")]
    public bool slideUp;
    public bool slideDown;
    public bool slideLeft;
    public bool slideRight;

    [Header("Activation Settings")]
    public Transform player;
    public float activationDistance;
    public bool hasKeycard = false;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isOpening = false;

    void Start()
    {
        if (door == null) door = transform;
        initialPosition = door.position;
        targetPosition = initialPosition + GetSlideVector();
    }

    void Update()
    {
        if (!hasKeycard) return;

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= activationDistance)
        {
            isOpening = true;
            Debug.LogError("distance " + distance);
        }

        if (isOpening)
        {
            door.position = Vector3.MoveTowards(door.position, targetPosition, slideSpeed * Time.deltaTime);
        }
    }

    Vector3 GetSlideVector()
    {
        Vector3 direction = Vector3.zero;
        if (slideUp) direction += Vector3.up;
        if (slideDown) direction += Vector3.down;
        if (slideLeft) direction += Vector3.left;
        if (slideRight) direction += Vector3.right;
        return direction.normalized * slideDistance;
    }

    public void GiveKeycard()
    {
        hasKeycard = true;
    }
}
*/

using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform door;                 // The door object to move
    public float openHeight = 3f;          // How far the door moves up when opening
    public float openSpeed = 2f;           // How fast the door moves
    public float detectionRange = 5f;      // How close the player needs to be

    [Header("Player Reference")]
    public Transform player;               // Player’s Transform (assign in Inspector)

    private Vector3 closedPosition;        // Original position of the door
    private Vector3 openPosition;          // Target position when open
    private bool isOpen = false;

    void Start()
    {
        if (door == null)
            door = transform; /// Default to the same object if no child is set

        closedPosition = door.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, door.position);

        if (distance < detectionRange)
        {
            // Player is close -> open door
            isOpen = true;
        }
        else
        {
            // Player is far -> close door
            isOpen = false;
        }

        // Smooth movement between open and closed positions
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;
        door.position = Vector3.Lerp(door.position, targetPosition, Time.deltaTime * openSpeed);
    }


    public void GiveKeycard()
    {

    }
}
