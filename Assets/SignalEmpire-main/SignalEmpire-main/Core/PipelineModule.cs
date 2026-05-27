using UnityEngine;

/// <summary>
/// Base class for pipeline modules. Each module transforms a signal's properties.
/// Order in the pipeline matters—outputs of one module become inputs to the next.
/// </summary>
public abstract class PipelineModule
{
    public string moduleName;
    public bool isUnlocked = true;
    public float efficiency = 1.0f; // 0-1, can be upgraded

    public PipelineModule(string name)
    {
        moduleName = name;
    }

    /// <summary>
    /// Apply engine-driven upgrades to this module.
    /// </summary>
    public virtual void ApplyTechModifiers(SignalEngine engine)
    {
        if (engine == null) return;
        efficiency = 1f + engine.amplifierBoost * 0.05f;
    }

    /// <summary>
    /// Process a signal through this module.
    /// </summary>
    public abstract void ProcessSignal(Signal signal);

    /// <summary>
    /// Get human-readable description of module state.
    /// </summary>
    public virtual string GetDescription()
    {
        return $"{moduleName} (Efficiency: {efficiency * 100:F0}%)";
    }
}

/// <summary>
/// Amplifier: Multiplies signal strength, but also increases noise.
/// Formula: S ×= (1 + A) and N ×= (1 + 0.25A)
/// where A is a parameter based on upgrade level.
/// </summary>
public class AmplifierModule : PipelineModule
{
    public float amplificationFactor = 0.5f; // Upgradeable

    public AmplifierModule() : base("Amplifier") { }

    public override void ApplyTechModifiers(SignalEngine engine)
    {
        base.ApplyTechModifiers(engine);
        if (engine == null) return;

        amplificationFactor = 0.5f + engine.amplifierBoost * 0.1f;
        if (engine.stableAmpsEnabled)
        {
            amplificationFactor += 0.15f;
        }
    }

    public override void ProcessSignal(Signal signal)
    {
        if (!isUnlocked) return;

        float A = amplificationFactor * efficiency;
        signal.strength *= (1.0 + A);
        signal.noise *= (1.0 + 0.25f * A);

        Debug.Log($"Amplifier: {A:F2}x amp → S×{1.0 + A:F2}, N×{1.0 + 0.25f * A:F2}");
    }

    public override string GetDescription()
    {
        return $"{moduleName}: +{amplificationFactor * 100:F0}% Strength (increases Noise by 25%)";
    }
}

/// <summary>
/// Noise Filter: Reduces noise but weakens signal slightly.
/// Formula: N ×= (1 - F) and S ×= (1 - 0.2F)
/// where F is filter strength.
/// </summary>
public class NoiseFilterModule : PipelineModule
{
    public float filterStrength = 0.5f; // Upgradeable, 0-1

    public NoiseFilterModule() : base("Noise Filter") { }

    public override void ApplyTechModifiers(SignalEngine engine)
    {
        base.ApplyTechModifiers(engine);
        if (engine == null) return;

        filterStrength = Mathf.Clamp01(0.5f + engine.noiseReduction * 0.15f + (engine.snrFilterBoost - 1f) * 0.1f);
    }

    public override void ProcessSignal(Signal signal)
    {
        if (!isUnlocked) return;

        float F = filterStrength * efficiency;
        F = Mathf.Clamp01(F);

        signal.noise *= (1.0 - F);
        signal.strength *= (1.0 - 0.2f * F);

        Debug.Log($"Filter: {F:F2} → N×{1.0 - F:F2}, S×{1.0 - 0.2f * F:F2}");
    }

    public override string GetDescription()
    {
        return $"{moduleName}: -{filterStrength * 100:F0}% Noise (costs 20% Strength)";
    }
}

