using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Career Set", menuName = "Progression/CareerSet")]
public class CareerSet : ScriptableObject
{
    [SerializeField] private List<Career> careers;
    [SerializeField] private Map associateMap;

    public List<Career> Careers => careers;
    public Map AssociateMap => associateMap;
}
