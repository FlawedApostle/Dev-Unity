using UnityEngine;
using UnityEngine.UI;
// This works in tandem with OxygenSystem class 
public class OxyGenBarUI : MonoBehaviour
{
    [SerializeField] private OxygenSystem Oxygen;               // Calling Oxygen System Class
    [SerializeField] private Slider slider;

    private void Awake()
    {
        // This is compatible only with a UI that has a slider
        // I followed multiple tutorials and stitched this together with A.I i knida get it and I dont at the same time
        if (Oxygen == null) Oxygen = FindFirstObjectByType<OxygenSystem>();
        if (slider == null) slider = GetComponentInChildren<Slider>();
        // Move slider based on level of Health .. min and max, then value to which the slider moves 
        if (slider != null && Oxygen != null)
        {
            slider.minValue = 0f;
            slider.maxValue = Oxygen.MaxOxygen;
            slider.value = Oxygen.CurrentOxygen;
        }
    }

    private void OnEnable()
    {
        if (Oxygen != null)
            Oxygen.OnOxygenChanged.AddListener(UpdateBar);
    }

    private void OnDisable()
    {
        if (Oxygen != null)
            Oxygen.OnOxygenChanged.RemoveListener(UpdateBar);
    }

    private void UpdateBar(float current, float max)
    {
        if (slider == null) return;
        slider.maxValue = max;
        slider.value = current;
    }
}
