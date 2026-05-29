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
    
    /// <summary>Data yield multiplier modified by Foundation Tree (DataYield nodes)</summary>
    public double dataMult = 1.0;
    /// <summary>Processing speed multiplier modified by Foundation Tree (TapeSpeed nodes)</summary>
    public float speedMult = 1.0f;

    [Header("Tech Tree Modifiers")]
    /// <summary>Travel time reduction multiplier from Discipline Tree (Automated Routing)</summary>
    public float travelTimeReduction = 0f;
    /// <summary>Amplifier output boost from Discipline Tree (Stable Amps)</summary>
    public float amplifierBoost = 1.0f;
    /// <summary>Mineral yield multiplier from Discipline Tree (Heavy Drills)</summary>
    public float mineralYieldMult = 1.0f;
    /// <summary>Signal range multiplier from Foundation Tree (SignalRange node)</summary>
    public float signalRangeMult = 1.0f;
    float actualMaxHeat = 0f;
    public bool stableAmpsEnabled = false;
    public bool canRunAtFullHeat = false;
    public bool heatCorruptionEnabled = true;
    /// <summary>Pure mineral drops unlocked (Deep Vein Mining node)</summary>
    public bool pureMineralDropsUnlocked = false;
    /// <summary>Industrial overdrive system unlocked (Industrial Overdrive node)</summary>
    public bool industrialOverdriveUnlocked = false;
    /// <summary>Industrial overdrive currently active</summary>
    public bool industrialOverdriveActive = false;
    /// <summary>Flux cost to activate industrial overdrive</summary>
    public float industrialOverdriveFluxCost = 10f;
    public float flux = 0f;
    public float heatToFluxRate = 0f;
    public float currentHeat = 0f;

    public float baseNoiseFloor = 1.0f;
    public float highSNRChanceBonus = 0f;
    /// <summary>Frequency match multiplier (Harmonic Tuning node)</summary>
    public float frequencyMatchMultiplier = 1.0f;
    /// <summary>Prevents static spikes (Signal Isolation node)</summary>
    public bool preventsStaticSpikes = false;
    /// <summary>Auto remove noise loops (Pattern Filtration node)</summary>
    public bool autoRemoveNoiseLoops = false;
    /// <summary>Buffer decay reduction (Atmospheric Buffering node)</summary>
    public float bufferDecayReduction = 0f;
    /// <summary>Sub-zero data bonus per heat reduction</summary>
    public float subZeroDataBonusPerHeatReduction = 0f;
    public float snrFilterBoost = 1.0f;

    public double infoValueMult = 1.0;
    /// <summary>Blueprint fragment drop rate (Fragment Analysis node)</summary>
    public float blueprintFragmentDropRate = 0f;
    /// <summary>Math signal payout multiplier (Prime Sequence Detection node)</summary>
    public float mathSignalMultiplier = 1.0f;
    /// <summary>Stack value bonuses (Heuristic Learning node)</summary>
    public bool stackingValueBonuses = false;
    /// <summary>Compression efficiency (Dictionary Encoding node)</summary>
    public float compressionEfficiency = 1.0f;
    /// <summary>Pipeline slot cost reduction (Logic-Gate Optimization node)</summary>
    public float pipelineSlotCostReduction = 0f;
    /// <summary>Non-linear multiplier unlocked (Fractal Mapping node)</summary>
    public bool nonLinearMultiplierUnlocked = false;
    /// <summary>Compression cap removed (Lossless Mastery node)</summary>
    public bool compressionCapRemoved = false;
    /// <summary>Squared output unlocked (Zero-Floor Protocol node)</summary>
    public bool unlocksSquaredOutput = false;
    /// <summary>Infinite value unlocked (Singularity Compression node)</summary>
    public bool infiniteValueUnlocked = false;
    /// <summary>Void credit siphon rate (Void Credit Siphoning node)</summary>
    public float vcSiphonRate = 0f;

    /// <summary>Rare source discovery speed (artifact tech nodes)</summary>
    public float rareSourceDiscoverySpeed = 0f;
    /// <summary>Ancient schematics value multiplier (Xeno-Archaeology node)</summary>
    public float ancientSchematicsValue = 0f;
    /// <summary>Fragment synthesis enabled (Ancient Artifact node)</summary>
    public bool fragmentSynthesisEnabled = false;
    /// <summary>Tier 4 signals unlocked (Cosmic Substrate node)</summary>
    public bool tier4SignalUnlocked = false;
    /// <summary>Fragment cost reduction (Blueprint Stabilization node)</summary>
    public float fragmentCostReduction = 0f;
    /// <summary>Chrono-tuning enabled (Chrono-Tuning node)</summary>
    public bool chronoTuningEnabled = false;
    /// <summary>Planetary data boost (Interstellar Networking node)</summary>
    public float planetaryDataBoost = 0f;

    [Header("Decay Logic")]
    public float baseDecayRate = 0.02f;
    private float currentSignalQuality = 1.0f;
    public float currentProgress = 0f; // Made public for UI access

    [Header("Signal Matching")]
    public SignalMatchingUI signalMatchingUI;
    /// <summary>Auto signal matching unlocked (SignalAutomation/SignalMatchingAutomation nodes)</summary>
    public bool signalMatchingAutomationUnlocked = false;

    [Header("Tree & UI Support")]
    /// <summary>Apply speed multiplier as bonus to data yield (DataYield5 node)</summary>
    public bool useSpeedAsDataBonus = false;
    /// <summary>Multiplier applied to tape reset delay (TapeSpeed3 node)</summary>
    public float tapeResetDelay = 0f;
    /// <summary>Threshold below which tapes complete instantly (TapeSpeed5 node)</summary>
    public float instantThreshold = 0f;
    /// <summary>Base chance for rare signal discovery (RareChance1-3 nodes)</summary>
    public float rareChance = 0.05f;
    /// <summary>Payout multiplier for rare signals (RareMultiplier node)</summary>
    public float rareMultiplier = 2.0f;
    /// <summary>Unlock anomalous signal type (UnlockAnomalous node)</summary>
    public bool unlockAnomalous = false;
    /// <summary>Auto-seeker prioritizes highest-value targets (AutoSeekerTargeting node)</summary>
    public bool autoSeekerTargetsBest = false;
    /// <summary>Efficiency of offline processing (OfflineBonus/Max nodes)</summary>
    public float offlineEfficiency = 0.1f;
    /// <summary>Bonus data per hour offline (OfflineData node)</summary>
    public float offlineDataBonusPerHour = 0f;

    [Header("Foundation Tree Support")]
    /// <summary>Auto-seeker speed bonus per 10k data (AutoSeekerDataSpeed nodes)</summary>
    public float autoSeekerSpeedPerData = 0f;
    /// <summary>Rare signals grant power points (RareSignalsGrantPP/VoidSiphon nodes)</summary>
    public bool rareSignalsGrantPP = false;
    /// <summary>Unlock planetary system access (CanAccessPlanets/ApexGateway nodes)</summary>
    public bool canAccessPlanets = false;
    /// <summary>Heat capacity multiplier for Foundation Tree upgrades</summary>
    public float heatCapacityMult = 1.0f;

    // Add these methods at the bottom
    public int GetProcessingLevel() => processingLevel;
    public int GetClarityLevel() => clarityLevel;

    [Header("Final Script Support")]
    public double powerPoints; // Not 'powerPoints' previously? Added for FoundationTree.
    public float totalDataLifetime = 60f;
    /// <summary>Cost reduction per 'A' node in Foundation Tree (UpgradeCostReduction nodes)</summary>
    public float upgradeCostReductionPerANode = 0f;
    /// <summary>Speed burst effect enabled (ChronosPulse/SpeedBurst/EnableSpeedBurst nodes)</summary>
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