using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CalcAnswer", menuName = "Progression/CalcAnswer")]
public class CalcAnswer : ScriptableObject
{
    [SerializeField] private int factor1;
    [SerializeField] private int factor2;
    [SerializeField] private int answer;

    public int Answer => answer;

    public bool CompareFactors(List<int> values) =>
        values.Contains(factor1) && values.Contains(factor2);
    
}