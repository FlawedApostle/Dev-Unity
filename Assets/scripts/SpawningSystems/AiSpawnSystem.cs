using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiSpawnSystem : MonoBehaviour
{
    [Header("Pickup Prefabs")]
    [SerializeField] private GameObject Pickup_Health;
    [SerializeField] private GameObject Pickup_Oxygen;

    [Header("NavMesh & Layers")]
    [SerializeField] private LayerMask groundLayers;      // same as your original
    [SerializeField] private LayerMask furnitureLayers;   // avoids furniture
    [SerializeField] private float furnitureAvoidRadius = 2f;

    [Header("Pickup Avoidance (NEW)")]
    [SerializeField] private bool usePickupAvoidance = true;
    [SerializeField] private float pickupAvoidRadius = 2f;
    [SerializeField] private LayerMask pickupLayer;

    [Header("Spawn Limits")]
    [SerializeField] private int maxSpawnCount = 10;
    private int currentSpawnedCount = 0;

    [Header("Spawn Delays (NEW)")]
    [SerializeField] private bool useSpawnDelay = true;

    [Tooltip("Health spawn interval.")]
    [SerializeField] private float healthInterval = 5f;

    [Tooltip("Oxygen spawn interval.")]
    [SerializeField] private float oxygenInterval = 5f;

    [Tooltip("Spawn offset delay (prevents burst spawning).")]
    [SerializeField] private float spawnOffsetDelay = 0.5f;

    private float timerHealth = 0f;
    private float timerOxygen = 0f;

    private NavMeshTriangulation triangulation;

    void Awake()
    {
        triangulation = NavMesh.CalculateTriangulation();
    }

    void Update()
    {
        if (currentSpawnedCount >= maxSpawnCount)
            return;

        if (useSpawnDelay)
        {
            timerHealth += Time.deltaTime;
            timerOxygen += Time.deltaTime;

            if (timerHealth >= healthInterval)
            {
                timerHealth = 0f;
                SpawnPickup(Pickup_Health);
            }

            if (timerOxygen >= oxygenInterval)
            {
                timerOxygen = 0f;
                SpawnPickup(Pickup_Oxygen);
            }
        }
        else
        {
            // No delays → always try spawning
            SpawnPickup(Pickup_Health);
            SpawnPickup(Pickup_Oxygen);
        }
    }

    // ---------------------------
    // SPAWN FUNCTION (NEW CLEAN)
    // ---------------------------
    private void SpawnPickup(GameObject prefab)
    {
        if (currentSpawnedCount >= maxSpawnCount)
            return;

        if (prefab == null)
            return;

        if (TryGetValidSpawnPoint(out Vector3 pos))
        {
            Instantiate(prefab, pos, Quaternion.identity);
            currentSpawnedCount++;
        }
    }

    // ---------------------------
    // NAVMESH POINT CHECK
    // ---------------------------
    private bool TryGetValidSpawnPoint(out Vector3 result)
    {
        result = Vector3.zero;

        for (int i = 0; i < 30; i++)
        {
            Vector3 candidate = GetRandomPointFromTriangulation();

            if (Physics.Raycast(candidate + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 15f))
            {
                // Must be ground layer
                if ((groundLayers.value & (1 << hit.collider.gameObject.layer)) == 0)
                    continue;

                // Avoid furniture
                if (Physics.CheckSphere(hit.point, furnitureAvoidRadius, furnitureLayers))
                    continue;

                // Avoid other pickups
                if (usePickupAvoidance &&
                    Physics.CheckSphere(hit.point, pickupAvoidRadius, pickupLayer))
                    continue;

                result = hit.point;
                return true;
            }
        }

        return false;
    }

    // ---------------------------
    // RANDOM TRIANGLE POINT
    // ---------------------------
    private Vector3 GetRandomPointFromTriangulation()
    {
        int tri = Random.Range(0, triangulation.indices.Length / 3) * 3;

        Vector3 p1 = triangulation.vertices[triangulation.indices[tri]];
        Vector3 p2 = triangulation.vertices[triangulation.indices[tri + 1]];
        Vector3 p3 = triangulation.vertices[triangulation.indices[tri + 2]];

        float r1 = Random.value;
        float r2 = Random.value;

        if (r1 + r2 > 1f)
        {
            r1 = 1f - r1;
            r2 = 1f - r2;
        }

        return p1 + (p2 - p1) * r1 + (p3 - p1) * r2;
    }
}
