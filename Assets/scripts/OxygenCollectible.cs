//using UnityEngine;

//public class OxygenCollectible : MonoBehaviour
//{
//    [SerializeField] private float oxygenValue = 20f; // editable in Inspector

//    private void OnTriggerEnter(Collider other)
//    {
//        // Check if the colliding object has an OxygenSystem
//        OxygenSystem oxygenSystem = other.GetComponent<OxygenSystem>();
//        if (oxygenSystem != null)
//        {
//            oxygenSystem.OxygenAddAmount(oxygenValue);
//            Destroy(gameObject); // remove collectible after pickup
//        }
//    }
//}
