using UnityEngine;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    private SignalEngine engine;
    private ResourceStorage storage;

    void Awake()
    {
        engine = GetComponent<SignalEngine>();
        storage = GetComponent<ResourceStorage>();
    }

    public void SaveGame()
    {
        // 1. SAVE CORE PROGRESS
        PlayerPrefs.SetInt("TapeLevel", engine.tapeLevel);
        PlayerPrefs.SetInt("SpeedLevel", engine.speedLevel);
        PlayerPrefs.SetInt("SensitivityLevel", engine.sensitivityLevel);
        PlayerPrefs.SetFloat("RareChance", engine.rareSignalChance);
        
        PlayerPrefs.SetString("CurrentData", engine.currentData.ToString());
        PlayerPrefs.SetString("TotalPP", engine.totalPowerPoints.ToString());

        // 2. SAVE TECH TREE STATE
        PlayerPrefs.SetInt("SignalMatchingAutomationUnlocked", engine.signalMatchingAutomationUnlocked ? 1 : 0);

        // 2. SAVE MATERIAL INVENTORY
        foreach (var entry in storage.materialInventory)
        {
            PlayerPrefs.SetString("Mat_" + entry.Key.ToString(), entry.Value.ToString());
        }

        // 3. SAVE UNLOCKED PLANETS
        string planetData = "";
        foreach (PlanetData p in storage.unlockedPlanets)
        {
            planetData += p.planetName + ",";
        }
        PlayerPrefs.SetString("UnlockedPlanets", planetData);
        
        if (storage.activePlanet != null)
        {
            PlayerPrefs.SetString("ActivePlanet", storage.activePlanet.planetName);
        }

        // 4. SAVE PLANET UPGRADES
        foreach (PlanetData p in storage.unlockedPlanets)
        {
            PlayerPrefs.SetInt(p.planetName + "_ReceiverLevel", p.signalReceiverLevel);
            PlayerPrefs.SetInt(p.planetName + "_MineLevel", p.mineLevel);
            string upgradeData = "";
            foreach (MineUpgradeNode node in p.mineUpgradeNodes)
            {
                if (node.isPurchased) upgradeData += node.id + ",";
            }
            PlayerPrefs.SetString(p.planetName + "_MineUpgrades", upgradeData);
        }

        PlayerPrefs.Save();
        Debug.Log("<color=green>Game Saved Successfully.</color>");
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("TapeLevel")) return;

        engine.tapeLevel = PlayerPrefs.GetInt("TapeLevel");
        engine.speedLevel = PlayerPrefs.GetInt("SpeedLevel");
        engine.sensitivityLevel = PlayerPrefs.GetInt("SensitivityLevel");
        engine.rareSignalChance = PlayerPrefs.GetFloat("RareChance");
        
        engine.currentData = double.Parse(PlayerPrefs.GetString("CurrentData", "0"));
        engine.totalPowerPoints = double.Parse(PlayerPrefs.GetString("TotalPP", "0"));

        // LOAD TECH TREE STATE
        engine.signalMatchingAutomationUnlocked = PlayerPrefs.GetInt("SignalMatchingAutomationUnlocked", 0) == 1;

        foreach (Material mat in System.Enum.GetValues(typeof(Material)))
        {
            string key = "Mat_" + mat.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                storage.materialInventory[mat] = double.Parse(PlayerPrefs.GetString(key));
            }
        }

        string planetData = PlayerPrefs.GetString("UnlockedPlanets", "");
        string[] planetNames = planetData.Split(',');
        
        PlanetInitializer initializer = FindObjectOfType<PlanetInitializer>();
        foreach (string name in planetNames)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                PlanetData planet = initializer.GetPlanetByName(name);
                if (planet != null)
                {
                    storage.unlockedPlanets.Add(planet);
                    
                    // Load upgrades
                    planet.signalReceiverLevel = PlayerPrefs.GetInt(name + "_ReceiverLevel", 1);
                    planet.mineLevel = PlayerPrefs.GetInt(name + "_MineLevel", 1);
                    
                    string upgradeData = PlayerPrefs.GetString(name + "_MineUpgrades", "");
                    string[] upgrades = upgradeData.Split(',');
                    foreach (string upgradeId in upgrades)
                    {
                        if (!string.IsNullOrWhiteSpace(upgradeId))
                        {
                            MineUpgradeNode node = planet.mineUpgradeNodes.Find(n => n.id == upgradeId);
                            if (node != null) node.isPurchased = true;
                        }
                    }
                }
            }
        }
        
        string activePlanetName = PlayerPrefs.GetString("ActivePlanet", "");
        if (!string.IsNullOrEmpty(activePlanetName))
        {
            storage.activePlanet = initializer.GetPlanetByName(activePlanetName);
        }
        
        // Logic to re-link PlanetData assets by name would go here
        
        Debug.Log("<color=cyan>Game Loaded Successfully.</color>");
    }

    public void WipeSave()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("<color=red>Save Data Wiped.</color>");
    }
}