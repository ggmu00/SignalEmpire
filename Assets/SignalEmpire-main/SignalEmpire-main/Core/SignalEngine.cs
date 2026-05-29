using UnityEngine;

public class SignalEngine : MonoBehaviour
{
    public static SignalEngine instance;

    // --- FIX: ADDED MISSING COMPONENT REFERENCES ---
    public SaveSystem saveSystem;
    public PipelineManager pipelineManager;
    public MiningController miningController; 
    // -----------------------------------------------

    [Header("Current Resources")]
    public double currentData;
    public double totalPowerPoints;

    public float maxHeatCapacity = 100f;

    [Header("Pipeline Support")]
    public float noiseReduction = 0f;
    public float complexityReduction = 0f;
    public float extractionMultiplier = 1.0f;

    [Header("Active Stats")]
    public int tapeLevel = 1;
    public int speedLevel = 1;
    public int sensitivityLevel = 1;
    public float rareSignalChance = 0.05f;

    public int processingLevel = 1;
    public int clarityLevel = 1;
    public int compressionLevel = 1;
    public int decayResistanceLevel = 1;

    public float pipelineSpeedMultiplier = 1.0f;
    public float signalDecayResistance = 0f;
    public float precisionMultiplier = 1.0f;

    [Header("Tech Tree Modifiers")]
    public double dataMult = 1.0;
    public float speedMult = 1.0f;
    public float travelTimeReduction = 0f;
    public float amplifierBoost = 1.0f;
    public float mineralYieldMult = 1.0f;
    public float signalRangeMult = 1.0f;
    float actualMaxHeat = 0f;
    public bool stableAmpsEnabled = false;
    public bool canRunAtFullHeat = false;
    public bool heatCorruptionEnabled = true;
    public bool pureMineralDropsUnlocked = false;
    public bool industrialOverdriveUnlocked = false;
    public bool industrialOverdriveActive = false;
    public float industrialOverdriveFluxCost = 10f;
    public float flux = 0f;
    public float heatToFluxRate = 0f;
    public float currentHeat = 0f;

    public float baseNoiseFloor = 1.0f;
    public float highSNRChanceBonus = 0f;
    public float frequencyMatchMultiplier = 1.0f;
    public bool preventsStaticSpikes = false;
    public bool autoRemoveNoiseLoops = false;
    public float bufferDecayReduction = 0f;
    public float subZeroDataBonusPerHeatReduction = 0f;
    public float snrFilterBoost = 1.0f;

    public double infoValueMult = 1.0;
    public float blueprintFragmentDropRate = 0f;
    public float mathSignalMultiplier = 1.0f;
    public bool stackingValueBonuses = false;
    public float compressionEfficiency = 1.0f;
    public float pipelineSlotCostReduction = 0f;
    public bool nonLinearMultiplierUnlocked = false;
    public bool compressionCapRemoved = false;
    public bool unlocksSquaredOutput = false;
    public bool infiniteValueUnlocked = false;
    public float vcSiphonRate = 0f;

    public float rareSourceDiscoverySpeed = 0f;
    public float ancientSchematicsValue = 0f;
    public bool fragmentSynthesisEnabled = false;
    public bool tier4SignalUnlocked = false;
    public float fragmentCostReduction = 0f;
    public bool chronoTuningEnabled = false;
    public float planetaryDataBoost = 0f;

    [Header("Decay Logic")]
    public float baseDecayRate = 0.02f;
    private float currentSignalQuality = 1.0f;
    public float currentProgress = 0f; // Made public for UI access

    [Header("Signal Matching")]
    public SignalMatchingUI signalMatchingUI;
    public bool signalMatchingAutomationUnlocked = false;

    [Header("Tree & UI Support")]
    public bool useSpeedAsDataBonus = false;
    public float tapeResetDelay = 0f;
    public float instantThreshold = 0f;
    public float rareChance = 0.05f;
    public float rareMultiplier = 2.0f;
    public bool unlockAnomalous = false;
    public bool autoSeekerTargetsBest = false;
    public float offlineEfficiency = 0.1f;
    public float offlineDataBonusPerHour = 0f;

