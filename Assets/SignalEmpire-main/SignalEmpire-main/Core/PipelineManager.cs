using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all pipelines and signal sources in the game.
/// Coordinates signal generation, queuing, and pipeline processing.
/// Integrates with SignalEngine for tech tree upgrades.
/// </summary>
public class PipelineManager : MonoBehaviour
{
    public static PipelineManager instance;

    /// <summary>
    /// Dictionary of named pipelines.
    /// </summary>
    private Dictionary<string, Pipeline> pipelines = new Dictionary<string, Pipeline>();

    /// <summary>
    /// Dictionary of active signal sources.
    /// </summary>
    private Dictionary<string, SignalSource> signalSources = new Dictionary<string, SignalSource>();

    /// <summary>
    /// Reference to the signal engine for tech tree modifiers.
    /// </summary>
    private SignalEngine signalEngine;

    /// <summary>
    /// Get all pipelines for external access.
    /// </summary>
    public Dictionary<string, Pipeline>.ValueCollection GetPipelines()
    {
        return pipelines.Values;
    }

    /// <summary>
    /// Cached data generation rate for UI display.
    /// </summary>
    public double dataGenerationRate = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        signalEngine = SignalEngine.instance;
        if (signalEngine == null)
        {
            signalEngine = FindObjectOfType<SignalEngine>();
        }

        InitializeDefaultSources();
        InitializeDefaultPipelines();

