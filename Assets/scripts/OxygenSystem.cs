using UnityEngine;
using UnityEngine.Events;
// This works in tandem with OxygenBarUI class
/// Oxygen System
/// prefab the player collectes. after collection the prefab will add to the players health level
/// if health is below ZERO than the character is dead
public class OxygenSystem : MonoBehaviour
{
    [Header("Oxygen Settings")]
    [SerializeField] private float maxOxygen = 100f;
    [SerializeField] private float startOxygen = 100f;
    [SerializeField] private bool depleteOxygenOverTime = true;
    [SerializeField] private float depletionOxygenRatePerSecond = 1.0f; // oxygen lost per second

    [Header("Events")]
    public UnityEvent<float, float> OnOxygenChanged; // (current, max)                  // healthBar change
    public UnityEvent OnDeath;
    public UnityEvent OnGameOver; // keep separate for different logic later

    // Oxygen - Public Settings
    public float CurrentOxygen { get; private set; }
    public float MaxOxygen => maxOxygen;
    public float NormalizedOxygen => Mathf.Clamp01(CurrentOxygen / maxOxygen);             /// Convenience: normalized 0.1 for UI fill

    // HealthSystem [Settings] - implementation into OxygenSystem
    [SerializeField] private HealthSystem healthSystem;

    // Seperating the Health from 'full death'
    private bool oxygenDepleted = false;

    // isDead check
    private bool isDead = false;

    private void Awake()
    {
        // Updating OxygenSystem UIbar
        SetOxygen();
        // Health - get HealthSystem Component
        if (healthSystem == null)
            healthSystem = FindObjectOfType<HealthSystem>();
        
        EmitOxygenChanged();            /// healthBarUI Slider
    }

    // Update FrameRate
    /// If depleteOverTime == true and is Not Dead then -> depletionRate * Time -> ApplyDamage Function apply Damage to player 
    private void Update()
    {
        // Deplete Health
        if (depleteOxygenOverTime && !isDead)               // if true && notDead
        {
            float delta = depletionOxygenRatePerSecond * Time.deltaTime;
            ApplyOxygenDamage(delta);
        }

        // If oxygen is zero, start damaging health
        if (oxygenDepleted && healthSystem != null && !isDead)
        {
            float healthDamage = depletionOxygenRatePerSecond * Time.deltaTime;
            healthSystem.ApplyHealthDamage(healthDamage);
        }
    }
    
    // Damage Oxygen
    public void ApplyOxygenDamage(float amount)
    {
        if (isDead) return;
        if (amount <= 0f) return;

        CurrentOxygen = Mathf.Max(0f, CurrentOxygen - amount);
        EmitOxygenChanged();
        
        if (CurrentOxygen <= 0f && !isDead)
        {
            oxygenDepleted = true;
        }
    }

    // ADD Oxygen
    public void OxygenAddAmount(float amount)
    {
        if (isDead) return;
        if (amount <= 0f) return;

        CurrentOxygen = Mathf.Min(maxOxygen, CurrentOxygen + amount);
        // if CurrentOxygen is equal or over the max then set health to max
        if (CurrentOxygen >= startOxygen)
        {
            CurrentOxygen = Mathf.Clamp(startOxygen, 0f, maxOxygen);
            EmitOxygenChanged();
        }

        if (CurrentOxygen > 0f)
        {
            oxygenDepleted = false;
        }

        EmitOxygenChanged();
    }
    // Set Oxygen
    public void SetOxygen(float value)
    {
        if (isDead) return;

        CurrentOxygen = Mathf.Clamp(value, 0f, maxOxygen);
        EmitOxygenChanged();

        if (CurrentOxygen <= 0f && !isDead)
        {
            isDead = true;
            HandleDeath();
        }
    }
    
    public void SetOxygen()
    {
        // Updating OxygenSystem UIbar
        maxOxygen = Mathf.Max(1f, maxOxygen);
        CurrentOxygen = Mathf.Clamp(startOxygen, 0f, maxOxygen);
    }
    // Set Oxygen Max
    public void SetMaxOxygen(float newMax, bool keepRatio = true)
    {
        newMax = Mathf.Max(1f, newMax);
        float ratio = NormalizedOxygen;
        maxOxygen = newMax;
        CurrentOxygen = keepRatio ? ratio * maxOxygen : Mathf.Min(CurrentOxygen, maxOxygen);
        EmitOxygenChanged();
    }

    // Depletion - Oxygen
    public void EnableOxygenDepletion(bool enabled)
    {
        depleteOxygenOverTime = enabled;
    }
    public void SetOxygenDepletionRate(float perSecond)
    {
        depletionOxygenRatePerSecond = Mathf.Max(0f, perSecond);
    }

    // Update Oxygen Bar
    private void EmitOxygenChanged()
    {
        OnOxygenChanged?.Invoke(CurrentOxygen, maxOxygen);
    }

    // *** Game Over ***
    /// Death
    private void HandleDeath()
    {
        OnDeath?.Invoke();
        OnGameOver?.Invoke();
        // Optional: disable movement, play animation, etc.
        // Example: GetComponent<PlayerController>()?.enabled = false;
    }



}