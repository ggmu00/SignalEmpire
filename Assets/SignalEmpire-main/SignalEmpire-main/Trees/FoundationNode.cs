using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoundationNode : MonoBehaviour
{
    private SignalEngine engine => SignalEngine.instance;

    [Header("Node Settings")]
    public string nodeID;   // Matches the 'id' in FoundationTree (e.g., "y_1")
    public string nodeName; // Matches the 'case' in the switch statement (e.g., "DataYield1")
    public double cost;
    public bool isUnlocked = false;

    [Header("UI References")]
    public Button nodeButton;
    public TextMeshProUGUI statusText; 

    public TMP_Text globalDescriptionText; // Drag the NodeDescription TMP Text component here

    private FoundationTree tree;
    private static FoundationNode currentlySelectedNode = null; 

    void OnValidate()
    {
        if (nodeButton == null) nodeButton = GetComponent<Button>();
        if (statusText == null) statusText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        tree = GetComponentInParent<FoundationTree>();
        if (tree == null) 
            tree = FindFirstObjectByType<FoundationTree>();

        if (nodeButton != null)
        {
            nodeButton.onClick.RemoveAllListeners();
            nodeButton.onClick.AddListener(HandleNodeClick);
        }

        UpdateNodeVisuals();
    }

    void Update()
    {
        if (engine == null || tree == null || nodeButton == null) return;

        if (tree.IsNodePurchased(nodeID))
        {
            isUnlocked = true;
        }

        UpdateNodeVisuals();
    }

    public void HandleNodeClick()
    {
        if (tree == null) return;

        if (isUnlocked)
        {
            DisplayDescription();
            return;
        }

        bool isNewSelection = currentlySelectedNode != this;

        if (currentlySelectedNode != null && currentlySelectedNode != this)
        {
            currentlySelectedNode.UpdateNodeVisuals();
        }

        currentlySelectedNode = this;
        DisplayDescription();
        UpdateNodeVisuals();

        if (isNewSelection)
        {
            // First click selects the node.
            return;
        }

        // Second click on the same node attempts purchase.
        if (!tree.ArePrerequisitesMet(nodeID))
        {
            if (globalDescriptionText != null)
                globalDescriptionText.text = $"<b>{statusText.text.Split('\n')[0]}</b>\nPrerequisites not met.";
            return;
        }

        if (engine == null)
        {
            return;
        }

        if (engine.totalPowerPoints < cost)
        {
            if (globalDescriptionText != null)
                globalDescriptionText.text = $"<b>{statusText.text.Split('\n')[0]}</b>\nNot enough PP to purchase.";
            return;
        }

        engine.totalPowerPoints -= cost;
        isUnlocked = true;
        ApplyNodeBonus();
        tree.MarkAsPurchased(nodeID);
        currentlySelectedNode = null;
        if (globalDescriptionText != null) globalDescriptionText.text = "";
        UpdateNodeVisuals();
    }

    public void UpdateNodeVisuals()
    {
        if (statusText == null || tree == null) return;

        bool prereqsMet = tree.ArePrerequisitesMet(nodeID);
        TechNode backendNode = tree.nodes.Find(n => n.id == this.nodeID);
        string givenName = backendNode != null ? backendNode.title : nodeName;

        if (isUnlocked)
        {
            if (nodeButton != null) nodeButton.interactable = false;
            statusText.text = $"{givenName}\nACTIVE";
            statusText.color = new Color(0f, 0.5f, 0f);
            return;
        }

        if (currentlySelectedNode == this)
        {
            statusText.text = $"{givenName}\nSELECTED";
            statusText.color = Color.cyan;
            if (nodeButton != null) nodeButton.interactable = true;
            return;
        }

        statusText.color = prereqsMet ? Color.black : new Color(0.65f, 0.2f, 0.2f);
        statusText.text = prereqsMet ? $"{givenName}\nCost: {cost.ToString("N0")} PP" : $"{givenName}\nLOCKED";
        if (nodeButton != null) nodeButton.interactable = true;
    }

    private void DisplayDescription()
    {
        if (globalDescriptionText == null) return;

        TechNode backendNode = tree.nodes.Find(n => n.id == this.nodeID);
        string givenName = backendNode != null ? backendNode.title : nodeName;
        string description = backendNode != null ? backendNode.description : "No log data available.";

        globalDescriptionText.text = $"<b>{givenName}</b>\n{description}\n<color=yellow>[Click again to buy]</color>";
    }

    public void ApplyNodeBonus()
    {
        if (engine == null) return;
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