        if (signalEngine == null)
        {
            Debug.LogWarning("PipelineManager: SignalEngine instance not found.");
        }
        else
        {
            foreach (var pipeline in pipelines.Values)
            {
                pipeline.engine = signalEngine;
                pipeline.baseProcessingSpeed = pipeline.processingSpeed;
                pipeline.SetProcessingMultiplier(signalEngine.speedMult * signalEngine.pipelineSpeedMultiplier);
                pipeline.computeAllocation *= 1f + signalEngine.travelTimeReduction;
            }

            ApplyTechModifiers();
            UpdatePipelineSpeeds();
        }
    }

    private void Update()
    {
        // Update all signal sources
        foreach (var source in signalSources.Values)
        {
            if (source.IsReadyToGenerate(Time.deltaTime))
            {
                Signal signal = source.GenerateSignal();
                if (signal != null)
                {
                    // Route signal to appropriate pipeline
                    RouteSignalToPipeline(signal);
                }
            }
        }

        // Update all pipelines
        foreach (var pipeline in pipelines.Values)
        {
            pipeline.Update(Time.deltaTime);
        }

        // Calculate data generation rate
        CalculateDataGenerationRate();
    }

    /// <summary>
    /// Initialize default signal sources based on game phase.
    /// </summary>
    private void InitializeDefaultSources()
    {
        SignalSource localSignals = SignalSources.LocalSignals();
        AddSignalSource(localSignals);

        SignalSource deepSpace = SignalSources.DeepSpaceSignals();
        deepSpace.isActive = false;
        AddSignalSource(deepSpace);

        SignalSource alienEncoding = SignalSources.AlienEncoding();
        alienEncoding.isActive = false;
        AddSignalSource(alienEncoding);

        SignalSource networkOptimization = SignalSources.NetworkOptimization();
        networkOptimization.isActive = false;
        AddSignalSource(networkOptimization);

        SignalSource universalConstants = SignalSources.UniversalConstants();
        universalConstants.isActive = false;
        AddSignalSource(universalConstants);
    }

    /// <summary>
    /// Initialize default pipelines.
    /// </summary>
    private void InitializeDefaultPipelines()
    {
        // Primary pipeline with all module types
        Pipeline primary = new Pipeline("Primary");
        primary.AddModule(new AmplifierModule());
        primary.AddModule(new NoiseFilterModule());
        primary.AddModule(new CompressorModule());
        primary.AddModule(new PatternDetectorModule());
        primary.AddModule(new DecoderModule());
        AddPipeline(primary);

        // Secondary pipeline (can be customized or left empty initially)
        Pipeline secondary = new Pipeline("Secondary");
        AddPipeline(secondary);
    }

    /// <summary>
    /// Add a pipeline to the manager.
    /// </summary>
    public void AddPipeline(Pipeline pipeline)
    {
        if (pipeline == null) return;
        pipelines[pipeline.pipelineName] = pipeline;
    }

    /// <summary>
    /// Get a pipeline by name.
    /// </summary>
    public Pipeline GetPipeline(string name)
    {
        if (pipelines.ContainsKey(name))
            return pipelines[name];
        return null;
    }

    /// <summary>
    /// Add a signal source to the manager.
    /// </summary>
    public void AddSignalSource(SignalSource source)
    {
        if (source == null) return;
        signalSources[source.sourceName] = source;
    }

    /// <summary>
    /// Get a signal source by name.
    /// </summary>
    public SignalSource GetSignalSource(string name)
    {
        if (signalSources.ContainsKey(name))
            return signalSources[name];
        return null;
    }

    /// <summary>
    /// Activate a new signal source (unlocked via tech tree).
    /// </summary>
    public void UnlockSignalSource(string sourceType)
    {
        if (signalSources.ContainsKey(sourceType))
        {
            signalSources[sourceType].isActive = true;
            Debug.Log($"Signal source '{sourceType}' unlocked!");
        }
        else
        {
            SignalSource newSource = null;

            switch (sourceType)
            {
                case "Deep Space":
                    newSource = SignalSources.DeepSpaceSignals();
                    break;
                case "Alien Encoding":
                    newSource = SignalSources.AlienEncoding();
                    break;
                case "Network Optimization":
                    newSource = SignalSources.NetworkOptimization();
                    break;
                case "Universal Constants":
                    newSource = SignalSources.UniversalConstants();
                    break;
            }

            if (newSource != null)
            {
                AddSignalSource(newSource);
            }
        }
    }

    /// <summary>
    /// Unlock a new module type in a pipeline.
    /// </summary>
    public void UnlockModule(string pipelineName, string moduleType)
    {
        Pipeline pipeline = GetPipeline(pipelineName);
        if (pipeline == null) return;

        PipelineModule module = null;

        switch (moduleType)
        {
            case "Amplifier":
                module = new AmplifierModule();
                break;
            case "Filter":
                module = new NoiseFilterModule();
                break;
            case "Compressor":
                module = new CompressorModule();
                break;
            case "PatternDetector":
                module = new PatternDetectorModule();
                break;
            case "Decoder":
                module = new DecoderModule();
                break;
        }

        if (module != null)
        {
            pipeline.AddModule(module);
            Debug.Log($"Module '{moduleType}' added to pipeline '{pipelineName}'");
        }
    }

    /// <summary>
    /// Route a signal to the pipeline with the shortest queue.
    /// </summary>
    private void RouteSignalToPipeline(Signal signal)
    {
        if (pipelines.Count == 0) return;

        Pipeline targetPipeline = null;
        int shortestQueue = int.MaxValue;

        foreach (var pipeline in pipelines.Values)
        {
            if (pipeline == null) continue;
            int queueLength = pipeline.GetQueueLength();
            if (queueLength < shortestQueue)
            {
                shortestQueue = queueLength;
                targetPipeline = pipeline;
            }
        }

        if (targetPipeline != null)
        {
            targetPipeline.EnqueueSignal(signal);
        }
    }

    /// <summary>
    /// Calculate the current signal generation rate (signals per second).
    /// </summary>
    private void CalculateDataGenerationRate()
    {
        double totalRate = 0;

        foreach (var source in signalSources.Values)
        {
            if (source.isActive)
            {
                totalRate += source.generationRate;
            }
        }

        dataGenerationRate = totalRate;
    }

    public double GetEstimatedPipelineThroughput()
    {
        double totalSpeed = 0;
        int pipelineCount = 0;

        foreach (var pipeline in pipelines.Values)
        {
            totalSpeed += pipeline.processingSpeed;
            pipelineCount++;
        }

        if (pipelineCount == 0) return 0;
        return dataGenerationRate * (totalSpeed / pipelineCount);
    }

    public float GetPrimarySignalQuality()
    {
        if (pipelines.TryGetValue("Primary", out Pipeline primary))
        {
            return primary.GetCurrentSignalQuality();
        }
        return 0f;
    }

    /// <summary>
    /// Apply tech tree modifiers to pipeline modules.
    /// Called by SignalEngine when tech nodes are unlocked.
    /// </summary>
    public void ApplyTechModifiers()
    {
        if (signalEngine == null) return;

        foreach (var pipeline in pipelines.Values)
        {
            foreach (var module in pipeline.modules)
            {
                module.ApplyTechModifiers(signalEngine);
            }
        }
    }

    public void UpdatePipelineSpeeds()
    {
        if (signalEngine == null) return;

        foreach (var pipeline in pipelines.Values)
        {
            pipeline.SetProcessingMultiplier(signalEngine.speedMult * signalEngine.pipelineSpeedMultiplier);
        }
    }

    /// <summary>
    /// Get diagnostic info for all pipelines.
    /// </summary>
    public string GetPipelineStatus()
    {
        string status = "=== PIPELINE MANAGER ===\n";

        foreach (var pipeline in pipelines.Values)
        {
            status += pipeline.GetStatus() + "\n\n";
        }

        return status;
    }

    /// <summary>
    /// Get diagnostic info for all signal sources.
    /// </summary>
    public string GetSignalSourceStatus()
    {
        string status = "=== SIGNAL SOURCES ===\n";

        foreach (var source in signalSources.Values)
        {
            status += source.ToString() + "\n\n";
        }

        return status;
    }
}
