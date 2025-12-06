using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Spawns health/oxygen pickups on the NavMesh, on a specific ground layer,
/// avoiding furniture/obstacle layers within a radius, with a max active count.
/// </summary>
public class PickupSpawner : MonoBehaviour
{
    [Header("Pickup Prefabs")]
    [Tooltip("Prefab for the Oxygen pickup.")]
    [SerializeField] private GameObject oxygenPrefab;

    [Tooltip("Prefab for the Health pickup.")]
    [SerializeField] private GameObject healthPrefab;

    [Header("Layers")]
    [Tooltip("Layer(s) that count as valid ground for spawning (e.g. Layer_Ground).")]
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("Layer(s) that should be avoided (e.g. Furniture, Walls, etc.).")]
    [SerializeField] private LayerMask obstacleLayers;

    [Header("Spawn Area")]
    [Tooltip("Radius around this GameObject where spawn positions will be searched.")]
    [SerializeField] private float spawnRadius = 20f;

    [Tooltip("How far around the spawn point we check for obstacles (furniture, etc.).")]
    [SerializeField] private float obstacleCheckRadius = 5f;

    [Tooltip("Maximum attempts to find a valid spawn point per spawn cycle.")]
    [SerializeField] private int maxNavMeshSampleTries = 25;

    [Header("Spawn Timing & Limits")]
    [Tooltip("Seconds between spawn attempts.")]
    [SerializeField] private float spawnInterval = 5f;

    [Tooltip("Maximum number of active pickups allowed at once.")]
    [SerializeField] private int maxSpawnCount = 10;

    private float _timer;
    private int _currentSpawnCount;

    private void Start()
    {
        _timer = spawnInterval;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            TrySpawn();
            _timer = spawnInterval;
        }
    }

    /// <summary>
    /// Attempts to spawn a pickup if under the max count and a valid point is found.
    /// </summary>
    private void TrySpawn()
    {
        if (_currentSpawnCount >= maxSpawnCount)
            return;

        if (oxygenPrefab == null && healthPrefab == null)
        {
            Debug.LogWarning("PickupSpawner: No pickup prefabs assigned.", this);
            return;
        }

        if (!FindValidSpawnPoint(out Vector3 spawnPos))
            return;

        GameObject prefabToSpawn = ChooseRandomPickupPrefab();
        if (prefabToSpawn == null)
            return;

        GameObject instance = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // Attach / get tracker so we can decrement count when destroyed.
        PickupTracker tracker = instance.GetComponent<PickupTracker>();
        if (tracker == null)
        {
            tracker = instance.AddComponent<PickupTracker>();
        }
        tracker.RegisterSpawner(this);

        _currentSpawnCount++;
    }

    /// <summary>
    /// Finds a valid position on the NavMesh, on the ground layer, and not too close to obstacles.
    /// </summary>
    private bool FindValidSpawnPoint(out Vector3 result)
    {
        for (int i = 0; i < maxNavMeshSampleTries; i++)
        {
            // Random point in a circle around this spawner (XZ plane).
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 randomPos = new Vector3(
                transform.position.x + randomCircle.x,
                transform.position.y + 5f,  // Start a bit above
                transform.position.z + randomCircle.y
            );

            // Sample the NavMesh near that random point.
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit navHit, 10f, NavMesh.AllAreas))
            {
                // Raycast DOWN to the ground layer to ensure we hit the ground.
                Ray ray = new Ray(navHit.position + Vector3.up * 5f, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit groundHit, 20f, groundLayer))
                {
                    Vector3 candidate = groundHit.point;

                    // Optional: obstacle radius check (only if obstacleLayers is not empty)
                    if (obstacleLayers.value != 0)
                    {
                        bool blocked = Physics.CheckSphere(candidate, obstacleCheckRadius, obstacleLayers);
                        if (blocked)
                        {
                            // Too close to furniture/obstacles, try again
                            continue;
                        }
                    }

                    result = candidate;
                    return true;
                }
            }
        }

        // Failed to find a valid position after X tries
        result = Vector3.zero;
        return false;
    }

    /// <summary>
    /// 50/50 random choice between Health and Oxygen. 
    /// If only one is assigned, always use that one.
    /// </summary>
    private GameObject ChooseRandomPickupPrefab()
    {
        if (oxygenPrefab != null && healthPrefab != null)
        {
            int roll = Random.Range(0, 100); // 0–99
            return roll < 50 ? healthPrefab : oxygenPrefab;
        }

        // Fallback: only one is assigned
        if (oxygenPrefab != null)
            return oxygenPrefab;

        if (healthPrefab != null)
            return healthPrefab;

        return null;
    }

    /// <summary>
    /// Called by PickupTracker on the pickup when it is destroyed/collected.
    /// </summary>
    public void NotifyPickupDestroyed()
    {
        _currentSpawnCount = Mathf.Max(0, _currentSpawnCount - 1);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Visualize the spawn radius in the editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        // Visualize approximate obstacle check radius at center (just as a hint)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, obstacleCheckRadius);
    }
#endif
}
