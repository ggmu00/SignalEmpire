using UnityEngine;
using System.Collections.Generic;

public class MiningController : MonoBehaviour
{
    public PlanetData currentPlanet;
    public int mineLevel = 0;
    public int minePowerLevel = 1;
    public int mineSpeedLevel = 1;
    public SignalEngine engine;

    // Storage for all mined material types
    public Dictionary<Material, double> materialInventory = new Dictionary<Material, double>();

    void Awake()
    {
        foreach (Material mat in System.Enum.GetValues(typeof(Material)))
            materialInventory[mat] = 0;
    }

    public void ProcessMining(float deltaTime)
    {
        if (mineLevel <= 0 || currentPlanet == null) return;

        double yieldMultiplier = engine != null ? engine.mineralYieldMult : 1.0;
        double outputMultiplier = (engine != null && engine.industrialOverdriveActive) ? 2.0 : 1.0;
        double powerBonus = 1.0 + (minePowerLevel - 1) * 0.15;
        double speedBonus = 1.0 + (mineSpeedLevel - 1) * 0.12;
        double planetYieldBonus = currentPlanet.GetMineYieldMultiplier();

        double yield = (mineLevel * 0.75) * deltaTime * yieldMultiplier * outputMultiplier * powerBonus * speedBonus * planetYieldBonus;
        materialInventory[currentPlanet.primaryMaterial] += yield;

        if (engine != null && engine.pureMineralDropsUnlocked && Random.value <= 0.002f * deltaTime)
        {
            materialInventory[currentPlanet.primaryMaterial] += 0.25;
            Debug.Log($"<color=cyan>Pure Mineral Bonus:</color> +0.25 {currentPlanet.primaryMaterial}");
        }
    }

    public void AddMaterial(Material mat, double amount)
    {
        materialInventory[mat] += amount;
        Debug.Log($"<color=cyan>Material Secured:</color> +{amount} {mat}");
    }

    public double GetMinePowerUpgradeCost() => EconomyDefinitions.CalculateCost(minePowerLevel, 75, 1.25);
    public double GetMineSpeedUpgradeCost() => EconomyDefinitions.CalculateCost(mineSpeedLevel, 80, 1.25);

    public bool UpgradeMinePowerLevel()
    {
        if (engine == null) return false;

        double cost = GetMinePowerUpgradeCost();
        if (engine.currentData >= cost)
        {
            engine.currentData -= cost;
            minePowerLevel++;
            Debug.Log($"<color=green>Mine Power Upgraded:</color> Level {minePowerLevel}");
            return true;
        }
        return false;
    }

    public bool UpgradeMineSpeedLevel()
    {
        if (engine == null) return false;

        double cost = GetMineSpeedUpgradeCost();
        if (engine.currentData >= cost)
        {
            engine.currentData -= cost;
            mineSpeedLevel++;
            Debug.Log($"<color=green>Mine Speed Upgraded:</color> Level {mineSpeedLevel}");
            return true;
        }
        return false;
    }
}