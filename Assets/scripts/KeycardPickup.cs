using UnityEngine;

public enum KeycardColor { Red, Blue, Green, Yellow }       

[RequireComponent(typeof(Collider))]                /// add collider componenet if user doesnt
public class Keycard : MonoBehaviour
{
    [Header("Keycard Settings")]
    public KeycardColor keycardColor = KeycardColor.Red;

    private void Reset()
    {
        // Ensure collider is set to trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }
}



/*
using UnityEngine;

public enum KeycardColor { Red, Blue, Green, Yellow }
public player; 
[RequireComponent(typeof(Collider))]
public class Keycard : MonoBehaviour
{
    [Header("Keycard Settings")]
    public KeycardColor keycardColor = KeycardColor.Red; // Set color in Inspector

    private void OnTriggerEnter(Collider other)
    {
        //float distance = Vector3.Distance(transform.position, other.CompareTag("Player"));
        // Check if the object that touched it is the Player
        if (other.CompareTag("Player"))
        {
            PlayerInventory.AddKeycard(keycardColor);
            Debug.Log($"Picked up {keycardColor} keycard!");
            Destroy(gameObject); // Remove keycard from scene
        }
    }
}
*/