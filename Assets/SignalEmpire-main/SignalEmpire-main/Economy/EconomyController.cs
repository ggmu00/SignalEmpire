using UnityEngine;
using System;

public class EconomyController : MonoBehaviour
{
    private SignalEngine manager;

    [Header("Signal Tiers")]
    // Defines the payout multipliers for different signal qualities
    public float commonMult = 1.0f;
    public float rareMult = 15.0f;
    public float anomalousMult = 100.0f;
    public float zenithMult = 1000.0f; // Ultra-rare late game signal

    [Header("Global Inflation Scaling")]
    // Used to keep costs in line with exponential earnings
    public double globalInflationRate = 1.12; 

    void Awake()
    {
        manager = GetComponent<SignalEngine>();
    }

    /// <summary>
    /// Calculates the exact data payout based on tier and active multipliers.
    /// </summary>
    public double CalculatePayout(SignalTier tier)
    {
        double baseVal = manager.GetTapeValue();
        double multiplier = 1.0;

        switch (tier)
        {
            case SignalTier.Common:    multiplier = commonMult; break;
            case SignalTier.Rare:      multiplier = rareMult; break;
            case SignalTier.Anomalous: multiplier = anomalousMult; break;
            case SignalTier.Zenith:    multiplier = zenithMult; break;
        }

        // Apply tech tree and planetary bonuses
        double total = baseVal * multiplier * manager.dataMult;
        
        return total;
    }

    /// <summary>
    /// prestige calculation: How many Void Credits do I get for my current total Data?
    /// Formula: 150 * sqrt(TotalData / 1e9)
    /// </summary>
    public double CalculatePrestigeGain()
    {
        if (manager.totalDataLifetime < 1000000000) return 0;
        return Math.Floor(150 * Math.Sqrt(manager.totalDataLifetime / 1000000000));
    }
}