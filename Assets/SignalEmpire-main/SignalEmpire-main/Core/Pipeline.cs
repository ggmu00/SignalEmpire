using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// A pipeline is a sequence of modules that processes signals in order.
/// Module order matters—outputs cascade through the chain.
/// </summary>
public class Pipeline
{
    public string pipelineName;
    public List<PipelineModule> modules = new List<PipelineModule>();

    /// <summary>
    /// Current signal being processed through this pipeline.
    /// </summary>
    public Signal currentSignal;

    /// <summary>
    /// Reference to the global signal engine for data delivery and tech modifiers.
    /// </summary>
    public SignalEngine engine;

    /// <summary>
    /// Processing speed (signals per second). Upgraded via tech tree.
    /// </summary>
    public float processingSpeed = 1.0f;
    public float baseProcessingSpeed = 1.0f;

    /// <summary>
    /// How much processing power this pipeline allocates per update.
    /// Higher = faster processing of current signal.
    /// </summary>
    public float computeAllocation = 1.0f;

    /// <summary>
    /// Queue of signals waiting to be processed.
    /// </summary>
    private Queue<Signal> signalQueue = new Queue<Signal>();

    /// <summary>
    /// Total data extracted by this pipeline over lifetime.
    /// </summary>
    public double totalDataExtracted = 0;

    public Pipeline(string name)
    {
        pipelineName = name;
        baseProcessingSpeed = processingSpeed;
    }

    public void SetProcessingMultiplier(float multiplier)
    {
        processingSpeed = baseProcessingSpeed * multiplier;
    }

    /// <summary>
    /// Add a module to this pipeline (appended to the end).
    /// </summary>
    public void AddModule(PipelineModule module)
    {
        if (module == null) return;
        modules.Add(module);
        Debug.Log($"Pipeline '{pipelineName}': Added {module.moduleName}");
    }

    /// <summary>
    /// Remove a module at specified index.
    /// </summary>
    public void RemoveModule(int index)
    {
        if (index >= 0 && index < modules.Count)
        {
            modules.RemoveAt(index);
        }
    }

    /// <summary>
    /// Queue a signal for processing.
    /// </summary>
    public void EnqueueSignal(Signal signal)
    {
        signalQueue.Enqueue(signal);
    }

    /// <summary>
    /// Process queued signals. Called each frame.
    /// </summary>
    public void Update(float deltaTime)
    {
        if (signalQueue.Count == 0 && currentSignal == null)
            return;

        // Age waiting signals before processing them.
        foreach (var queuedSignal in signalQueue.ToArray())
        {
            queuedSignal.Age(deltaTime, engine != null ? engine.signalDecayResistance : 0f);
        }

        // If no current signal, dequeue the next one
        if (currentSignal == null && signalQueue.Count > 0)
        {
            currentSignal = signalQueue.Dequeue();
            Debug.Log($"Pipeline '{pipelineName}': Starting signal {currentSignal}");
        }

        if (currentSignal == null)
            return;

        // Age the currently processing signal as well.
        currentSignal.Age(deltaTime, engine != null ? engine.signalDecayResistance : 0f);

        // Process the current signal
        float processingRate = processingSpeed * computeAllocation;
        currentSignal.processingProgress += processingRate * deltaTime;

        // When processing completes (100% progress), apply pipeline
        if (currentSignal.processingProgress >= 1.0f)
        {
            if (engine != null && !engine.signalMatchingAutomationUnlocked && engine.signalMatchingUI != null)
            {
                // Require manual matching before processing
                engine.signalMatchingUI.SetSignal(currentSignal);
                currentSignal.processingProgress = 0.99f; // Pause processing
            }
            else
            {
                CompleteSignalProcessing();
            }
        }
    }

    /// <summary>
    /// Process a signal completely through all modules in order.
    /// </summary>
    private void CompleteSignalProcessing()
    {
        if (currentSignal == null) return;

        Signal signal = currentSignal;

        // Pass signal through each module in sequence
        foreach (PipelineModule module in modules)
        {
            module.ProcessSignal(signal);
        }

        // Extract final data value
        double finalData = signal.GetEffectiveDataOutput();
        totalDataExtracted += finalData;

        if (engine != null)
        {
            engine.AddExtractedData(finalData);
        }

        Debug.Log($"Pipeline '{pipelineName}': Completed signal → {finalData:F2} data (Total: {totalDataExtracted:F2})");

        // Signal complete, move to next
        currentSignal = null;
    }

    /// <summary>
    /// Get the SNR of the current signal (useful for UI).
    /// </summary>
    public double GetCurrentSNR()
    {
        return currentSignal != null ? currentSignal.GetSignalToNoiseRatio() : 0;
    }

    public float GetCurrentSignalQuality()
    {
        return currentSignal != null ? currentSignal.quality * 100f : 0f;
    }

    public int GetQueueLength()
    {
        return signalQueue.Count;
    }

    /// <summary>
    /// Get processing progress as percentage.
    /// </summary>
    public float GetProcessingProgress()
    {
        return currentSignal != null ? currentSignal.processingProgress * 100f : 0;
    }

    /// <summary>
    /// Get human-readable pipeline status.
    /// </summary>
    public string GetStatus()
    {
        string status = $"Pipeline: {pipelineName}\n";
        status += $"Modules: {modules.Count}\n";

        foreach (var module in modules)
        {
            status += $"  • {module.GetDescription()}\n";
        }

        status += $"Queue: {signalQueue.Count} signals\n";

        if (currentSignal != null)
        {
            status += $"Processing: {currentSignal}\n";
            status += $"Progress: {GetProcessingProgress():F1}%\n";
        }
        else
        {
            status += "Status: Idle\n";
        }

        status += $"Total Extracted: {totalDataExtracted:F2} data";

        return status;
    }
}
