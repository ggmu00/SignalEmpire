using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class FoundationTreeUI : MonoBehaviour
{
    // Use the Singleton instance for the engine
    private SignalEngine engine => SignalEngine.instance;

    [SerializeField] private FoundationTree treeLogic; // Link to your FoundationTree script

    private List<TechNode> allNodes = new List<TechNode>();
    private Dictionary<char, List<TechNode>> nodesByBranch = new Dictionary<char, List<TechNode>>();

    void Awake()
    {
        if (treeLogic == null)
            treeLogic = GetComponent<FoundationTree>();
    }

    void Start()
    {
        InitializeTreeUI();
    }

    public void InitializeTreeUI()
    {
        if (treeLogic == null) return;

        // Get the nodes we defined in FoundationTree
        allNodes = treeLogic.nodes;

        // Organize nodes by branch
        // y=yield, s=speed, l=luck, a=auto
        foreach (char branch in new[] { 'y', 's', 'l', 'a' }) 
        {
            nodesByBranch[branch] = new List<TechNode>();
        }

        foreach (TechNode node in allNodes)
        {
            if (node.id.Length > 0)
            {
                char branch = node.id[0];
                if (nodesByBranch.ContainsKey(branch))
                    nodesByBranch[branch].Add(node);
            }
        }

        Debug.Log($"<color=yellow>Foundation Tree UI Initialized:</color> {allNodes.Count} nodes organized.");
    }

    public List<TechNode> GetNodesByBranch(char branchLetter)
    {
        if (nodesByBranch.ContainsKey(branchLetter))
            return nodesByBranch[branchLetter];
        return new List<TechNode>();
    }

    public List<TechNode> GetSynergyNodes()
    {
        List<TechNode> result = new List<TechNode>();
        foreach (var node in allNodes)
        {
            if (node.id.StartsWith("syn") || node.id == "apex" || node.id == "auto")
                result.Add(node);
        }
        return result;
    }

    /// <summary>
    /// This handles the UI side of the purchase, while FoundationTree handles the logic
    /// </summary>
    public bool TryPurchaseNode(string nodeID)
    {
        TechNode node = allNodes.Find(n => n.id == nodeID);
        if (node == null) return false;

        if (node.isPurchased)
        {
            Debug.LogWarning($"Node {node.id} already purchased");
            return false;
        }

        // Check prerequisites via the Tree Logic
        if (!treeLogic.ArePrerequisitesMet(node.id))
        {
            Debug.LogWarning($"Prerequisites not met for {node.id}");
            return false;
        }

        if (engine.totalPowerPoints < node.cost)
        {
            Debug.LogWarning($"Cannot afford {node.id}. Need {node.cost} PP");
            return false;
        }

        // Deduct cost and mark as purchased
        engine.totalPowerPoints -= node.cost;
        node.isPurchased = true;
        
        // Notify the Tree to mark this as purchased for future prerequisites
        treeLogic.MarkAsPurchased(node.nodeName);
        
        Debug.Log($"<color=lime>Purchased:</color> {node.title}");
        return true;
    }

    public string GetNodeDisplayInfo(string nodeID)
    {
        TechNode node = allNodes.Find(n => n.id == nodeID);
        if (node == null) return "Unknown Node";

        string info = $"<b>{node.title}</b>\n";
        info += $"<size=80%>{node.description}</size>\n\n";
        info += $"<color=yellow>Cost:</color> {node.cost.ToString("N0")} PP\n";

        if (node.isPurchased)
            info += "<color=lime>[Purchased]</color>";
        else if (treeLogic.ArePrerequisitesMet(node.id))
            info += "<color=green>[Ready to Purchase]</color>";
        else
            info += "<color=red>[Prerequisites Required]</color>";

        return info;
    }

    // DEBUGGING TOOLS
    public void DebugPrintBranch(char branchLetter)
    {
        var branchNodes = GetNodesByBranch(branchLetter);
        Debug.Log($"\n<color=cyan>=== Branch {branchLetter} ===</color>");
        foreach (var node in branchNodes)
        {
            string status = node.isPurchased ? "[✓]" : "[✗]";
            Debug.Log($"{status} {node.title} ({node.id})");
        }
    }

    public void DebugPrintAllBranches()
    {
        foreach (char branch in new[] { 'y', 's', 'l', 'a' })
        {
            DebugPrintBranch(branch);
        }
    }
}