using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float startHealth = 100f;
    [SerializeField] private bool depleteOverTime = true;
    [SerializeField] private float depletionRatePerSecond = 1f; // health lost per second

    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged; // (current, max)
    public UnityEvent OnDeath;
    public UnityEvent OnGameOver; // keep separate in case you want different logic later

    // Public, callable value
    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;

    // Convenience: normalized 0..1 for UI fill
    public float Normalized => Mathf.Clamp01(CurrentHealth / maxHealth);

    private bool isDead = false;

    private void Awake()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        CurrentHealth = Mathf.Clamp(startHealth, 0f, maxHealth);
        EmitHealthChanged();
    }
    // If depleteOverTime == true and is Not Dead then -> depletionRate * Time -> ApplyDamage Function apply Damage to player 
    private void Update()
    {
        if (depleteOverTime && !isDead)
        {
            float delta = depletionRatePerSecond * Time.deltaTime;
            ApplyDamage(delta);
        }
    }

    /// DAMAGE
    public void ApplyDamage(float amount)
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
    /// HEAL
    public void Heal(float amount)
    {
        if (isDead) return;
        if (amount <= 0f) return;

        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        EmitHealthChanged();
    }

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

    public void SetMaxHealth(float newMax, bool keepRatio = true)
    {
        newMax = Mathf.Max(1f, newMax);
        float ratio = Normalized;
        maxHealth = newMax;
        CurrentHealth = keepRatio ? ratio * maxHealth : Mathf.Min(CurrentHealth, maxHealth);
        EmitHealthChanged();
    }

    public void EnableDepletion(bool enabled)
    {
        depleteOverTime = enabled;
    }

    public void SetDepletionRate(float perSecond)
    {
        depletionRatePerSecond = Mathf.Max(0f, perSecond);
    }

    private void HandleDeath()
    {
        OnDeath?.Invoke();
        OnGameOver?.Invoke();
        // Optional: disable movement, play animation, etc.
        // Example: GetComponent<PlayerController>()?.enabled = false;
    }

    private void EmitHealthChanged()
    {
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}
