using UnityEngine;

namespace VFXTools
{
    [ExecuteAlways]
    public class VFXController : MonoBehaviour
    {
        [Header("Paramètres Modifiables")]
        [SerializeField] public Color particleColor = Color.white;
        [SerializeField, Range(0f, 4f)] private float intensity = 1f; 
        [SerializeField] private Vector3 windDirection = Vector3.zero;

        private ParticleSystem[] particleSystems; 
        private float[] defaultRateOverTimeValues;

        void Awake()
        {
            ApplySettings();
        }

        void OnValidate()
        {
            ApplySettings();
        }

        void FindParticles()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>();
            defaultRateOverTimeValues = new float[particleSystems.Length];
        }

        private void ApplySettings()
        {
            if (particleSystems == null || particleSystems.Length == 0)
            {
                FindParticles();
            }

            for (int i = 0; i < particleSystems.Length; i++)
            {
                var ps = particleSystems[i];
                var main = ps.main;
                var emission = ps.emission;
                var velocityOverLifetime = ps.velocityOverLifetime;

                main.startColor = particleColor;

                if (defaultRateOverTimeValues[i] == 0f)
                {
                    defaultRateOverTimeValues[i] = emission.rateOverTime.constant;
                }

                var rate = emission.rateOverTime;

                if (rate.constant > 0f)
                {
                    rate.constant = defaultRateOverTimeValues[i] * intensity;
                }
                else
                {
                    rate.constantMin = defaultRateOverTimeValues[i] * intensity;
                    rate.constantMax = defaultRateOverTimeValues[i] * intensity;
                }

                emission.rateOverTime = rate;

                if (velocityOverLifetime.enabled)
                {
                    velocityOverLifetime.x = windDirection.x;
                    velocityOverLifetime.y = windDirection.y;
                    velocityOverLifetime.z = windDirection.z;
                }
            }
        }

        public void SetParticleColor(Color newColor)
        {
            particleColor = newColor;
            ApplySettings();
        }

        public void SetIntensity(float newIntensity)
        {
            intensity = Mathf.Clamp(newIntensity, 0f, 2f);
            ApplySettings();
        }

        public void SetWindDirection(Vector3 newWindDirection)
        {
            windDirection = newWindDirection;
            ApplySettings();
        }

        public Color GetParticleColor()
        {
            return particleColor;
        }

        public float GetIntensity()
        {
            return intensity;
        }

        public Vector3 GetWindDirection()
        {
            return windDirection;
        }
    }
}
