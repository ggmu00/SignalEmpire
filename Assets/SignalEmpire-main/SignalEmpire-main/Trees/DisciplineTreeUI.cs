using UnityEngine;
using System.Collections.Generic;

public class DisciplineTreeUI : MonoBehaviour
{
    [SerializeField] private DisciplineTreeInitializer treeInitializer;
    [SerializeField] private DisciplineTreeManager treeManager;
    [SerializeField] private ResourceStorage storage;
    [SerializeField] private SignalEngine engine;

    private List<DisciplineNode> allNodes = new List<DisciplineNode>();
    private Dictionary<DisciplinePath, List<DisciplineNode>> nodesByPath = new Dictionary<DisciplinePath, List<DisciplineNode>>();

    void Awake()
    {
        if (treeInitializer == null)
            treeInitializer = GetComponent<DisciplineTreeInitializer>();
    }

    void Start()
    {
        InitializeTreeUI();
    }

    public void InitializeTreeUI()
    {
        if (treeInitializer == null) return;
        
        treeInitializer.InitializeAllTrees();
        allNodes = treeInitializer.GetAllNodes();

        // Organize nodes by discipline path
        foreach (DisciplinePath path in System.Enum.GetValues(typeof(DisciplinePath)))
        {
            nodesByPath[path] = new List<DisciplineNode>();
        }

        foreach (DisciplineNode node in allNodes)
        {
            nodesByPath[node.discipline].Add(node);
        }

        Debug.Log($"<color=yellow>Discipline Tree UI Initialized:</color> {allNodes.Count} nodes organized by path.");
    }

    /// <summary>
    /// Returns all nodes for a specific discipline path, organized by type.
    /// </summary>
    public Dictionary<NodeType, List<DisciplineNode>> GetNodesByPath(DisciplinePath path)
    {
        Dictionary<NodeType, List<DisciplineNode>> result = new Dictionary<NodeType, List<DisciplineNode>>();

        foreach (NodeType type in System.Enum.GetValues(typeof(NodeType)))
        {
            result[type] = new List<DisciplineNode>();
        }

        if (nodesByPath.ContainsKey(path))
        {
            foreach (DisciplineNode node in nodesByPath[path])
            {
                result[node.type].Add(node);
            }
        }

        return result;
    }

    /// <summary>
    /// Attempts to unlock a node via the DisciplineTreeManager.
    /// </summary>
    public bool TryUnlockNode(DisciplineNode node)
    {
        if (node.isUnlocked) return false;
        if (!node.ArePrerequisitesMet()) 
        {
            Debug.LogWarning($"Prerequisites not met for {node.nodeName}");
            return false;
        }

        if (engine.totalPowerPoints >= node.ppCost && 
            storage.CanAffordSpecial(node.requiredMaterial, node.materialAmount))
        {
            treeManager.UnlockNode(node);
            return true;
        }

        Debug.LogWarning($"Cannot afford {node.nodeName}. Need {node.ppCost} PP and {node.materialAmount} {node.requiredMaterial}");
        return false;
    }

    /// <summary>
    /// Gets visual information about a node for UI display.
    /// </summary>
    public string GetNodeDisplayInfo(DisciplineNode node)
    {
        string info = $"<b>{node.nodeName}</b>\n";
        info += $"<size=80%>{node.description}</size>\n\n";
        info += $"<color=yellow>Cost:</color> {node.ppCost} PP + {node.materialAmount}x {node.requiredMaterial}\n";

        if (!node.isUnlocked && node.ArePrerequisitesMet())
            info += "<color=green>[Ready to Unlock]</color>";
        else if (!node.isUnlocked)
            info += "<color=red>[Prerequisites Required]</color>";
        else
            info += "<color=lime>[Unlocked]</color>";

        return info;
    }

    /// <summary>
    /// Displays tree structure in console for debugging.
    /// </summary>
    public void DebugPrintTree(DisciplinePath path)
    {
        var nodesByType = GetNodesByPath(path);
        Debug.Log($"\n<color=cyan>=== {path} Tree ===</color>");

        foreach (NodeType type in new[] { NodeType.Entry, NodeType.BranchA, NodeType.BranchB, NodeType.Utility, NodeType.Merge, NodeType.Mastery })
        {
            if (nodesByType[type].Count > 0)
            {
                Debug.Log($"\n<b>{type}:</b>");
                foreach (DisciplineNode node in nodesByType[type])
                {
                    string status = node.isUnlocked ? "<color=lime>[✓]</color>" : "<color=red>[✗]</color>";
                    Debug.Log($"  {status} {node.nodeName}");
                }
            }
        }
    }
}