    [Header("Foundation Tree Support")]
    public float autoSeekerSpeedPerData = 0f;
    public bool rareSignalsGrantPP = false;
    public bool canAccessPlanets = false;
    public float heatCapacityMult = 1.0f;

    // Add these methods at the bottom
    public int GetProcessingLevel() => processingLevel;
    public int GetClarityLevel() => clarityLevel;

    [Header("Final Script Support")]
    public double powerPoints; // Not 'powerPoints' previously? Added for FoundationTree.
    public float totalDataLifetime = 60f;
    public float upgradeCostReductionPerANode = 0f;
    public bool enableSpeedBurst = false;

    // Add these methods at the bottom of SignalEngine
    public double GetTapeValue() => tapeLevel; // Or whatever your tape value logic is
    public float GetCurrentSignalQualityPercent() => currentSignalQuality * 100f;
    public float GetSignalDecayResistancePercent() => signalDecayResistance * 100f;

    public void ProcessMatchedSignal(Signal signal) {
        // Leave empty for now or add your signal completion logic
        Debug.Log("Signal Processed: " + signal.name);
    }

    // Add these missing "Get" functions for your SignalMetricsPanel
    public int GetCompressionLevel() => compressionLevel;
    public int GetDecayResistanceLevel() => decayResistanceLevel;

    void Awake()
    {
        // Initialize Singleton
        if (instance == null) instance = this;
    }

    void Start()
    {
        // --- FIX: FIND THE MISSING COMPONENTS ---
        saveSystem = GetComponent<SaveSystem>();
        miningController = FindObjectOfType<MiningController>();
        pipelineManager = FindObjectOfType<PipelineManager>();
        
        if (saveSystem != null) saveSystem.LoadGame();

        RecalculatePipelineBonuses();

        if (pipelineManager != null)
        {
            pipelineManager.ApplyTechModifiers();
        }

        InvokeRepeating(nameof(AutoSave), 60f, 60f);
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        
        // Default values if no planet is unlocked yet
        float planetDecayMod = 1.0f; 
        float receiverFloor = 0.1f;

        // SAFETY CHECK: Only access planet data if it exists
        if (miningController != null && miningController.currentPlanet != null)
        {
            planetDecayMod = miningController.currentPlanet.decayModifier;
            receiverFloor = miningController.currentPlanet.GetReceiverQualityFloor();
            
            // This ensures mining logic only runs on a planet
            miningController.ProcessMining(deltaTime);
        }

        // This part now runs safely regardless of planets
        float totalSpeed = 10f * (1f + (speedLevel * 0.2f)) * speedMult * (1f + travelTimeReduction) * amplifierBoost;

        currentProgress += totalSpeed * deltaTime;
        currentSignalQuality -= (baseDecayRate * planetDecayMod) * deltaTime;
        currentSignalQuality = Mathf.Clamp(currentSignalQuality, receiverFloor, 1.0f);

        GenerateFluxFromHeat(deltaTime);

        if (currentProgress >= 100f)
        {
            ProcessPlanetaryCycle();
        }
    }

    // ... [Rest of your methods: AddExtractedData, Upgrade methods, etc. stay exactly the same] ...

    public void AddExtractedData(double amount)
    {
        double scaledAmount = amount * infoValueMult * dataMult * extractionMultiplier;
        if (unlocksSquaredOutput) scaledAmount *= 1.1;
        if (infiniteValueUnlocked) scaledAmount *= 1.25;
        currentData += scaledAmount;
        if (vcSiphonRate > 0f) totalPowerPoints += scaledAmount * vcSiphonRate;
    }

    public double GetProcessingUpgradeCost() => EconomyDefinitions.CalculateCost(processingLevel, 20, 1.3);
    public double GetClarityUpgradeCost() => EconomyDefinitions.CalculateCost(clarityLevel, 30, 1.25);
    public double GetCompressionUpgradeCost() => EconomyDefinitions.CalculateCost(compressionLevel, 40, 1.2);
    public double GetDecayResistanceUpgradeCost() => EconomyDefinitions.CalculateCost(decayResistanceLevel, 50, 1.2);

