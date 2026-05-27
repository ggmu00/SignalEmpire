using UnityEngine;

public class LayerUnlockManager : MonoBehaviour
{
    public PlanetInitializer planetInitializer;
    public PlanetaryUnlockShop unlockShop;
    public ResourceStorage storage;

    private void Start()
    {
        // Check for unlocks periodically
        InvokeRepeating(nameof(CheckLayerUnlocks), 1f, 1f);
    }

    public void UnlockInnerLayer()
    {
        if (unlockShop != null)
        {
            unlockShop.UnlockLayer(OrbitalLevel.Inner);
        }
    }

    public void CheckLayerUnlocks()
    {
        if (planetInitializer == null || unlockShop == null || storage == null) return;

        // Check Middle layer: any 2 mines maxed in Inner
        int maxedInInner = planetInitializer.GetMaxedMineCountInLayer(OrbitalLevel.Inner);
        if (maxedInInner >= 2 && !IsLayerUnlocked(OrbitalLevel.Middle))
        {
            unlockShop.UnlockLayer(OrbitalLevel.Middle);
        }

        // Check Outer layer: 5 total maxed mines
        int totalMaxed = planetInitializer.GetTotalMaxedMines();
        if (totalMaxed >= 5 && !IsLayerUnlocked(OrbitalLevel.Outer))
        {
            unlockShop.UnlockLayer(OrbitalLevel.Outer);
        }
    }

    private bool IsLayerUnlocked(OrbitalLevel orbit)
    {
        var planetsInLayer = planetInitializer.GetPlanetsInLayer(orbit);
        return planetsInLayer.TrueForAll(p => storage.unlockedPlanets.Contains(p));
    }
}