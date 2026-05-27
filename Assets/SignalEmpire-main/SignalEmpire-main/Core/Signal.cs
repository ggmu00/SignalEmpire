using UnityEngine;

/// <summary>
/// Represents a signal passing through the processing pipeline.
/// Contains measurable properties: Strength, Noise, Complexity.
/// </summary>
[System.Serializable]
public class Signal
{

    public string name;

    
    /// <summary>
    /// Raw signal strength. Higher = more raw input.
    /// </summary>
    public double strength;

    /// <summary>
    /// Signal noise level. Higher = more useless data mixed in.
    /// </summary>
    public double noise;

    /// <summary>
    /// Signal complexity. Higher = harder to process, increases compute cost.
    /// </summary>
    public double complexity;

    /// <summary>
    /// Current state of processing. Tracks partial completion.
    /// </summary>
    public float processingProgress = 0f;

    /// <summary>
    /// Base data extracted from this signal (before pattern bonuses).
    /// </summary>
    public double extractedData = 0;

    /// <summary>
    /// Pattern bonuses applied to this signal (multiplicative).
    /// </summary>
    public double patternMultiplier = 1.0;

    /// <summary>
    /// How long the signal has been waiting in a pipeline.
    /// </summary>
    public float age = 0f;

    /// <summary>
    /// Signal wavelength (in arbitrary units, affects matching difficulty).
    /// </summary>
    public float wavelength;

    /// <summary>
    /// Signal frequency (in Hz, affects matching difficulty).
    /// </summary>
    public float frequency;

    /// <summary>
    /// Remaining processing quality after decay.
    /// </summary>
    public float quality = 1.0f;

    /// <summary>
    /// The originating source name.
    /// </summary>
    public string sourceName = "Unknown";

    /// <summary>
    /// Signal tier to support progression.
    /// </summary>
    public int sourceTier = 1;

    public float decayRate = 0.02f;

    public Signal(double initialStrength = 100, double initialNoise = 80, double initialComplexity = 1.0, float initialDecayRate = 0.01f, string origin = "Unknown", int tier = 1)
    {
        strength = initialStrength;
        noise = initialNoise;
        complexity = initialComplexity;
        decayRate = initialDecayRate;
        sourceName = origin;
        sourceTier = tier;
        
        // Generate random wavelength and frequency for matching
        wavelength = UnityEngine.Random.Range(400f, 700f); // Visible spectrum range
        frequency = UnityEngine.Random.Range(1f, 100f); // 1-100 Hz
        
        extractedData = GetUsableInput();
    }

    /// <summary>
    /// Calculate signal-to-noise ratio.
    /// </summary>
    public double GetSignalToNoiseRatio()
    {
        if (noise <= 0) return strength / 0.001;
        return strength / noise;
    }

    /// <summary>
    /// Calculate usable input (efficiency after noise penalty).
    /// </summary>
    public double GetUsableInput()
    {
        double snr = GetSignalToNoiseRatio();
        double processedSignal = (strength * strength) / (strength + noise);
        processedSignal *= quality;
        return processedSignal;
    }

    public void Age(float deltaTime, float globalDecayResistance = 0f)
    {
        if (deltaTime <= 0f) return;

        age += deltaTime;
        float effectiveDecay = Mathf.Max(0f, decayRate - globalDecayResistance);
        quality -= effectiveDecay * deltaTime;
        quality = Mathf.Clamp01(quality);
        extractedData = GetUsableInput();
    }

    /// <summary>
    /// Calculate processing cost multiplier based on complexity.
    /// </summary>
    public double GetProcessingCostMultiplier()
    {
        return 1.0 + complexity;
    }

    /// <summary>
    /// Calculate effective data output with all modifiers.
    /// </summary>
    public double GetEffectiveDataOutput()
    {
        double complexityPenalty = 1.0 / (1.0 + complexity * 0.25);
        return extractedData * patternMultiplier * complexityPenalty;
    }

    /// <summary>
    /// Create a copy of this signal.
    /// </summary>
    public Signal Clone()
    {
        return new Signal(strength, noise, complexity, decayRate, sourceName, sourceTier)
        {
            processingProgress = processingProgress,
            extractedData = extractedData,
            patternMultiplier = patternMultiplier,
            age = age,
            quality = quality
        };
    }

    public override string ToString()
    {
        return $"Signal[{sourceName}] S:{strength:F1} N:{noise:F1} C:{complexity:F2} Q:{quality:F2} SNR:{GetSignalToNoiseRatio():F2} Data:{GetEffectiveDataOutput():F2}";
    }
}
