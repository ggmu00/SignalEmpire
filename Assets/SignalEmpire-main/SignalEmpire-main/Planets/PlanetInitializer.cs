using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlanetInitializer : MonoBehaviour
{
    public List<PlanetData> allPlanets = new List<PlanetData>();

    private void Awake()
    {
        InitializePlanets();
    }

    private void InitializePlanets()
    {
        // Inner, Pyros-7, Prism-Dust, Extreme Heat: +100% Decay Rate.
        PlanetData pyros7 = CreatePlanet("Pyros-7", OrbitalLevel.Inner, 1000, 2.0f, 1.0f, "Extreme Heat: +100% Decay Rate.", Material.PrismDust, Material.PrismDust, 0.05f);
        allPlanets.Add(pyros7);

        // Inner, Vespera, Aether-Glass, Dense Clouds: -20% Signal Strength.
        PlanetData vespera = CreatePlanet("Vespera", OrbitalLevel.Inner, 1500, 1.0f, 0.8f, "Dense Clouds: -20% Signal Strength.", Material.AetherGlass, Material.AetherGlass, 0.05f);
        allPlanets.Add(vespera);

        // Middle, Oasis Prime, Obsidian-Flux, Temperate: No modifiers.
        PlanetData oasisPrime = CreatePlanet("Oasis Prime", OrbitalLevel.Middle, 2000, 1.0f, 1.0f, "Temperate: No modifiers.", Material.ObsidianFlux, Material.ObsidianFlux, 0.05f);
        allPlanets.Add(oasisPrime);

        // Middle, Kaltos, Cryo-Quartz, Cryo-Storms: Mine Speed -10%.
        PlanetData kaltos = CreatePlanet("Kaltos", OrbitalLevel.Middle, 2500, 1.0f, 1.0f, "Cryo-Storms: Mine Speed -10%.", Material.CryoQuartz, Material.CryoQuartz, 0.05f);
        allPlanets.Add(kaltos);

        // Middle, Aethelgard, Isotope-9, Radio-Quiet: +15% Rare Signal Chance.
        PlanetData aethelgard = CreatePlanet("Aethelgard", OrbitalLevel.Middle, 3000, 1.0f, 1.0f, "Radio-Quiet: +15% Rare Signal Chance.", Material.Isotope9, Material.Isotope9, 0.20f);
        allPlanets.Add(aethelgard);

        // Outer, Nox, Grav-Salt, High Gravity: Mine Build Cost +25%.
        PlanetData nox = CreatePlanet("Nox", OrbitalLevel.Outer, 4000, 1.0f, 1.0f, "High Gravity: Mine Build Cost +25%.", Material.GravSalt, Material.GravSalt, 0.05f);
        allPlanets.Add(nox);

        // Outer, Zenith-9, Neuralite, Void-static: Signal Strength fluctuates.
        PlanetData zenith9 = CreatePlanet("Zenith-9", OrbitalLevel.Outer, 5000, 1.0f, 1.0f, "Void-static: Signal Strength fluctuates.", Material.Neuralite, Material.Neuralite, 0.05f);
        allPlanets.Add(zenith9);

        // Outer, The Abyss, Void-Matter, Physics Distortion: Min Quality is 5%.
        PlanetData theAbyss = CreatePlanet("The Abyss", OrbitalLevel.Outer, 6000, 1.0f, 1.0f, "Physics Distortion: Min Quality is 5%.", Material.VoidMatter, Material.VoidMatter, 0.05f);
        allPlanets.Add(theAbyss);

        // Initialize mine upgrades for all planets
        foreach (PlanetData planet in allPlanets)
        {
            planet.InitializeMineUpgrades();
        }

        Debug.Log($"<color=green>Planets Initialized:</color> {allPlanets.Count} planets created.");
    }

    private PlanetData CreatePlanet(string name, OrbitalLevel orbit, double unlockCost, float decayModifier,
        float signalStrengthMult, string environmentModifier, Material primaryMaterial, Material rareMaterial, float rareDropChance)
    {
        PlanetData planet = ScriptableObject.CreateInstance<PlanetData>();
        planet.planetName = name;
        planet.orbit = orbit;
        planet.unlockCost = unlockCost;
        planet.decayModifier = decayModifier;
        planet.signalStrengthMult = signalStrengthMult;
        planet.environmentModifier = environmentModifier;
        planet.primaryMaterial = primaryMaterial;
        planet.rareMaterial = rareMaterial;
        planet.rareDropChance = rareDropChance;
        return planet;
    }

    public List<PlanetData> GetPlanetsInLayer(OrbitalLevel orbit)
    {
        List<PlanetData> layerPlanets = new List<PlanetData>();
        foreach (PlanetData planet in allPlanets)
        {
            if (planet.orbit == orbit)
                layerPlanets.Add(planet);
        }
        return layerPlanets;
    }

    public int GetMaxedMineCountInLayer(OrbitalLevel orbit)
    {
        return GetPlanetsInLayer(orbit).Count(p => p.IsMineMaxed());
    }

    public int GetTotalMaxedMines()
    {
        return allPlanets.Count(p => p.IsMineMaxed());
    }

    public PlanetData GetPlanetByName(string name)
    {
        return allPlanets.Find(p => p.planetName == name);
    }
}