using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement", menuName = "Progression/Achievement")]
public class Achievement : ScriptableObject
{
    [SerializeField]
    private int index;
    public int Index
    {
        get { return index; }
    }

    public new string name;
    public string description;
    public Sprite icon;

}
