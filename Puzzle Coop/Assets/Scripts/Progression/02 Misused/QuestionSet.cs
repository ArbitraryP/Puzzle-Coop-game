using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Question Set", menuName = "Progression/QuestionSet")]
public class QuestionSet : ScriptableObject
{
    [SerializeField] private List<Question> questions;
    [SerializeField] private Map associateMap;

    public List<Question> Questions => questions;
    public Map AssociateMap => associateMap;
}
