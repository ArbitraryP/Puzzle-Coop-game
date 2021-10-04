using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sentence Set", menuName = "Progression/SentenceSet")]
public class SentenceSet : ScriptableObject
{
    [SerializeField] private List<Sentence> sentences;
    [SerializeField] private Map associateMap;
    [SerializeField] private bool showCalendarHideClock = false;

    [Header("Solution")]
    [SerializeField] private int correctMonth = 8;
    [SerializeField] private int correctDate = 15;
    [SerializeField] private int correctHour = 8;
    [SerializeField] private int correctMinutes = 59;
    [SerializeField] private bool correctMeridiemAM = true;

    public List<Sentence> Sentences => sentences;
    public Map AssociateMap => associateMap;
    public bool ShowCalendarHideClock => showCalendarHideClock;

    public int CorrectMonth => correctMonth;
    public int CorrectDate => correctDate;
    public int CorrectHour => correctHour;
    public int CorrectMinutes => correctMinutes;
    public bool CorrectMeridiemAM => correctMeridiemAM;
}
