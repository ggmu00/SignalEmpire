using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlanetaryUnlockShop : MonoBehaviour
{
    public ResourceStorage storage;
    public SignalEngine engine;
    public PlanetInitializer planetInitializer;

    [Header("UI Elements")]
    public Text planetNameDisplay;
    public Text costDisplay;
    public Button unlockButton;

    private PlanetData selectedPlanet;

    // Called when a player clicks a planet icon in the shop menu
    public void SelectPlanet(PlanetData planet)
    {
        selectedPlanet = planet;
        planetNameDisplay.text = planet.planetName;
        costDisplay.text = $"Cost: {planet.unlockCost:N0} Data";

        // Check if already unlocked
        if (storage.unlockedPlanets.Contains(planet))
        {
            unlockButton.interactable = true;
            unlockButton.GetComponentInChildren<Text>().text = "Travel";
        }
        else
        {
            unlockButton.interactable = engine.currentData >= planet.unlockCost;
            unlockButton.GetComponentInChildren<Text>().text = "Unlock";
        }
    }

    public void OnUnlockPressed()
    {
        if (selectedPlanet == null) return;

        // Logic: Travel if unlocked, Buy if not
        if (storage.unlockedPlanets.Contains(selectedPlanet))
        {
            TravelToPlanet(selectedPlanet);
        }
        else if (engine.currentData >= selectedPlanet.unlockCost)
        {
            engine.currentData -= selectedPlanet.unlockCost;
            storage.UnlockPlanet(selectedPlanet);
            TravelToPlanet(selectedPlanet);
        }
    }

    public double GetLayerUnlockCost(OrbitalLevel orbit)
    {
        if (planetInitializer == null || storage == null) return 0.0;

        double cost = 0.0;
        foreach (PlanetData planet in planetInitializer.GetPlanetsInLayer(orbit))
        {
            if (!storage.unlockedPlanets.Contains(planet))
                cost += planet.unlockCost;
        }
        return cost;
    }

    public void UnlockLayer(OrbitalLevel orbit)
    {
        if (planetInitializer == null || storage == null || engine == null) return;

        List<PlanetData> planetsToUnlock = new List<PlanetData>();
        double totalCost = 0.0;

        foreach (PlanetData planet in planetInitializer.GetPlanetsInLayer(orbit))
        {
            if (!storage.unlockedPlanets.Contains(planet))
            {
                planetsToUnlock.Add(planet);
                totalCost += planet.unlockCost;
            }
        }

        if (planetsToUnlock.Count == 0)
        {
            Debug.Log($"<color=yellow>Layer already unlocked:</color> {orbit}");
            return;
        }

        if (engine.currentData < totalCost)
        {
            Debug.Log($"<color=red>Not enough Data:</color> Need {totalCost:N0} to unlock {orbit} layer.");
            return;
        }

        engine.currentData -= totalCost;
        storage.UnlockPlanets(planetsToUnlock);
        Debug.Log($"<color=green>Layer Unlocked:</color> {orbit} layer unlocked with {planetsToUnlock.Count} planets.");
    }

    private void TravelToPlanet(PlanetData planet)
    {
        storage.activePlanet = planet;
        Debug.Log($"Warping to {planet.planetName} ({planet.orbit} Orbit)");
        // Update UI or play warp animation here
        this.gameObject.SetActive(false); // Close shop
    }
}