/// <summary>
/// Compressor: Converts usable signal into extractable data.
/// Output scales logarithmically with SNR, rewarding high signal-to-noise ratios.
/// Formula: Data ×= (1 + k × log(1 + S/N))
/// </summary>
public class CompressorModule : PipelineModule
{
    public float compressionRatio = 1.0f; // Upgradeable

    public CompressorModule() : base("Compressor") { }

    public override void ApplyTechModifiers(SignalEngine engine)
    {
        base.ApplyTechModifiers(engine);
        if (engine == null) return;

        compressionRatio = 1.0f + engine.compressionEfficiency + (float)(engine.infoValueMult * 0.05);
        if (engine.nonLinearMultiplierUnlocked)
        {
            compressionRatio += 0.5f;
        }
    }

    public override void ProcessSignal(Signal signal)
    {
        if (!isUnlocked) return;

        double snr = signal.GetSignalToNoiseRatio();
        double k = compressionRatio * efficiency;

        double compressionBonus = 1.0 + k * System.Math.Log(1.0 + snr);
        signal.extractedData *= compressionBonus;

        Debug.Log($"Compressor: SNR {snr:F2} → Data ×{compressionBonus:F2}");
    }

    public override string GetDescription()
    {
        return $"{moduleName}: Data scales with SNR (×log modifier)";
    }
}

/// <summary>
/// Pattern Detector: Adds multiplicative bonuses based on SNR thresholds.
/// Encourages players to optimize for cleaner signals.
/// </summary>
public class PatternDetectorModule : PipelineModule
{
    public PatternDetectorModule() : base("Pattern Detector") { }

    public override void ApplyTechModifiers(SignalEngine engine)
    {
        base.ApplyTechModifiers(engine);
        if (engine == null) return;

        efficiency += engine.highSNRChanceBonus;
        if (engine.stackingValueBonuses)
        {
            efficiency += 0.25f;
        }
    }

    public override void ProcessSignal(Signal signal)
    {
        if (!isUnlocked) return;

        double snr = signal.GetSignalToNoiseRatio();
        double bonus = 1.0;

        if (snr >= 8.0) bonus = 4.0;
        else if (snr >= 5.0) bonus = 3.0;
        else if (snr >= 2.0) bonus = 1.6;
        else bonus = 1.0;

        bonus *= efficiency;
        if (signal.sourceTier >= 4)
        {
            bonus += 0.2;
        }

        signal.patternMultiplier *= bonus;
        Debug.Log($"Pattern Detector: SNR {snr:F2} → Multiplier ×{bonus:F2}");
    }

    public override string GetDescription()
    {
        return $"{moduleName}: Higher SNR gives non-linear pattern bonuses.";
    }
}

/// <summary>
/// Decoder: Reduces complexity penalty, making harder signals easier to process.
/// Formula: C ×= (1 - D)
/// where D is decoder strength.
/// </summary>
public class DecoderModule : PipelineModule
{
    public float decoderStrength = 0.5f; // Upgradeable

    public DecoderModule() : base("Decoder") { }

    public override void ApplyTechModifiers(SignalEngine engine)
    {
        base.ApplyTechModifiers(engine);
        if (engine == null) return;

        decoderStrength = Mathf.Clamp01(0.5f + engine.complexityReduction * 0.1f);
        if (engine.chronoTuningEnabled)
        {
            decoderStrength = Mathf.Clamp01(decoderStrength + 0.15f);
        }
    }

    public override void ProcessSignal(Signal signal)
    {
        if (!isUnlocked) return;

        float D = decoderStrength * efficiency;
        D = Mathf.Clamp01(D);

        signal.complexity *= (1.0 - D);
        signal.noise *= (1.0 - 0.05f * D);

        Debug.Log($"Decoder: -{D * 100:F0}% Complexity → C×{1.0 - D:F2}, Noise×{1.0 - 0.05f * D:F2}");
    }

    public override string GetDescription()
    {
        return $"{moduleName}: Reduces Complexity and prunes noise.";
    }
}
