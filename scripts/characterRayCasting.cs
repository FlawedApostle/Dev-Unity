using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ForwardRaycastVisible : MonoBehaviour
{
    [Header("Ray Settings")]
    public float rayDistance = 5f;       // How far the ray will check
    public LayerMask detectionMask;      // Optional: filter what layers to detect

    private LineRenderer lineRenderer;

    void Awake()
    {
        // Get or add the LineRenderer
        lineRenderer = GetComponent<LineRenderer>();

        // Configure the line appearance
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        // Perform the raycast
        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, detectionMask))
        {
            // If we hit something, draw the line to the hit point
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);

            // Optional: log what we hit
            Debug.Log("Hit object: " + hit.collider.name);
        }
        else
        {
            // If nothing is hit, draw the line to max distance
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * rayDistance);
        }
    }
}


/// THIS IS A BASIC RAYCAST THAT ISNT USING LINE-RENDERER - YOU CANNOT SEE IT IN GAME/TEST 

/*
//using UnityEngine;

//public class ForwardRaycastDetector : MonoBehaviour
//{
//    [Header("Raycast Settings")]
//    public float rayDistance = 5f;       // How far the ray will check
//    public LayerMask detectionMask;      // Optional: filter what layers to detect

//    void Update()
//    {
//        // Origin: center of this object
//        Vector3 origin = transform.position;
//        // Direction: forward in world space
//        Vector3 direction = transform.forward;

//        // Perform the raycast
//        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, detectionMask))
//        {
//            // We hit something within range
//            Debug.Log("Hit object: " + hit.collider.name);
//        }
//    }
//}
*/