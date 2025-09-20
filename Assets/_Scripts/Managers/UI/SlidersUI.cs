using UnityEngine;
using UnityEngine.UI;

public class SlidersUI : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private Slider cooldownSlider;

    [Header("Game References")]
    [SerializeField] private TowerStats playerStats;
    [SerializeField] private WaveSpawner waveSpawner;

    private void Update()
    {
        if (playerStats != null && healthSlider != null)
        {
            float healthPercent = playerStats.CurrentHealth / playerStats.MaxHealth;
            healthSlider.value = Mathf.Clamp01(healthPercent);
        }

        if (waveSpawner != null)
        {
            if (timerSlider != null)
            {
                float timerPercent = waveSpawner.TimeRemaining / waveSpawner.RoundDuration;
                timerSlider.value = Mathf.Clamp01(timerPercent);
            }

            if (cooldownSlider != null)
            {
                float cooldownMax = waveSpawner.CooldownRemaining;
                float cooldownPercent = waveSpawner.CooldownRemaining / cooldownMax;
                cooldownSlider.value = Mathf.Clamp01(cooldownPercent);
            }
        }
    }
}
