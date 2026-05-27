using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlanetUIElement : MonoBehaviour
{
    public PlanetData planet; // Drag your Planet ScriptableObject here in Inspector
    public TextMeshProUGUI nameLabel;
    public Button upgradeButton;

    void Start()
    {
        // Update the UI with the Planet's data
        nameLabel.text = planet.planetName;
        
        // Setup the button to call the Planet's upgrade method
        upgradeButton.onClick.AddListener(AttemptUpgrade);
    }

    void AttemptUpgrade()
    {
        // Calling a method from the PlanetData class
        // Note: You'll need a reference to your SignalEngine's 'data' variable
        double playerData = SignalEngine.instance.currentData; 
        
        if(planet.PurchaseMineUpgrade("yield_1", ref playerData))
        {
             // Update the global data after purchase
             SignalEngine.instance.currentData = playerData;
             Debug.Log("Upgrade Successful!");
        }
    }
}