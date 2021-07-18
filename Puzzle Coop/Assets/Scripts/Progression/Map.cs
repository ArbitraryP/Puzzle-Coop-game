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

    public string displayName;

    [Range(1, 5)]
    public int difficulty;
    public List<Achievement> achievments;

    [Scene]
    public string Scene;

}