using UnityEngine;
using System.Collections.Generic;

// Define the orbital levels for organization
public enum OrbitalLevel { Inner, Middle, Outer }

// Material types are now the only mined resources
public enum Material { CryoQuartz, Neuralite, ObsidianFlux, VoidMatter, AetherGlass, PrismDust, Isotope9, GravSalt, Silica, Iron, Cobalt, Helium3, Platinum, Antimatter, DarkMatter }

[System.Serializable]
public class MineUpgradeNode
{
    public string id;
    public string title;
    [TextArea] public string description;
    public double cost;
    
    [Header("State")]
    public bool isPurchased;
    public string prerequisiteIds; // Comma-separated

    public bool IsUnlocked(List<MineUpgradeNode> allNodes)
    {
        if (string.IsNullOrEmpty(prerequisiteIds)) return true;
        
        string[] requirements = prerequisiteIds.Split(',');
        foreach (string reqId in requirements)
        {
            var preReq = allNodes.Find(n => n.id == reqId.Trim());
            if (preReq == null || !preReq.isPurchased) return false;
        }
        return true;
    }
}

[CreateAssetMenu(fileName = "NewPlanet", menuName = "Economy/Planet")]
public class PlanetData : ScriptableObject
{
    public string planetName;
    public OrbitalLevel orbit;
    public double unlockCost; 
    
    [Header("Environment")]
    public float decayModifier = 1.0f; // How much faster/slower signals rot here
    public float signalStrengthMult = 1.0f; // Environmental interference
    public string environmentModifier = "";
    public int signalReceiverLevel = 1;

    [Header("Mine")]
    public int mineLevel = 1;
    public int maxMineLevel = 5;
    public List<MineUpgradeNode> mineUpgradeNodes = new List<MineUpgradeNode>();

    [Header("Resources")]
    public Material primaryMaterial;
    public Material rareMaterial;
    public float rareDropChance = 0.05f; // 5% base chance for special materials

    public float GetReceiverQualityFloor()
    {
        return Mathf.Min(0.8f, 0.1f + (signalReceiverLevel - 1) * 0.05f);
    }

    public double GetReceiverSignalBonus()
    {
        return 1.0 + (signalReceiverLevel - 1) * 0.05;
    }

    public bool IsMineMaxed() => mineUpgradeNodes.Count > 0 && mineUpgradeNodes.TrueForAll(n => n.isPurchased);

    public double GetMineUpgradeCost() => EconomyDefinitions.CalculateCost(mineLevel, 120, 1.4);

    public void InitializeMineUpgrades()
    {
        if (mineUpgradeNodes.Count > 0) return; // Already initialized

        // Create 6 upgrade nodes for this mine
        mineUpgradeNodes.Add(new MineUpgradeNode { id = "yield_1", title = "Enhanced Drills", description = "+25% Mine Yield", cost = 1000, prerequisiteIds = "" });
        mineUpgradeNodes.Add(new MineUpgradeNode { id = "yield_2", title = "Ore Refinement", description = "+35% Mine Yield", cost = 2500, prerequisiteIds = "yield_1" });
        mineUpgradeNodes.Add(new MineUpgradeNode { id = "efficiency_1", title = "Automated Haulers", description = "-15% Mine Upgrade Costs", cost = 4000, prerequisiteIds = "yield_2" });
        mineUpgradeNodes.Add(new MineUpgradeNode { id = "rare_1", title = "Rare Ore Scanner", description = "+10% Rare Material Drop Chance", cost = 6000, prerequisiteIds = "efficiency_1" });
        mineUpgradeNodes.Add(new MineUpgradeNode { id = "signal_1", title = "Signal Amplifier", description = "+15% Signal Strength on this Planet", cost = 8000, prerequisiteIds = "rare_1" });
        mineUpgradeNodes.Add(new MineUpgradeNode { id = "decay_1", title = "Stabilizer Core", description = "-20% Signal Decay Rate on this Planet", cost = 10000, prerequisiteIds = "signal_1" });
    }

    public bool PurchaseMineUpgrade(string id, ref double playerData)
    {
        MineUpgradeNode node = mineUpgradeNodes.Find(n => n.id == id);
        if (node != null && !node.isPurchased && node.IsUnlocked(mineUpgradeNodes) && playerData >= node.cost)
        {
            playerData -= node.cost;
            node.isPurchased = true;
            ApplyMineUpgradeEffect(node.id);
            return true;
        }
        return false;
    }

    private void ApplyMineUpgradeEffect(string id)
    {
        switch (id)
        {
            case "yield_1": // Handled in mining logic
            case "yield_2": break;
            case "efficiency_1": // Reduce costs - handled elsewhere
            case "rare_1": rareDropChance += 0.10f; break;
            case "signal_1": signalStrengthMult *= 1.15f; break;
            case "decay_1": decayModifier *= 0.8f; break;
        }
    }

    public float GetMineYieldMultiplier()
    {
        float mult = 1.0f;
        if (mineUpgradeNodes.Exists(n => n.id == "yield_1" && n.isPurchased)) mult *= 1.25f;
        if (mineUpgradeNodes.Exists(n => n.id == "yield_2" && n.isPurchased)) mult *= 1.35f;
        return mult;
    }

    public float GetMineCostReduction()
    {
        return mineUpgradeNodes.Exists(n => n.id == "efficiency_1" && n.isPurchased) ? 0.85f : 1.0f;
    }
}
