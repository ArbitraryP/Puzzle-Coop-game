using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map", menuName = "Progression/Map")]
public class Map : ScriptableObject
{
    [SerializeField]
    private int index;
    public int Index
    {
        get { return index; }
    }

    public List<Map> prerequisite;
    public List<Map> unlocks;

    public string displayName;

    [Range(1, 5)]
    public int difficulty;
    [TextArea(8,15)]
    public string description;

    public List<Achievement> achievments;

    [Scene]
    public string Scene;

    public bool IsPrerequisiteMet(List<int> completedMaps, List<int> unlockedMaps)
    {
        if (prerequisite.Count <= 0 || prerequisite == null)
            return true;

        if (!unlockedMaps.Contains(Index))
            return false;

        foreach (Map prereqMap in prerequisite)
        {
            if (!completedMaps.Contains(prereqMap.Index) ||
                !unlockedMaps.Contains(prereqMap.Index))
                return false;
        }
        
        return true;
    }
}
