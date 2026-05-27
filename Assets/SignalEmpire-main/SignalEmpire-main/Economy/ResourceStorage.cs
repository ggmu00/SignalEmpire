using UnityEngine;
using System.Collections.Generic;

public class ResourceStorage : MonoBehaviour
{
    // --- PLANET TRACKING ---
    [Header("Unlocked Locations")]
    public List<PlanetData> unlockedPlanets = new List<PlanetData>();
    public PlanetData activePlanet;

    // --- RESOURCE INVENTORY ---
    // All mined resources are now materials
    public Dictionary<Material, double> materialInventory = new Dictionary<Material, double>();

    private void Awake()
    {
        InitializeStorage();
    }

    /// <summary>
    /// Initializes all material types to zero to prevent KeyNotFound errors.
    /// </summary>
    private void InitializeStorage()
    {
        foreach (Material mat in System.Enum.GetValues(typeof(Material)))
        {
            if (!materialInventory.ContainsKey(mat))
                materialInventory[mat] = 0.0;
        }
        Debug.Log("Storage Initialized: Material inventory mapped.");
    }

    // --- CORE RESOURCE METHODS ---

    public void AddMaterial(Material type, double amount)
    {
        materialInventory[type] += amount;
        Debug.Log($"<color=cyan>Material Secured:</color> +{amount} {type}");
    }

    // --- SPENDING & VALIDATION ---

    public bool CanAffordSpecial(Material mat, int amount)
    {
        return materialInventory.ContainsKey(mat) && materialInventory[mat] >= amount;
    }

    public void SpendSpecial(Material mat, int amount)
    {
        if (CanAffordSpecial(mat, amount))
        {
            materialInventory[mat] -= amount;
            Debug.Log($"Spent {amount} {mat}. Remaining: {materialInventory[mat]}");
        }
    }

    // --- PLANETARY PROGRESSION ---

    public void UnlockPlanet(PlanetData newPlanet)
    {
        if (!unlockedPlanets.Contains(newPlanet))
        {
            unlockedPlanets.Add(newPlanet);
            Debug.Log($"<color=green>New Horizon:</color> {newPlanet.planetName} is now accessible.");
        }
    }

    public void UnlockPlanets(IEnumerable<PlanetData> planets)
    {
        foreach (PlanetData planet in planets)
        {
            UnlockPlanet(planet);
        }
    }
}