using UnityEngine;
using UnityEngine.Events;
// This works in tandem with HealthBarUI class
/// Health System
/// prefab the player collectes. after collection the prefab will add to the players health level
/// if health is below ZERO than the character is dead
public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float startHealth = 100f;
    [SerializeField] private bool depleteHealthOverTime = true;
    [SerializeField] private float depletionHealthRatePerSecond = 1.0f; // health lost per second

    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged; // (current, max)                  // healthBar change
    //public UnityEvent<float, float> OnOxygenChanged; // (current, max)                  // OxygenBar change
    public UnityEvent OnDeath;
    public UnityEvent OnGameOver; // keep separate for different logic later

    // Health - Public Settings
    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;
    public float NormalizedHealth => Mathf.Clamp01(CurrentHealth / maxHealth);             /// Convenience: normalized 0.1 for UI fill


    // isDead check
    private bool isDead = false;

    private void Awake()
    {
        // Health setting the health , and setting the UIBar
        maxHealth = Mathf.Max(1f, maxHealth);
        CurrentHealth = Mathf.Clamp(startHealth, 0f, maxHealth);
        EmitHealthChanged();

    }

    // Update FrameRate
    /// If depleteOverTime == true and is Not Dead then -> depletionRate * Time -> ApplyDamage Function apply Damage to player 
    private void Update()
    {
        // Deplete Health
        if (depleteHealthOverTime && !isDead)
        {
            float delta = depletionHealthRatePerSecond * Time.deltaTime;
            ApplyHealthDamage(delta);
        }
    }
                                                           // *** Health ***
    // DAMAGE - Health
    public void ApplyHealthDamage(float amount)
    {
        if (isDead) return;
        if (amount <= 0f) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        EmitHealthChanged();

        if (CurrentHealth <= 0f && !isDead)
        {
            isDead = true;
            HandleDeath();
        }
    }

    /// Heal
    public void Heal(float amount)
    {
        if (isDead) return;
        if (amount <= 0f) return;

        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        EmitHealthChanged();
    }
    /// Set Health
    public void SetHealth(float value)
    {
        if (isDead) return;

        CurrentHealth = Mathf.Clamp(value, 0f, maxHealth);
        EmitHealthChanged();

        if (CurrentHealth <= 0f && !isDead)
        {
            isDead = true;
            HandleDeath();
        }
    }
    /// Set Health Max
    public void SetMaxHealth(float newMax, bool keepRatio = true)
    {
        newMax = Mathf.Max(1f, newMax);
        float ratio = NormalizedHealth;
        maxHealth = newMax;
        CurrentHealth = keepRatio ? ratio * maxHealth : Mathf.Min(CurrentHealth, maxHealth);
        EmitHealthChanged();
    }
    
    // Depletion - Health
    public void EnableHealthDepletion(bool enabled)
    {
        depleteHealthOverTime = enabled;
    }
    public void SetHealthDepletionRate(float perSecond)
    {
        depletionHealthRatePerSecond = Mathf.Max(0f, perSecond);
    }

    // Update Health Bar
    private void EmitHealthChanged()
    {
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
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
