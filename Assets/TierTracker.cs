using UnityEngine;
using TMPro;
using System;

public class TierTracker : MonoBehaviour
{
    public static TierTracker Instance { get; private set; }

    [SerializeField] private int currentTier = 1;
    [SerializeField] private TMP_Text tierText;
    private int minTier = 1;
    private int maxTier = 3;

    public int CurrentTier => currentTier;

    public event Action<int> OnTierChanged;

    private void Awake() => Instance = this;

    private void Start() => UpdateUI();

    public void IncreaseTier()
    {
        currentTier = Mathf.Min(currentTier + 1, maxTier);
        OnTierChanged?.Invoke(currentTier);
        UpdateUI();
    }

    public void DecreaseTier()
    {
        currentTier = Mathf.Max(currentTier - 1, minTier);
        OnTierChanged?.Invoke(currentTier);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (tierText != null)
            tierText.text = $"Current Tier: {currentTier}";
    }
}
