using UnityEngine;

public class OxygenCollectible : MonoBehaviour
{
    [Header("Oxygen Value")]
    [SerializeField] private float oxygenValue = 20f;       // editable in Inspector
    [SerializeField] private OxygenSystem oxygenSystem;     // calling the oxygenSystem

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has an OxygenSystem
        oxygenSystem = other.GetComponent<OxygenSystem>();
        if (oxygenSystem != null)
        {
            oxygenSystem.OxygenAddAmount(oxygenValue);                      // adding the prefab to the OxygenSystem component
            Destroy(gameObject); // remove collectible after pickup
        }
    }
}
