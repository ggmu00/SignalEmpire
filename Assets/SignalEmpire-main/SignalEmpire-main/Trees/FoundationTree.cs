using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FoundationTree : MonoBehaviour
{
    private SignalEngine engine => SignalEngine.instance;
    
    [Header("Tree Data")]
    public List<TechNode> nodes = new List<TechNode>();    
    [Header("Managers")]
    public LayerUnlockManager layerUnlockManager;

    void Awake()
    {
        InitializeFullTree();
    }

    private void InitializeFullTree()
    {
        nodes.Clear();

        // --- BRANCH: DATA ARCHITECT (YIELD) ---
        AddNode("y_1", "DataYield1", "Iron Oxide Coating", "+20% Data yield.", 10, "");
        AddNode("y_2", "DataYield2", "Bit-Packing", "+35% Data yield.", 50, "y_1");
        AddNode("y_3", "DataYield3", "Lossless Encoding", "+50% Data yield.", 150, "y_2");
        AddNode("y_4", "DataYield4", "Neural Compression", "+100% Data yield.", 600, "y_3");
        AddNode("y_5", "DataYield5", "Quantum Storage", "Yield x Tape Speed.", 2000, "y_4");

        // --- BRANCH: KINETICIST (SPEED) ---
        AddNode("s_1", "TapeSpeed1", "Polished Spools", "+15% Speed.", 15, "y_1");
        AddNode("s_2", "TapeSpeed2", "High-Torque Motors", "+30% Speed.", 75, "s_1");
        AddNode("s_3", "TapeSpeed3", "Dual-Reel Sync", "-50% reset delay.", 200, "s_2");
        AddNode("s_4", "TapeSpeed4", "Vacuum Chamber", "+100% Speed.", 800, "s_3");
        AddNode("s_5", "TapeSpeed5", "Tachyon Drive", "Instant < 0.5s tapes.", 2500, "s_4");

        // --- BRANCH: CHANCELLOR (LUCK) ---
        AddNode("l_1", "RareChance1", "Static Filtering", "+3% Rare Chance.", 25, "y_1");
        AddNode("l_2", "RareChance2", "Signal Resonance", "+6% Rare Chance.", 110, "l_1");
        AddNode("l_3", "RareMultiplier", "Prism Lens", "Rare signals 20x.", 300, "l_2");
        AddNode("l_4", "RareChance3", "Void Magnet", "+15% Rare Chance.", 1000, "l_3");
        AddNode("l_5", "UnlockAnomalous", "Anomalous Horizon", "Unlocks Anomalous.", 3000, "l_4");

        // --- BRANCH: AUTOMATON (EFFICIENCY) ---
        AddNode("a_1", "AutoSeekerTargeting", "Smart Seek", "Target high value.", 40, "s_1");
        AddNode("a_2", "OfflineBonus", "Background Scrub", "80% Offline Eff.", 150, "a_1");
        AddNode("a_3", "OfflineData", "Buffer Array", "5% bonus/hour offline.", 500, "a_2");
        AddNode("a_4", "OfflineMax", "Infinite Loop", "100% Offline Eff.", 1500, "a_3");
        AddNode("a_5", "UpgradeCostReduction", "Deep Learning", "-10% cost per 'A' node.", 4000, "a_4");

        // --- SYNERGY  ---
        AddNode("syn_1", "SpeedBurst", "Chronos Pulse", "Rare signal speed burst.", 1200, "s_3,l_3");
        AddNode("syn_2", "AutoSeekerDataSpeed", "Economic AI", "Speed scales with Data.", 1500, "y_3,a_3");
        AddNode("syn_3", "RareSignalsGrantPP", "Void Siphon", "Rare signals grant PP.", 5000, "l_4,a_4");

        // --- THE APEX GATEWAY ---
        AddNode("apex", "CanAccessPlanets", "Apex Gateway", "Unlock Planetary Access.", 10000, "syn_3");
        AddNode("auto", "SignalMatchingAutomation", "Signal Automation", "Auto Signal Matching.", 15000, "apex");
    }

    private void AddNode(string id, string caseName, string title, string desc, double cost, string preReqs)
    {
        nodes.Add(new TechNode
        {
            id = id,
            nodeName = caseName,
            title = title,
            description = desc,
            cost = cost,
            prerequisiteIds = preReqs,
            isPurchased = false
        });
    }



    // Fix: CS1061 - FoundationTree does not contain a definition for 'MarkAsPurchased'
    public void MarkAsPurchased(string idOrName)
    {
        // Try to find by ID first, then by nodeName
        TechNode node = nodes.Find(n => n.id == idOrName || n.nodeName == idOrName);
        if (node != null)
        {
            node.isPurchased = true;
            ApplyNodeEffect(node.id); 
        }
    }

    public bool IsNodePurchased(string id)
    {
        TechNode node = nodes.Find(n => n.id == id);
        return node != null && node.isPurchased;
    }

    public bool ArePrerequisitesMet(string id)
    {
        TechNode node = nodes.Find(n => n.id == id);
        if (node == null || string.IsNullOrEmpty(node.prerequisiteIds)) return true;

        string[] requirements = node.prerequisiteIds.Split(',');
        foreach (string reqId in requirements)
        {
            TechNode reqNode = nodes.Find(n => n.id == reqId.Trim());
            if (reqNode != null && !reqNode.isPurchased) return false;
        }
        return true;
    }

    // FIX: Added the missing ApplyNodeEffect method
    public void ApplyNodeEffect(string id)
    {
        SignalEngine engine = SignalEngine.instance;
        if (engine == null) return;

        switch (id)
        {
            case "y_1": engine.dataMult *= 1.2; break;
            case "s_1": engine.speedMult *= 1.15f; break;
            case "l_1": engine.rareChance += 0.03f; break;
            // Add more cases here matching your y_1, s_1, etc.
        }
        Debug.Log($"Effect applied for {id}");
    }
}