using UnityEngine;
using System;

/// <summary>
/// Generates signals with properties that increase in difficulty over time.
/// Different signal sources produce signals with different characteristics.
/// </summary>
[System.Serializable]
public class SignalSource
{
    public string sourceName;
    public bool isActive = true;

    /// <summary>
    /// Base signal strength for this source.
    /// </summary>
    public double baseStrength = 100;

    /// <summary>
    /// Base noise level.
    /// </summary>
    public double baseNoise = 80;

    /// <summary>
    /// Base complexity (processing cost).
    /// </summary>
    public double baseComplexity = 1.0;

    /// <summary>
    /// Scaling factor: strength/noise/complexity increase with each signal generated.
    /// </summary>
    public double difficultyScaling = 0.05;

    /// <summary>
    /// How quickly this signal decays while queued.
    /// </summary>
    public float decayRate = 0.01f;

    /// <summary>
    /// Which progression tier this source belongs to.
    /// </summary>
    public int tier = 1;

    /// <summary>
    /// Number of signals generated so far. Used to calculate current difficulty.
    /// </summary>
    public long signalsGenerated = 0;

    /// <summary>
    /// Time since last signal was generated (for rate limiting).
    /// </summary>
    public float timeSinceLastSignal = 0;

    /// <summary>
    /// How often this source produces a signal (per second).
    /// </summary>
    public float generationRate = 1.0f; // 1 signal per second default

    /// <summary>
    /// Constructor with all parameters customizable.
    /// </summary>
    public SignalSource(string name, double str, double noise, double complexity, double scaling, float rate, float decay = 0.01f, int sourceTier = 1)
    {
        sourceName = name;
        baseStrength = str;
        baseNoise = noise;
        baseComplexity = complexity;
        difficultyScaling = scaling;
        generationRate = rate;
        decayRate = decay;
        tier = sourceTier;
    }

    /// <summary>
    /// Generate a new signal from this source.
    /// Difficulty increases with each signal.
    /// </summary>
    public Signal GenerateSignal()
    {
        if (!isActive) return null;

        // Apply difficulty scaling to base properties
        double difficultyMultiplier = 1.0 + (difficultyScaling * signalsGenerated);

        double strength = baseStrength * difficultyMultiplier;
        double noise = baseNoise * difficultyMultiplier;
        double complexity = baseComplexity * difficultyMultiplier;

        Signal signal = new Signal(strength, noise, complexity, decayRate, sourceName, tier);

        signalsGenerated++;
        timeSinceLastSignal = 0;

        Debug.Log($"Signal Source '{sourceName}': Generated signal #{signalsGenerated} - {signal}");
        return signal;
    }

    /// <summary>
    /// Check if this source should generate a new signal this frame.
    /// </summary>
    public bool IsReadyToGenerate(float deltaTime)
    {
        if (!isActive) return false;

        timeSinceLastSignal += deltaTime;
        float interval = 1f / generationRate;

        if (timeSinceLastSignal >= interval)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Get current signal difficulty (scaled multiplier).
    /// </summary>
    public double GetCurrentDifficulty()
    {
        return 1.0 + (difficultyScaling * signalsGenerated);
    }

    /// <summary>
    /// Get human-readable source info.
    /// </summary>
    public override string ToString()
    {
        return $"SignalSource: {sourceName}\n" +
               $"  Tier: {tier}\n" +
               $"  Base: S={baseStrength:F0} N={baseNoise:F0} C={baseComplexity:F2}\n" +
               $"  Difficulty: {GetCurrentDifficulty():F2}x (Scaling: +{difficultyScaling * 100:F1}% per signal)\n" +
               $"  Decay: {decayRate * 100:F1}%/sec\n" +
               $"  Rate: {generationRate:F2} signals/sec\n" +
               $"  Generated: {signalsGenerated} signals";
    }
}

/// <summary>
/// Pre-configured signal sources for different game phases.
/// </summary>
public static class SignalSources
{
    /// <summary>
    /// Local Signals: Early game, weak but numerous.
    /// Fast generation, low difficulty scaling.
    /// </summary>
    public static SignalSource LocalSignals()
    {
        return new SignalSource(
            name: "Local Signals",
            str: 50,
            noise: 40,
            complexity: 0.5,
            scaling: 0.02,
            rate: 2.0f,
            decay: 0.005f,
            sourceTier: 1
        );
    }

    /// <summary>
    /// Deep Space: Mid game, more powerful signals.
    /// Slower generation, higher difficulty.
    /// </summary>
    public static SignalSource DeepSpaceSignals()
    {
        return new SignalSource(
            name: "Deep Space",
            str: 150,
            noise: 120,
            complexity: 1.5,
            scaling: 0.05,
            rate: 0.5f,
            decay: 0.01f,
            sourceTier: 2
        );
    }

    /// <summary>
    /// Alien Encoding: Signals from unknown origins.
    /// Unpredictable and complex.
    /// </summary>
    public static SignalSource AlienEncoding()
    {
        return new SignalSource(
            name: "Alien Encoding",
            str: 200,
            noise: 180,
            complexity: 3.0,
            scaling: 0.08,
            rate: 0.25f,
            decay: 0.015f,
            sourceTier: 3
        );
    }

    /// <summary>
    /// Network Optimization: Signals from distant networks.
    /// Very high SNR potential but requires sophisticated processing.
    /// </summary>
    public static SignalSource NetworkOptimization()
    {
        return new SignalSource(
            name: "Network Optimization",
            str: 300,
            noise: 150,
            complexity: 2.0,
            scaling: 0.03,
            rate: 0.1f,
            decay: 0.02f,
            sourceTier: 4
        );
    }

    /// <summary>
    /// Universal Constants: Endgame signals representing cosmic truths.
    /// Extremely rare, extremely powerful.
    /// </summary>
    public static SignalSource UniversalConstants()
    {
        return new SignalSource(
            name: "Universal Constants",
            str: 500,
            noise: 100,
            complexity: 5.0,
            scaling: 0.01,
            rate: 0.02f,
            decay: 0.03f,
            sourceTier: 5
        );
    }
}
