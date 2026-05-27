using UnityEngine;
using System;

[System.Serializable]
public class TechNode
{
    public string id;          // e.g., "y_1"
    public string nodeName;    // e.g., "DataYield1"
    public string title;
    public string description;
    public double cost;
    public string prerequisiteIds;
    public bool isPurchased;
    
    public bool IsUnlocked(System.Collections.Generic.List<TechNode> allNodes)
    {
        if (string.IsNullOrEmpty(prerequisiteIds)) return true;
        
        string[] requirements = prerequisiteIds.Split(',');
        foreach (string reqId in requirements)
        {
            var preReq = allNodes.Find(n => n.id == reqId.Trim());
            if (preReq == null || !preReq.isPurchased) return false;
        }
        return true;
    }
}