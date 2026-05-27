using UnityEngine;

// Standard Signal Tiers for early to mid-game scaling
public enum SignalTier { Common, Rare, Anomalous, Zenith }

public static class EconomyDefinitions
{
    // --- COST SCALING CONSTANTS ---
    public const double BASE_UPGRADE_COST = 10.0;
    public const double COST_EXPONENT = 1.15; // Standard exponential growth
    
    // --- REWARD SCALING ---
    public const double BASE_SIGNAL_VALUE = 5.0;
    public const double PP_CHANCE_BASE = 0.10; // 10% base chance to gain Power Points per cycle
    
    // --- SIGNAL QUALITY MULTIPLIERS ---
    public const double MULT_COMMON = 1.0;
    public const double MULT_RARE = 15.0;
    public const double MULT_ANOMALOUS = 100.0;
    public const double MULT_ZENITH = 1000.0; 

    public static double GetTierMultiplier(SignalTier signalTier)
    {
        return signalTier switch
        {
            SignalTier.Common => MULT_COMMON,
            SignalTier.Rare => MULT_RARE,
            SignalTier.Anomalous => MULT_ANOMALOUS,
            SignalTier.Zenith => MULT_ZENITH,
            _ => MULT_COMMON
        };
    }

    // --- MATHEMATICAL FORMULAS ---

    /// <summary>
    /// Standard Idle Equation: Cost = Base * (Multiplier ^ Level)
    /// </summary>
    public static double CalculateCost(int level, double baseCost, double multiplier)
    {
        return baseCost * System.Math.Pow(multiplier, level);
    }

    public static double GetBulkCost(int amount, int currentLevel, double baseCost, double multiplier)
    {
        if (multiplier == 1.0) return baseCost * amount;
        double currentFactor = System.Math.Pow(multiplier, currentLevel);
        double amountFactor = System.Math.Pow(multiplier, amount) - 1.0;
        return baseCost * (currentFactor * amountFactor) / (multiplier - 1.0);
    }

    /// <summary>
    /// Calculates the final data payout including Data Decay quality.
    /// Formula: ((Base Value * Tier Multiplier) * (1 + 10% per Tape Level) * globalMult) * quality
    /// </summary>
    public static double CalculateFinalPayout(SignalTier signalTier, int tapeLevel, float quality, double globalMult = 1.0)
    {
        double tierBonus = GetTierMultiplier(signalTier);
        double levelBonus = 1.0 + (tapeLevel * 0.1); // 10% increase per tape level
        
        // The final payout is now directly scaled by the signal's current quality
        return (BASE_SIGNAL_VALUE * tierBonus * levelBonus * globalMult) * (double)quality;
    }
}