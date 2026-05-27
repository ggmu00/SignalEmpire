using UnityEngine;

public enum DisciplinePath { Power, Clarity, Logic, Discovery }
public enum NodeType { Entry, BranchA, BranchB, Utility, Merge, Mastery }

[CreateAssetMenu(fileName = "NewNode", menuName = "Economy/Discipline Node")]
public class DisciplineNode : ScriptableObject
{
    [Header("Identity")]
    public string nodeName;
    public DisciplinePath discipline;
    public NodeType type;
    [TextArea] public string description;

    [Header("Costs")]
    public int ppCost;
    public Material requiredMaterial;
    public int materialAmount;

    [Header("Prerequisites")]
    public DisciplineNode[] prerequisites; // Node 9 will require multiple entries

    [Header("State")]
    public bool isUnlocked = false;

    // This checks if the player can even see/buy this node
    public bool ArePrerequisitesMet()
    {
        if (prerequisites == null || prerequisites.Length == 0) return true;
        foreach (var p in prerequisites)
        {
            if (!p.isUnlocked) return false;
        }
        return true;
    }
}