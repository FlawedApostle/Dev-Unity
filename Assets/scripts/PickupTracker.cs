using UnityEngine;

/// <summary>
/// Lightweight helper that informs the PickupSpawner when this pickup is destroyed.
/// Does NOT change the pickup's own behaviour.
/// </summary>
public class PickupTracker : MonoBehaviour
{
    private PickupSpawner _spawner;

    /// <summary>
    /// Called by the spawner right after instantiation.
    /// </summary>
    public void RegisterSpawner(PickupSpawner spawner)
    {
        _spawner = spawner;
    }

    private void OnDestroy()
    {
        // This will also fire when exiting play mode, so we check Application.isPlaying.
        if (_spawner != null && Application.isPlaying)
        {
            _spawner.NotifyPickupDestroyed();
        }
    }
}
