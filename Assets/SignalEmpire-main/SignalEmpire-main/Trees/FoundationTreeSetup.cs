using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Setup helper for integrating Foundation Tree system into your game.
/// Standardized for TechNode and SignalEngine.
/// </summary>
public class FoundationTreeSetup : MonoBehaviour
{
    private SignalEngine engine => SignalEngine.instance;

    private FoundationTreeInitializer initializer;
    private FoundationTreeUI treeUI;
    private FoundationTree treeLogic;

    public void Initialize()
    {
        // Get components on the same object
        initializer = GetComponent<FoundationTreeInitializer>();
        treeUI = GetComponent<FoundationTreeUI>();
        treeLogic = GetComponent<FoundationTree>();

        if (initializer == null || treeUI == null || treeLogic == null)
        {
            Debug.LogError("FoundationTreeSetup: Missing required components on this GameObject!");
            return;
        }

        // Initialize the tree data first
        initializer.InitializeFullTree();
        
        // Then initialize the UI
        treeUI.InitializeTreeUI();

        Debug.Log("<color=green>Foundation Tree System Initialized Successfully!</color>");
        PrintSystemStatus();
    }

    public void PrintAllBranches()
    {
        treeUI.DebugPrintAllBranches();
    }

    private void PrintSystemStatus()
    {
        if (engine == null) return;

        Debug.Log("\n<color=cyan>=== Foundation Tree System Status ===</color>");
        Debug.Log($"Player Power Points: {engine.totalPowerPoints}");
        Debug.Log($"Data Multiplier: {engine.dataMult:F2}x");
        Debug.Log($"Rare Chance: {engine.rareChance * 100:F1}%");
    }

    /// <summary>
    /// Purchase a specific node by ID
    /// </summary>
    public void PurchaseNodeById(string nodeId)
    {
        // FIX: We now pass the ID string directly to TryPurchaseNode
        bool success = treeUI.TryPurchaseNode(nodeId);
        
        if (success)
            Debug.Log($"<color=lime>Purchased node: {nodeId}</color>");
        else
            Debug.Log($"<color=red>Failed to purchase node: {nodeId}</color>");
    }

    /// <summary>
    /// Example: Purchase all nodes in a branch (Cheat/Debug function)
    /// </summary>
    public void PurchaseBranch(char branchLetter)
    {
        var branchNodes = treeUI.GetNodesByBranch(branchLetter);
        int purchasedCount = 0;

        foreach (var node in branchNodes)
        {
            // FIX: Pass node.id (string) instead of node (object)
            if (treeUI.TryPurchaseNode(node.id))
                purchasedCount++;
        }

        Debug.Log($"<color=yellow>Branch '{branchLetter}' purchase attempt: {purchasedCount}/{branchNodes.Count} succeeded.</color>");
    }

    public void GiveTestPowerPoints(double amount = 100000)
    {
        if (engine != null)
        {
            engine.totalPowerPoints += amount;
            Debug.Log($"<color=yellow>Test PP Granted: +{amount} (Total: {engine.totalPowerPoints})</color>");
        }
    }

    public string GetNodeInfo(string nodeId)
    {
        // FIX: Passes the ID string to the UI for display info
        return treeUI.GetNodeDisplayInfo(nodeId);
    }

    public bool IsNodeAvailable(string nodeId)
    {
        if (treeLogic == null) return false;
        
        // FIX: Uses the logic tree to check if purchased and if prereqs are met
        bool alreadyBought = treeLogic.IsNodePurchased(nodeId);
        bool canUnlock = treeLogic.ArePrerequisitesMet(nodeId);
        
        return !alreadyBought && canUnlock;
    }
}