using UnityEngine;
using System;

public class SignalUpgrades : MonoBehaviour
{
    private SignalManager manager;

    void Start() {
        manager = GetComponent<SignalManager>();
    }

    // --- TAPE DENSITY (Yield Upgrade) ---
    // Increases Data ($D$) per tape completion.
    public double GetTapeValueCost() => 10 * Math.Pow(1.15, manager.tapeLevel);
    
    public void UpgradeTapeValue() {
        double cost = GetTapeValueCost();
        if (manager.data >= cost) {
            manager.data -= cost;
            manager.tapeLevel++;
        }
    }

    // --- MOTOR OVERCLOCK (Speed Upgrade) ---
    // Makes the progress bar move faster.
    public double GetSpeedCost() => 50 * Math.Pow(1.8, manager.speedLevel);

    public void UpgradeSpeed() {
        double cost = GetSpeedCost();
        if (manager.data >= cost) {
            manager.data -= cost;
            manager.speedLevel++;
        }
    }

    // --- SIGNAL SENSITIVITY (Clarity Upgrade) ---
    // Increases the rate at which you gain Discipline/Power Points (PP).
    public double GetSensitivityCost() => 150 * Math.Pow(2.2, manager.sensitivityLevel - 1);

    public void UpgradeSensitivity()
    {
        double cost = GetSensitivityCost();
        if (manager.data >= cost)
        {
            manager.data -= cost;
            manager.sensitivityLevel++;
            manager.rareSignalChance = Mathf.Min(0.05f + (manager.sensitivityLevel * 0.01f), 0.40f);
            Debug.Log("Signal Sensitivity Upgraded. New Chance: " + manager.rareSignalChance * 100.0f + "%");
        }
    }

    // Returns how much it would cost to buy 'amount' of levels at once
    public double GetBulkCost(int amount, int currentLevel, double basePrice, double multiplier) {
        // Formula: Cost = Base * ((Mult^Current * (Mult^Amount - 1)) / (Mult - 1))
        double totalCost = basePrice * ((Math.Pow(multiplier, currentLevel) * (Math.Pow(multiplier, amount) - 1)) / (multiplier - 1));
        return totalCost;
    }
}