using UnityEngine;

/// <summary>
/// Setup helper for integrating discipline tree systems into your game.
/// This demonstrates the proper initialization sequence and component wiring.
/// 
/// SETUP INSTRUCTIONS:
/// 1. Create an empty GameObject called "DisciplineTreeManager" in your scene
/// 2. Add these components:
///    - DisciplineTreeInitializer
///    - DisciplineTreeUI
///    - DisciplineTreeManager
///    - This script (DisciplineTreeSetup)
/// 
/// 3. In the Inspector, assign references to:
///    - SignalEngine (find it or create it)
///    - ResourceStorage (find it or create it)
/// 
/// 4. Call Initialize() from your game boot sequence
/// </summary>
public class DisciplineTreeSetup : MonoBehaviour
{
    [SerializeField] private SignalEngine engine;
    [SerializeField] private ResourceStorage storage;

    private DisciplineTreeInitializer initializer;
    private DisciplineTreeUI treeUI;
    private DisciplineTreeManager treeManager;

    public void Initialize()
    {
        // Get or create components
        initializer = GetComponent<DisciplineTreeInitializer>();
        treeUI = GetComponent<DisciplineTreeUI>();
        treeManager = GetComponent<DisciplineTreeManager>();

        if (initializer == null || treeUI == null || treeManager == null)
        {
            Debug.LogError("DisciplineTreeSetup: Missing required components!");
            return;
        }

        // Wire up references if not already set
        if (engine == null) engine = FindObjectOfType<SignalEngine>();
        if (storage == null) storage = FindObjectOfType<ResourceStorage>();

        if (engine == null || storage == null)
        {
            Debug.LogError("DisciplineTreeSetup: Cannot find SignalEngine or ResourceStorage in scene!");
            return;
        }

        treeManager.engine = engine;
        treeManager.storage = storage;

        // Initialize the tree UI (which calls InitializeAllTrees internally)
        treeUI.InitializeTreeUI();

        Debug.Log("<color=green>Discipline Tree System Initialized Successfully!</color>");
        PrintSystemStatus();
    }

    /// <summary>
    /// Display all trees in console for debugging
    /// </summary>
    public void PrintAllTrees()
    {
        foreach (DisciplinePath path in System.Enum.GetValues(typeof(DisciplinePath)))
        {
            treeUI.DebugPrintTree(path);
        }
    }

    /// <summary>
    /// Print current system status
    /// </summary>
    private void PrintSystemStatus()
    {
        Debug.Log("\n<color=cyan>=== Discipline Tree System Status ===</color>");
        Debug.Log($"Player Power Points: {engine.totalPowerPoints}");
        Debug.Log($"Available Materials:");
        foreach (Material mat in System.Enum.GetValues(typeof(Material)))
        {
            int amount = (int)storage.materialInventory[mat];
            Debug.Log($"  - {mat}: {amount}");
        }
    }

    /// <summary>
    /// Example: Unlock a specific node by name
    /// </summary>
    public void UnlockNodeByName(string nodeName, DisciplinePath path)
    {
        var nodesByType = treeUI.GetNodesByPath(path);
        
        foreach (var typeList in nodesByType.Values)
        {
            foreach (var node in typeList)
            {
                if (node.nodeName == nodeName)
                {
                    bool success = treeUI.TryUnlockNode(node);
                    Debug.Log(success ? 
                        $"<color=lime>Unlocked: {nodeName}</color>" : 
                        $"<color=red>Failed to unlock: {nodeName}</color>");
                    return;
                }
            }
        }

        Debug.LogWarning($"Node '{nodeName}' not found in {path} path");
    }

    /// <summary>
    /// Example: Give player test materials for unlocking
    /// </summary>
    public void GiveTestMaterials()
    {
        foreach (Material mat in System.Enum.GetValues(typeof(Material)))
        {
            storage.AddMaterial(mat, 100);
        }
        engine.totalPowerPoints += 1000;
        Debug.Log("<color=yellow>Test Materials Granted: 100 of each material + 1000 PP</color>");
    }
}
