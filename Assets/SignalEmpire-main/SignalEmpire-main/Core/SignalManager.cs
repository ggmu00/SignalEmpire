using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;


public class SignalManager : MonoBehaviour
{
    [Header("Resources")]
    public double data;
    public double powerPoints;

    [Header("Automation Settings")]
    public bool autoSeekerActive = false;
    public float tapeSpeed = 1.0f;
    public int tapeLevel = 1;
    public int speedLevel = 1;

    [Header("Internal Logic")]
    private float currentProgress = 0f;
    private float lastSaveTime;

    // Formulas (Exponential Scaling for long-term play)
    public double GetTapeValue() => 5.0 * Math.Pow(1.15, tapeLevel - 1);
    public double GetUpgradeCost() => Math.Floor(10 * Math.Pow(1.6, tapeLevel));
    public double GetSpeedCost() => Math.Floor(50 * Math.Pow(2, speedLevel - 1));

    [Header("Offline Settings")]
    public double maxOfflineHours = 24.0; // Cap to encourage daily check-ins
    public float offlineEfficiency = 0.5f; // Starts at 50% efficiency (upgradable)

    // This runs as soon as the script starts (App Open)
    void Start() 
    {
        LoadAndCalculateOffline();
    }

    [Header("Rare Signal Logic")]
    public float rareSignalChance = 0.05f; // Starts at 5%
    public float rareMultiplier = 5.0f;    // Rare signals give 5x Data
    public int sensitivityLevel = 1;

    // Adding a property for the UI to show the percentage
    public float RareChancePercentage => rareSignalChance * 100f;

    void Update()
    {
        // DeltaTime ensures the game runs at the same speed regardless of frame rate
        if (autoSeekerActive)
        {
            float increment = (15f * (1f + (speedLevel * 0.2f))) * Time.deltaTime;
            currentProgress += increment;

            if (currentProgress >= 100f)
            {
                CompleteSignalCycle();
                currentProgress = 0f;
            }
        }
    }

    public void ManualPulse()
    {
        data += 1.0;
        Debug.Log("Manual Signal Captured. Data: " + data);
    }

    public void BuyAutoSeeker()
    {
        if (data >= 20 && !autoSeekerActive)
        {
            data -= 20;
            autoSeekerActive = true;
            Debug.Log("Auto-Seeker Online.");
        }
    }

    public void UpgradeTape()
    {
        double cost = GetUpgradeCost();
        if (data >= cost)
        {
            data -= cost;
            tapeLevel++;
            Debug.Log("Tape Level Up: " + tapeLevel);
        }
    }

    public void LoadAndCalculateOffline(){
        if (!PlayerPrefs.HasKey("LastExitTime")) return;

        // 1. Get the time difference
        long lastBinaryTime = Convert.ToInt64(PlayerPrefs.GetString("LastExitTime"));
        DateTime lastExit = DateTime.FromBinary(lastBinaryTime);
        TimeSpan timeAway = DateTime.Now - lastExit;

        double secondsAway = timeAway.TotalSeconds;
        double cappedSeconds = Math.Min(secondsAway, maxOfflineHours * 3600);

        // 2. Determine Tape Cycles
        // How long does one tape take? (100 / current increment speed)
        float speedFactor = 15f * (1f + (speedLevel * 0.2f));
        float secondsPerTape = 100f / speedFactor;

        double totalTapesFinished = Math.Floor(cappedSeconds / secondsPerTape);

        // 3. Calculate Earnings with Luck
        if (totalTapesFinished > 0)
        {
            double baseVal = GetTapeValue();
            
            // Statistical average of Rare Signals: 
            // (Chance * Multiplier) + (Remainder * 1.0)
            double averageMultiplier = (rareSignalChance * rareMultiplier) + ((1f - rareSignalChance) * 1.0f);
            
            double totalEarned = totalTapesFinished * baseVal * averageMultiplier * offlineEfficiency;
            
            data += totalEarned;
            
            Debug.Log($"Welcome back! You were away for {timeAway.Hours}h {timeAway.Minutes}m.");
            Debug.Log($"Processed {totalTapesFinished} signals at {offlineEfficiency * 100}% efficiency.");
            Debug.Log($"Earned: {totalEarned:F2} Data.");
        }
    }

    void OnApplicationQuit()
    {
        SaveExitTime();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveExitTime();
    }

    private void SaveExitTime()
    {
        PlayerPrefs.SetString("LastExitTime", DateTime.Now.ToBinary().ToString());
        PlayerPrefs.SetFloat("CurrentData", (float)data); // Cast double to float for saving
        PlayerPrefs.Save();
    }


    private void CompleteSignalCycle()
    {
        double baseValue = GetTapeValue();
        
        // Roll a random number between 0.0 and 1.0
        float roll = UnityEngine.Random.value;

        if (roll <= rareSignalChance)
        {
            // JACKPOT: Rare Signal Detected
            double rareValue = baseValue * rareMultiplier;
            data += rareValue;
            
            // Discipline XP is doubled for rare signals
            powerPoints += 0.5; 

            Debug.Log($"<color=gold>RARE SIGNAL CAPTURED!</color> Received: {rareValue}");
            // Trigger visual flair here later (flashing lights, gold bar, etc.)
        }
        else
        {
            // Standard Signal
            data += baseValue;
            powerPoints += 0.1;
        }
    }

}