    public bool UpgradeProcessingLevel()
    {
        double cost = GetProcessingUpgradeCost();
        if (currentData >= cost)
        {
            currentData -= cost;
            processingLevel++;
            pipelineSpeedMultiplier = 1.0f + (processingLevel - 1) * 0.10f;
            RecalculatePipelineBonuses();
            return true;
        }
        return false;
    }

    public bool UpgradeClarityLevel()
    {
        double cost = GetClarityUpgradeCost();
        if (currentData >= cost)
        {
            currentData -= cost;
            clarityLevel++;
            noiseReduction = clarityLevel * 0.08f;
            complexityReduction = clarityLevel * 0.05f;
            RecalculatePipelineBonuses();
            return true;
        }
        return false;
    }

    public bool UpgradeCompressionLevel()
    {
        double cost = GetCompressionUpgradeCost();
        if (currentData >= cost)
        {
            currentData -= cost;
            compressionLevel++;
            extractionMultiplier = 1.0f + (compressionLevel - 1) * 0.12f;
            RecalculatePipelineBonuses();
            return true;
        }
        return false;
    }

    public bool UpgradeDecayResistanceLevel()
    {
        double cost = GetDecayResistanceUpgradeCost();
        if (currentData >= cost)
        {
            currentData -= cost;
            decayResistanceLevel++;
            signalDecayResistance = Mathf.Min(0.5f, decayResistanceLevel * 0.04f);
            RecalculatePipelineBonuses();
            return true;
        }
        return false;
    }

    public bool UpgradeCurrentPlanetReceiver()
    {
        if (miningController == null || miningController.currentPlanet == null) return false;
        PlanetData planet = miningController.currentPlanet;
        double cost = EconomyDefinitions.CalculateCost(planet.signalReceiverLevel, 100, 1.3);
        if (currentData >= cost)
        {
            currentData -= cost;
            planet.signalReceiverLevel++;
            return true;
        }
        return false;
    }

    public void RecalculatePipelineBonuses()
    {
        pipelineSpeedMultiplier = 1.0f + (processingLevel - 1) * 0.10f;
        noiseReduction = clarityLevel * 0.08f;
        complexityReduction = clarityLevel * 0.05f;
        extractionMultiplier = 1.0f + (compressionLevel - 1) * 0.12f;
        signalDecayResistance = Mathf.Min(0.5f, decayResistanceLevel * 0.04f);

        if (pipelineManager != null)
        {
            pipelineManager.ApplyTechModifiers();
            pipelineManager.UpdatePipelineSpeeds();
        }
    }

    private void ProcessPlanetaryCycle()
    {
        SignalTier tier = RollForTier();
        
        // Default values if no planet is present
        double strengthMult = 1.0;
        double receiverBonus = 1.0;

        // Only try to get planet stats if the controller exists
        if (miningController != null && miningController.currentPlanet != null)
        {
            strengthMult = miningController.currentPlanet.signalStrengthMult;
            receiverBonus = miningController.currentPlanet.GetReceiverSignalBonus();
        }

        double finalPayout = EconomyDefinitions.CalculateFinalPayout(tier, tapeLevel, currentSignalQuality, dataMult * strengthMult) * receiverBonus;

        currentData += finalPayout;

        // Reset for next signal
        currentProgress = 0f;
        currentSignalQuality = 1.0f;
    }

    private void GenerateFluxFromHeat(float deltaTime)
    {
        if (heatToFluxRate <= 0f || currentHeat <= 0f) return;
        flux += (currentHeat / 10f) * heatToFluxRate * deltaTime;
    }

    private SignalTier RollForTier()
    {
        float roll = Random.value;
        if (roll <= 0.001f) return SignalTier.Zenith;
        if (roll <= 0.01f) return SignalTier.Anomalous;
        if (roll <= rareSignalChance) return SignalTier.Rare;
        return SignalTier.Common;
    }

    public float GetExtractionMultiplier() 
    {
        return extractionMultiplier;
    }

    void AutoSave() => saveSystem?.SaveGame();
    void OnApplicationQuit() => saveSystem?.SaveGame();
    void OnApplicationPause(bool pause) { if (pause) saveSystem?.SaveGame(); }
}