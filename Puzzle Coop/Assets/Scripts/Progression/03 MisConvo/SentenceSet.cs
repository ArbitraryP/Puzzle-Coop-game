using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sentence Set", menuName = "Progression/SentenceSet")]
public class SentenceSet : ScriptableObject
{
    [SerializeField] private List<Sentence> sentences;
    [SerializeField] private Map associateMap;

    public List<Sentence> Sentences => sentences;
    public Map AssociateMap => associateMap;
}
