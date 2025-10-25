using UnityEngine;
using UnityEngine.UI;
// This works in tandem with HealthSystem class 
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private HealthSystem health;               // Calling Health System Class
    [SerializeField] private Slider slider;

    private void Awake()
    {
        // This is compatible only with a UI that has a slider
        // I followed multiple tutorials and stitched this together with A.I i knida get it and I dont at the same time
        if (health == null) health = FindFirstObjectByType<HealthSystem>();
        if (slider == null) slider = GetComponentInChildren<Slider>();
        // Move slider based on level of Health .. min and max, then value to which the slider moves 
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
