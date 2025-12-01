using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// In order for this script to work the following settings is required in Unity
/// Walkable surfaces must have a box or mesh collider - The raycast settings is checking for the collider  , it will then check what layer it is

public class AIPickupSpawner : MonoBehaviour
{
    [Header("Pickup Settings (Array)")]
    [Tooltip("Built in Array (List) of possible pickup prefabs.(Spawn chance is automatically 100 / # of pickups.")]
    public List<GameObject> pickupPrefabs = new List<GameObject>();
    
    [Header("Pickup Settings (GameObject)")]
    [Tooltip("Add prefab asset to associated GameObject title - (for correct spawning)")]
    [SerializeField] private GameObject Pickup_Health;
    [SerializeField] private GameObject Pickup_Oxygen;

    [Header("NavMesh & Layer Settings")]
    [Tooltip("Layer(s) that this spawner is allowed to spawn on.\nMust match the layer of the ground colliders used to bake the NavMesh.")]
    public LayerMask groundLayers;

    [Header("Avoidance Layers")]
    [Tooltip("Objects on these layers will block spawning within Avoid Radius.")]
    public LayerMask LayerAvoidance;
    //public LayerMask AvoidLayer_1;

    [Tooltip("Minimum distance from furniture objects.")]
    public float avoidRadius = 2.0f;

    [Header("Spawn Rules")]
    [Tooltip("Maximum number of pickups that can exist at once.")]
    public int maxSpawnCount = 10;

    [Tooltip("How many attempts to find a valid point before aborting.")]
    public int maxAttempts = 30;

    // Internal tracking
    private int currentSpawnedCount = 0;

    private NavMeshTriangulation triangulation;

    [Header("Spawn Settings")]
    [Tooltip("How long between pickups are spawned.")]                   // hover mouse over in UNity for info
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float spawnIntervalHealth = 5f;
    [SerializeField] private float spawnIntervalOxygen = 5f;
    private float timer;
    private float timerHealth;

    /// Checking prefab count , Checking and getting NavMesh triangles cache
    void Awake()
    {
        if (pickupPrefabs.Count == 0)
        {
            Debug.LogError("AIPickupSpawner: No pickup prefabs assigned.");
        }

        /// Cache the triangulation at startup
        triangulation = NavMesh.CalculateTriangulation();

        /// Validate the NavMesh + LayerMask
        if (!LayerExistsOnNavMesh())
        {
            Debug.LogError("AIPickupSpawner: The selected groundLayers do not appear anywhere on the NavMesh." +
                " Ensure your ground colliders use the correct layer and the NavMesh is baked with Include Layers.");
        }
    }

    /// How to spawn prefabs;
    /// Spawn health pickup every 10 seconds    - public flaot value
    /// Spawn oxygen pickup every 5 seconds     - public flaot value
    void Update()
    {
        timerHealth += Time.deltaTime;
        SpawnHealth(Time.deltaTime);

        //timer += Time.deltaTime;
        //if (timer >= spawnIntervalHealth)
        //{
            //SpawnOne();
            //SpawnHealth();
            //timer = 0f;
        //}
    }


    // ------------------------------
    // PUBLIC: Call this to spawn one pickup
    // ------------------------------
    public void SpawnOne()
    {
        if (currentSpawnedCount >= maxSpawnCount)
            return;

        GameObject chosenPickup = ChoosePickupByChance();
        if (chosenPickup == null) return;

        Vector3 spawnPos;
        if (TryGetValidSpawnPoint(out spawnPos))
        {
            Instantiate(chosenPickup, spawnPos, Quaternion.identity);
            currentSpawnedCount++;
        }
        else
        {
            Debug.LogWarning("AIPickupSpawner: Could not find valid spawn point after attempts.");
        }
    }

    // ------------------------------
    // SPAWN PICKUP HEALTH
    // ------------------------------
    private void SpawnHealth(float deltaTime)
    {
        /// Set timer , if timer is less than spawn interval return , re-set spawn clock back to 0 (going 0 - 5 each time)
        timerHealth += deltaTime;
        if (timerHealth < spawnIntervalHealth) return;
        timerHealth = 0f;


        if (currentSpawnedCount >= maxSpawnCount)
            return;

        Vector3 spawnPos;
        if (TryGetValidSpawnPoint(out spawnPos))
        {
            Instantiate(Pickup_Health, spawnPos, Quaternion.identity);
            //currentSpawnedCount++;
            Debug.Log("Health Pickup Spawned.");
        }
        else
        {
            Debug.LogWarning("AIPickupSpawner: Could not find valid spawn point after attempts.");
        }

    }

    // ------------------------------
    // PICKUP SELECTION (Auto-Frequency)
    // ------------------------------
    private GameObject ChoosePickupByChance()
    {
        if (pickupPrefabs.Count == 0)
            return null;

        float chancePerPickup = 100f / pickupPrefabs.Count;
        float roll = Random.Range(0f, 100f);

        // Determine index based on roll
        int index = Mathf.FloorToInt(roll / chancePerPickup);
        index = Mathf.Clamp(index, 0, pickupPrefabs.Count - 1);

        return pickupPrefabs[index];
    }

    // ------------------------------
    // PICK RANDOM VALID NAVMESH POINT
    // ------------------------------
    private bool TryGetValidSpawnPoint(out Vector3 result)
    {
        result = Vector3.zero;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {   
            // Get random Point from NavMesh triangle
            Vector3 candidate = GetRandomPointFromTriangulation();

            /// Raycast DOWN to detect ground
            // - start 2f above ground , Raycast DOWN to detect ground , out hit -> store information , Raycast distance of 10f
            if (Physics.Raycast(candidate + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f))
            {
                /// Check if this collider is in the groundLayers
                // hit (which stores the information of what collider it hit)
                // - take that info and the gameObject its attached to and then the Layer of which its assigned
                if ((groundLayers.value & (1 << hit.collider.gameObject.layer)) == 0)
                continue; // Not valid ground layer → skip

                // Check layer avoidance
                if (Physics.CheckSphere(hit.point, avoidRadius, LayerAvoidance))
                //if (Physics.CheckSphere(hit.point, avoidRadius, AvoidLayer_1))
                    continue;

                // Valid
                result = hit.point;
                return true;
            }
        }

        return false;
    }

    // Get a random triangle and random point within it
    private Vector3 GetRandomPointFromTriangulation()
    {
        int triangleIndex = Random.Range(0, triangulation.indices.Length / 3) * 3;

        Vector3 p1 = triangulation.vertices[triangulation.indices[triangleIndex]];
        Vector3 p2 = triangulation.vertices[triangulation.indices[triangleIndex + 1]];
        Vector3 p3 = triangulation.vertices[triangulation.indices[triangleIndex + 2]];

        // Random point inside triangle using barycentric coords
        float r1 = Random.Range(0f, 1f);
        float r2 = Random.Range(0f, 1f);

        // Ensure point is inside triangle
        if (r1 + r2 > 1f)
        {
            r1 = 1f - r1;
            r2 = 1f - r2;
        }

        return p1 + (p2 - p1) * r1 + (p3 - p1) * r2;
    }

    // ------------------------------
    // VALIDATE LAYERS (Error check)
    // ------------------------------
    private bool LayerExistsOnNavMesh()
    {
        foreach (Vector3 v in triangulation.vertices)
        {
            // Cast down to check the collider beneath this NavMesh vertex
            if (Physics.Raycast(v + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f))
            {
                if ((groundLayers.value & (1 << hit.collider.gameObject.layer)) != 0)
                    return true; // Found at least one valid layer match
            }
        }
        return false;
    }
}
