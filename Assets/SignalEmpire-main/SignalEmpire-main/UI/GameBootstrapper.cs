using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    void Start()
    {
        // Locates our manager script in the scene and fires it up
        FoundationTreeSetup treeSetup = FindFirstObjectByType<FoundationTreeSetup>();
        
        if (treeSetup != null)
        {
            treeSetup.Initialize();
            Debug.Log("<color=green>Bootstrapper successfully initialized FoundationTreeSetup!</color>");
        }
        else
        {
            Debug.LogError("GameBootstrapper: Could not find FoundationTreeSetup in the scene! Is the TreeLogic_Manager active?");
        }
    }
}