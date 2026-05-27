using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Engine Link")]
    public SignalEngine engine;
    
    [Header("Text Displays")]
    public TextMeshProUGUI dataDisplay;
    public TextMeshProUGUI ppDisplay;
    
    [Header("Bars")]
    public Slider progressSlider;
    public Image heatFillImage; // Assign the "Fill" image of your heat slider here

    void Update()
    {
        if (engine == null) return;

        // 1. Update Resources (using the names from your SignalEngine)
        if (dataDisplay != null) 
            dataDisplay.text = "DATA: " + engine.currentData.ToString("N0");
            
        if (ppDisplay != null) 
            ppDisplay.text = "PP: " + engine.totalPowerPoints.ToString("N0");

        // 2. Update Progress Bar
        if (progressSlider != null)
        {
            progressSlider.value = engine.currentProgress; // This is now public
        }

        // 3. Update Heat Visuals
        if (heatFillImage != null)
        {
            float heatPercent = engine.currentHeat / engine.maxHeatCapacity;
            heatFillImage.color = Color.Lerp(Color.cyan, Color.red, heatPercent);
        }
    }
}