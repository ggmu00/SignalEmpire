using UnityEngine;
using System.Collections.Generic;

public class FoundationTreeInitializer : MonoBehaviour
{
    [SerializeField] private List<TechNode> allNodes = new List<TechNode>();

    public List<TechNode> GetAllNodes() => allNodes;

    public void InitializeFullTree()
    {
        allNodes.Clear();

        // --- BRANCH: DATA ARCHITECT (YIELD) ---
        // Format: ID, CaseName (for switch), Title, Description, Cost, Prereqs
        AddNode("y_1", "DataYield1", "Iron Oxide Coating", "+20% Data yield.", 10, "");
        AddNode("y_2", "DataYield2", "Bit-Packing", "+35% Data yield.", 50, "y_1");
        AddNode("y_3", "DataYield3", "Lossless Encoding", "+50% Data yield.", 150, "y_2");
        AddNode("y_4", "DataYield4", "Neural Compression", "+100% Data yield.", 600, "y_3");
        AddNode("y_5", "DataYield5", "Quantum Storage", "Yield multiplied by Tape Speed.", 2000, "y_4");

        // --- BRANCH: KINETICIST (SPEED) ---
        AddNode("s_1", "TapeSpeed1", "Polished Spools", "+15% Speed.", 15, "y_1");
        AddNode("s_2", "TapeSpeed2", "High-Torque Motors", "+30% Speed.", 75, "s_1");
        AddNode("s_3", "TapeSpeed3", "Dual-Reel Sync", "Reduces tape reset delay by 50%.", 200, "s_2");
        AddNode("s_4", "TapeSpeed4", "Vacuum Chamber", "+100% Speed.", 800, "s_3");
        AddNode("s_5", "TapeSpeed5", "Tachyon Drive", "Tapes under 0.5s finish instantly.", 2500, "s_4");

        // --- BRANCH: CHANCELLOR (LUCK) ---
        AddNode("l_1", "RareChance1", "Static Filtering", "+3% Rare Chance.", 25, "y_1");
        AddNode("l_2", "RareChance2", "Signal Resonance", "+6% Rare Chance.", 110, "l_1");
        AddNode("l_3", "RareMultiplier", "Prism Lens", "Rare signals pay 20x instead of 5x.", 300, "l_2");
        AddNode("l_4", "RareChance3", "Void Magnet", "+15% Rare Chance.", 1000, "l_3");
        AddNode("l_5", "UnlockAnomalous", "Anomalous Horizon", "Unlocks Anomalous signals.", 3000, "l_4");

        // --- BRANCH: AUTOMATON (EFFICIENCY) ---
        AddNode("a_1", "AutoSeekerTargeting", "Smart Seek", "Auto-Seeker targets highest value.", 40, "s_1");
        AddNode("a_2", "OfflineBonus", "Background Scrub", "Offline efficiency 80%.", 150, "a_1");
        AddNode("a_3", "OfflineData", "Buffer Array", "5% bonus data per hour offline.", 500, "a_2");
        AddNode("a_4", "OfflineMax", "Infinite Loop", "Offline efficiency 100%.", 1500, "a_3");
        AddNode("a_5", "UpgradeCostReduction", "Deep Learning", "-10% Cost per 'A' node.", 4000, "a_4");

        // --- SYNERGY NODES ---
        AddNode("syn_1", "SpeedBurst", "Chronos Pulse", "Rare signals grant 10s Speed Burst.", 1200, "s_3,l_3");
        AddNode("syn_2", "AutoSeekerDataSpeed", "Economic AI", "Auto-Seeker +1% speed per 10k Data.", 1500, "y_3,a_3");
        AddNode("syn_3", "RareSignalsGrantPP", "Void Siphon", "Rare signals grant 1 Power Point.", 5000, "l_4,a_4");

        // --- THE APEX GATEWAY ---
        AddNode("apex_gate", "CanAccessPlanets", "Planetary Registry", "The stars are now within reach.", 10000, "y_5,s_5,l_5,a_5");
        AddNode("auto", "SignalMatchingAutomation", "Signal Automation", "Auto Signal Matching.", 15000, "apex_gate");

        Debug.Log($"<color=cyan>Foundation Tree Initialized:</color> {allNodes.Count} nodes loaded.");
    }

    private void AddNode(string id, string caseName, string title, string desc, double cost, string preReqs)
    {
        TechNode node = new TechNode
        {
            id = id,
            // Make sure your TechNode class has a 'caseName' field!
            // If it doesn't, you can use 'title' or 'nodeName'
            nodeName = caseName, 
            title = title,
            description = desc,
            cost = cost,
            prerequisiteIds = preReqs,
            isPurchased = false
        };
        allNodes.Add(node);
    }

    public TechNode GetNodeById(string id) => allNodes.Find(n => n.id == id);

    public List<TechNode> GetNodesByBranch(char branchLetter)
    {
        List<TechNode> result = new List<TechNode>();
        foreach (var node in allNodes)
        {
            if (node.id.Length > 0 && node.id[0] == branchLetter)
                result.Add(node);
        }
        return result;
    }
}