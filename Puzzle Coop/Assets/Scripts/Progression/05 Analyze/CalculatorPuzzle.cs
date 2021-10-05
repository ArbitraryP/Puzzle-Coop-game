using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Experimental.Rendering.Universal;

public class CalculatorPuzzle : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button buttonEnter = null;
    [SerializeField] private Button[] buttonChoices = null;
    [SerializeField] private Button[] buttonFactors = null;
    [SerializeField] private TMP_Text[] textFactors = null;
    [SerializeField] private TMP_Text textAnswer = null;
    [SerializeField] private Light2D light2DCalc = null;

    [Header("Solution")]
    [SerializeField] private List<CalcAnswer> calcAnswerList = null;
    [SerializeField] private CalcAnswer currentCalcAnswer = null;

    [Header("Data")]
    [SerializeField] private List<int> currentFactors = null;
    [SerializeField] private int selectedFactorIndex = 0;

    private void Start()
    {
        currentFactors.Add(0);
        currentFactors.Add(0);

        RandomizeCalcAnswer();
    }

    private void RandomizeCalcAnswer()
    {
        currentCalcAnswer = calcAnswerList[Random.Range(0, calcAnswerList.Count)];
        textAnswer.text = currentCalcAnswer.Answer.ToString();
    }

    private void ClearFactors()
    {
        currentFactors[0] = 0;
        currentFactors[1] = 0;
        textFactors[0].text = "x";
        textFactors[1].text = "y";
    }

    public void OnClickButtonFactor(int buttonIndex)
    {
        selectedFactorIndex = buttonIndex;
        foreach (Button button in buttonFactors)
            button.interactable = true;

        buttonFactors[selectedFactorIndex].interactable = false;
    }

    public void OnClickButtonChoice(int value)
    {
        currentFactors[selectedFactorIndex] = value;
        textFactors[selectedFactorIndex].text = value.ToString();

    }

    public void OnClickEnter()
    {
        if (!currentCalcAnswer.CompareFactors(currentFactors))
        {
            ClearFactors();
            RandomizeCalcAnswer();

            // Play Wrong Answer Sound
            return;
        }

        Debug.Log("Password Correct");

        List<Button> allButtons = new List<Button>(buttonFactors);
        allButtons.AddRange(buttonChoices);
        allButtons.Add(buttonEnter);
        foreach(Button button in allButtons)
            button.interactable = false;


        light2DCalc.color = Color.green;

        // Play Correct Sound and Call Server to Unlock
        FindObjectOfType<MapObjectManager_S>()?.CmdUnlockDoors();
    }




}
