using UnityEngine;
using UnityEngine.UI;

public class SignalMetricsPanel : MonoBehaviour
{
    public SignalEngine engine;
    public PipelineManager pipelineManager;

    [Header("Signal Metrics")]
    public Text signalQualityText;
    public Text decayResistanceText;
    public Text pipelineThroughputText;
    public Text extractionMultiplierText;

    [Header("Upgrade Levels")]
    public Text processingLevelText;
    public Text clarityLevelText;
    public Text compressionLevelText;
    public Text decayResistanceLevelText;

    private void Awake()
    {
        if (engine == null)
            engine = FindObjectOfType<SignalEngine>();

        if (pipelineManager == null)
            pipelineManager = PipelineManager.instance ?? FindObjectOfType<PipelineManager>();
    }

    private void Update()
    {
        if (engine == null) return;

        if (signalQualityText != null)
            signalQualityText.text = $"Signal Quality: {engine.GetCurrentSignalQualityPercent():F1}%";

        if (decayResistanceText != null)
            decayResistanceText.text = $"Decay Resistance: {engine.GetSignalDecayResistancePercent():F1}%";

        if (extractionMultiplierText != null)
            extractionMultiplierText.text = $"Extraction Multiplier: x{engine.GetExtractionMultiplier():F2}";

        if (pipelineThroughputText != null)
        {
            if (pipelineManager != null)
                pipelineThroughputText.text = $"Pipeline Throughput: {pipelineManager.GetEstimatedPipelineThroughput():F1} units/s";
            else
                pipelineThroughputText.text = "Pipeline Throughput: N/A";
        }

        if (processingLevelText != null)
            processingLevelText.text = $"Processing Level: {engine.GetProcessingLevel()}";

        if (clarityLevelText != null)
            clarityLevelText.text = $"Clarity Level: {engine.GetClarityLevel()}";

        if (compressionLevelText != null)
            compressionLevelText.text = $"Compression Level: {engine.GetCompressionLevel()}";

        if (decayResistanceLevelText != null)
            decayResistanceLevelText.text = $"Decay Resistance Level: {engine.GetDecayResistanceLevel()}";
    }
}
