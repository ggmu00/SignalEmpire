using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoundationNode : MonoBehaviour
{
    private SignalEngine engine => SignalEngine.instance;

    [Header("Node Settings")]
    public string nodeID;   // Matches the 'id' in FoundationTree (e.g., "y_1", "s_1")
    public string nodeName; // Matches the 'case' in the switch statement
    public double cost;
    public bool isUnlocked = false;

    [Header("UI References")]
    public Button nodeButton;
    public TextMeshProUGUI statusText; 

    private FoundationTree tree;

    void Start()
    {
        // Find the tree manager in the parent object
        tree = GetComponentInParent<FoundationTree>();

        if (nodeButton != null)
            nodeButton.onClick.AddListener(AttemptUnlock);
    }

    void Update()
    {
        if (engine == null || nodeButton == null || tree == null) return;

        // Check if prerequisites from the Tree logic are met
        bool prereqsMet = tree.ArePrerequisitesMet(nodeID);

        if (!isUnlocked)
        {
            // Button is interactable only if affordable AND prerequisites are met
            bool canAfford = engine.totalPowerPoints >= cost;
            nodeButton.interactable = canAfford && prereqsMet;

            if (statusText != null)
            {
                if (!prereqsMet) statusText.text = "LOCKED";
                else statusText.text = "Cost: " + cost.ToString("N0") + " PP";
            }
        }
        else
        {
            nodeButton.interactable = false;
            if (statusText != null) statusText.text = "ACTIVE";
        }
    }

    public void AttemptUnlock()
    {
        if (isUnlocked || tree == null) return;

        // Double check prerequisites and cost
        if (tree.ArePrerequisitesMet(nodeID) && engine.totalPowerPoints >= cost)
        {
            engine.totalPowerPoints -= cost;
            isUnlocked = true;
            
            // 1. Apply the mechanical bonus
            ApplyNodeBonus();
            
            // 2. Tell the tree this ID is now purchased so it can unlock the next nodes
            tree.MarkAsPurchased(nodeName); 
            
            Debug.Log(nodeName + " (ID: " + nodeID + ") Unlocked!");
        }
    }

    private void ApplyNodeBonus()
    {
        // Use the exact switch logic you provided
        switch (nodeName)
        {
            case "AutoSeeker": engine.autoSeekerSpeedPerData = 0.5f; break;
            case "RarePP": engine.rareSignalsGrantPP = true; break;
            case "DataBoost": engine.dataMult += 0.5; break;
            case "SpeedBoost": engine.speedMult += 0.5f; break;
            case "SignalRange": engine.signalRangeMult += 0.5f; break;
            case "UpgradeDiscount": engine.upgradeCostReductionPerANode = 0.10f; break;
            case "OfflineBonus": engine.offlineEfficiency = 0.8f; break;
            case "OfflineData": engine.offlineDataBonusPerHour = 0.05f; break;
            case "OfflineMax": engine.offlineEfficiency = 1.0f; break;
            case "ChronosPulse": engine.enableSpeedBurst = true; break;
            case "EconomicAI": engine.autoSeekerSpeedPerData = 0.01f; break;
            case "VoidSiphon": engine.rareSignalsGrantPP = true; break;
            case "SignalAutomation": engine.signalMatchingAutomationUnlocked = true; break;
            case "ApexGateway": engine.canAccessPlanets = true; break;
            case "DataYield1": engine.dataMult *= 1.2; break;
            case "DataYield2": engine.dataMult *= 1.35; break;
            case "DataYield3": engine.dataMult *= 1.5; break;
            case "DataYield4": engine.dataMult *= 2.0; break;
            case "DataYield5": engine.useSpeedAsDataBonus = true; break;
            case "TapeSpeed1": engine.speedMult *= 1.15f; break;
            case "TapeSpeed2": engine.speedMult *= 1.30f; break;
            case "TapeSpeed3": engine.tapeResetDelay *= 0.5f; break;
            case "TapeSpeed4": engine.speedMult *= 2.0f; break;
            case "TapeSpeed5": engine.instantThreshold = 0.5f; break;
            case "RareChance1": engine.rareChance += 0.03f; break;
            case "RareChance2": engine.rareChance += 0.06f; break;
            case "RareMultiplier": engine.rareMultiplier = 20.0f; break;
            case "RareChance3": engine.rareChance += 0.15f; break;
            case "UnlockAnomalous": engine.unlockAnomalous = true; break;
            case "AutoSeekerTargeting": engine.autoSeekerTargetsBest = true; break;
            case "UpgradeCostReduction": engine.upgradeCostReductionPerANode = 0.10f; break;
            case "SpeedBurst": engine.enableSpeedBurst = true; break;
            case "AutoSeekerDataSpeed": engine.autoSeekerSpeedPerData = 0.01f; break;
            case "RareSignalsGrantPP": engine.rareSignalsGrantPP = true; break;
            case "CanAccessPlanets": engine.canAccessPlanets = true; break;
            case "SignalMatchingAutomation": engine.signalMatchingAutomationUnlocked = true; break;
        }
    }
}