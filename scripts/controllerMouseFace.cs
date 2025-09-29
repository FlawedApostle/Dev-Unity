using UnityEngine;

public class MouseFacing : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Layers considered ground for mouse raycast (e.g., 'Ground').")]
    public LayerMask groundMask;
    [Tooltip("How fast the object turns toward the mouse.")]
    public float rotationSpeed = 12f;
    [Tooltip("Max raycast distance from camera into the scene.")]
    public float maxRayDistance = 1000f;
    [Tooltip("If true, use a flat plane at this Y when ground raycast fails.")]
    public bool fallbackToPlane = true;
    public float fallbackPlaneY = 0f;

    private Camera mainCam;

    void Awake()
    {
        // Auto-assign camera safely
        mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("[MouseFacing] No Camera.main found. Tag your camera as 'MainCamera'.");
        }

        // Warn if ground mask is empty
        if (groundMask.value == 0)
        {
            Debug.LogWarning("[MouseFacing] GroundMask is empty. Set it to include your ground layer.");
        }
    }

    void Update()
    {
        FaceMouse();
    }

    /// <summary>
    /// Rotates this transform to face the mouse position on ground or fallback plane.
    /// </summary>
    public void FaceMouse()
    {
        if (mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        Vector3 targetPoint;
        if (TryGetMouseWorldPoint(ray, out targetPoint))
        {
            Vector3 dir = targetPoint - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude >= 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private bool TryGetMouseWorldPoint(Ray ray, out Vector3 point)
    {
        // Prefer ground raycast
        if (groundMask.value != 0 &&
            Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            point = hit.point;
            return true;
        }

        // Optional fallback plane (useful if groundMask missed or cursor is over non-ground)
        if (fallbackToPlane)
        {
            Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, fallbackPlaneY, 0f));
            if (groundPlane.Raycast(ray, out float enter))
            {
                point = ray.GetPoint(enter);
                return true;
            }
        }

        point = Vector3.zero;
        return false;
    }

    // Visual aid: draw a short forward gizmo to see facing
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 1.5f);
    }
}
