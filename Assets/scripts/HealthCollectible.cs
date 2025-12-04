using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float HealthValue = 5.0f; // editable in Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has an OxygenSystem
        HealthSystem healthSystem = other.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.HealthAddAmount(HealthValue);
            Destroy(gameObject); // remove collectible after pickup
        }
    }
}
