using UnityEngine;
using UnityEngine.UI; // Required for Slider
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    // A shorthand property to reach the Engine's global instance
    private SignalEngine engine => SignalEngine.instance;

    [Header("Global Resource Displays")]
    public TextMeshProUGUI dataDisplayTM;
    public TextMeshProUGUI ppDisplayTM;
    public Slider processingBar;
    public TextMeshProUGUI progressPercentText; 

    [Header("UI Cost Labels")]
    public TextMeshProUGUI tapeCostText;
    public TextMeshProUGUI processingCostText; 
    public TextMeshProUGUI clarityCostText;

    void Start()
    {
        if (SignalEngine.instance == null)
        {
            Debug.LogError("UpgradeManager: SignalEngine instance not found!");
        }
    }

    void Update()
    {
        if (engine == null) return;

        // --- 1. UPDATE GLOBAL DISPLAYS ---
        if (dataDisplayTM != null)
            dataDisplayTM.text = "DATA: " + engine.currentData.ToString("N0");

        if (ppDisplayTM != null)
            ppDisplayTM.text = "PP: " + engine.totalPowerPoints.ToString("N0");

        // --- 2. UPDATE PROCESSING BAR & PERCENTAGE ---
        if (processingBar != null)
        {
            // Calculate the 0-1 value for the slider (Engine target is 100f)
            float progressFraction = Mathf.Clamp01(engine.currentProgress / 100f);
            processingBar.value = progressFraction;

            // Update the percentage text (P0 formats 0.5 as "50%")
            if (progressPercentText != null)
            {
                progressPercentText.text = progressFraction.ToString("P0");
            }
        }

        // --- 3. UPDATE UPGRADE COSTS & COLORS ---
        UpdateUpgradeUI(tapeCostText, EconomyDefinitions.CalculateCost(engine.tapeLevel, 10, 1.15));
        UpdateUpgradeUI(processingCostText, engine.GetProcessingUpgradeCost());
        UpdateUpgradeUI(clarityCostText, engine.GetClarityUpgradeCost());
    }

    // Helper to set text and change color if unaffordable
    private void UpdateUpgradeUI(TextMeshProUGUI label, double cost)
    {
        if (label == null) return;
        label.text = "Cost: " + cost.ToString("N0");
        label.color = (engine.currentData >= cost) ? Color.white : Color.red;
    }

    // --- BUTTON FUNCTIONS ---

    public void UpgradeTapeValue()
    {
        if (engine == null)
        {
            Debug.LogError("UpgradeManager: SignalEngine not initialized!");
            return;
        }
        double cost = EconomyDefinitions.CalculateCost(engine.tapeLevel, 10, 1.15);
        if (engine.currentData >= cost)
        {
            engine.currentData -= cost;
            engine.tapeLevel++;

            engine.dataMult *= 1.25; // +25% Data yield
            Debug.Log($"Tape Data Upgraded! New Multiplier: {engine.dataMult}x");
        }
    }

    public void UpgradeSpeed()
    {
        if (engine == null)
        {
            Debug.LogError("UpgradeManager: SignalEngine not initialized!");
            return;
        }
        double cost = EconomyDefinitions.CalculateCost(engine.speedLevel, 50, 1.8);
        if (engine.currentData >= cost)
        {
            engine.currentData -= cost;
            engine.speedLevel++;

            engine.speedMult *= 1.15f; // +15% Speed
            Debug.Log($"Processing Speed Upgraded! New Speed: {engine.speedMult}x");
        }
    }

    public void UpgradeSignalSensitivity()
    {
        if (engine == null)
        {
            Debug.LogError("UpgradeManager: SignalEngine not initialized!");
            return;
        }
        double cost = EconomyDefinitions.CalculateCost(engine.sensitivityLevel, 100, 2.0);
        if (engine.currentData >= cost)
        {
            engine.currentData -= cost;
            engine.sensitivityLevel++;
            engine.rareSignalChance += 0.01f;
        }
    }

    // --- PIPELINE UPGRADES ---

    public void UpgradeProcessingSpeed()
    {
        if (engine == null)
        {
            Debug.LogError("UpgradeManager: SignalEngine not initialized!");
            return;
        }
        if (engine.UpgradeProcessingLevel()) 
            Debug.Log("Processing Speed Upgraded.");
    }

    public void UpgradeClarity()
    {
        if (engine == null)
        {
            Debug.LogError("UpgradeManager: SignalEngine not initialized!");
            return;
        }
        if (engine.UpgradeClarityLevel()) 
            Debug.Log("Signal Clarity Upgraded.");
    }

    public void UpgradeCompression()
    {
        if (engine == null)
        {
            Debug.LogError("UpgradeManager: SignalEngine not initialized!");
            return;
        }
        if (engine.UpgradeCompressionLevel()) 
            Debug.Log("Compression Upgraded.");
    }

    public void UpgradeDecayResistance()
    {
        if (engine == null)
        {
            Debug.LogError("UpgradeManager: SignalEngine not initialized!");
            return;
        }
        if (engine.UpgradeDecayResistanceLevel()) 
            Debug.Log("Decay Resistance Upgraded.");
    }

    // --- MINING UPGRADES ---

    public void UpgradeMinePower()
    {
        if (engine == null)
        {
            Debug.LogError("UpgradeManager: SignalEngine not initialized!");
            return;
        }
        if (engine.miningController != null && engine.miningController.UpgradeMinePowerLevel())
            Debug.Log("Mine Power Upgraded.");
    }

    public void UpgradeMineSpeed()
    {
        if (engine == null)
        {
            Debug.LogError("UpgradeManager: SignalEngine not initialized!");
            return;
        }
        if (engine.miningController != null && engine.miningController.UpgradeMineSpeedLevel())
            Debug.Log("Mine Speed Upgraded.");
    }

    public void UpgradeSignalReceiver()
    {
        if (engine.UpgradeCurrentPlanetReceiver())
            Debug.Log("Signal Receiver Upgraded.");
    }

    // --- DATA GETTERS ---

    public double GetProcessingUpgradeCost() => engine.GetProcessingUpgradeCost();
    public double GetClarityUpgradeCost() => engine.GetClarityUpgradeCost();
    public double GetCompressionUpgradeCost() => engine.GetCompressionUpgradeCost();
    public double GetDecayResistanceUpgradeCost() => engine.GetDecayResistanceUpgradeCost();
    
    public double GetMinePowerUpgradeCost() => (engine.miningController != null) ? engine.miningController.GetMinePowerUpgradeCost() : 0.0;
    public double GetMineSpeedUpgradeCost() => (engine.miningController != null) ? engine.miningController.GetMineSpeedUpgradeCost() : 0.0;

    public int GetProcessingLevel() => engine.GetProcessingLevel();
    public int GetClarityLevel() => engine.GetClarityLevel();
    public int GetCompressionLevel() => engine.GetCompressionLevel();
    public int GetDecayResistanceLevel() => engine.GetDecayResistanceLevel();
    
    public int GetMinePowerLevel() => (engine.miningController != null) ? engine.miningController.minePowerLevel : 0;
    public int GetMineSpeedLevel() => (engine.miningController != null) ? engine.miningController.mineSpeedLevel : 0;
    
    public int GetSignalReceiverLevel() => (engine.miningController != null && engine.miningController.currentPlanet != null) ? engine.miningController.currentPlanet.signalReceiverLevel : 0;
    
    public float GetSignalDecayResistancePercent() => engine.GetSignalDecayResistancePercent();
    public float GetExtractionMultiplier() => engine.GetExtractionMultiplier();

    public void BulkUpgradeTapeValue(int amount)
    {
        double cost = EconomyDefinitions.GetBulkCost(amount, engine.tapeLevel, 10, 1.15);
        if (engine.currentData >= cost)
        {
            engine.currentData -= cost;
            engine.tapeLevel += amount;
        }
    }
}