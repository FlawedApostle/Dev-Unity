using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private HealthSystem health;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        if (health == null) health = FindFirstObjectByType<HealthSystem>();
        if (slider == null) slider = GetComponentInChildren<Slider>();

        if (slider != null && health != null)
        {
            slider.minValue = 0f;
            slider.maxValue = health.MaxHealth;
            slider.value = health.CurrentHealth;
        }
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnHealthChanged.AddListener(UpdateBar);
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnHealthChanged.RemoveListener(UpdateBar);
    }

    private void UpdateBar(float current, float max)
    {
        if (slider == null) return;
        slider.maxValue = max;
        slider.value = current;
    }
}
