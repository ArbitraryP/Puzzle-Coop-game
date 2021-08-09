using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_QuestionScreen : MonoBehaviour
{
    [SerializeField] private Button[] buttons = null;
    [SerializeField] private TMP_Text[] buttonTexts = null;
    [SerializeField] private TMP_Text questionText = null;
    [SerializeField] private Image questionImage = null;
    [SerializeField] private Image progressBar = null;

    // Declare a variable that holds the current question

    private int[] answers;

    public bool testFormat = true;

    // This method will be called to setup a question
    // Two parameters: Question, bool_true Question/Answer
    public void SetUpQuestion()
    {

        // loads answers to answers and shuffles it
    }

    public void SetQuestionFormat(bool isDefault)
    {
        if (isDefault)
        {
            buttonTexts[0].text = "A";
            buttonTexts[1].text = "B";
            buttonTexts[2].text = "C";
            buttonTexts[3].text = "D";

            foreach (Button button in buttons)
                button.interactable = true;

            questionText.text = "...";
            questionImage.color = Color.black;
        }
        else
        {
            foreach(TMP_Text buttonText in buttonTexts)
                buttonText.text = "Answer";
            foreach (Button button in buttons)
                button.interactable = false;

            questionText.text = "This is the Question";
            questionImage.color = Color.white;
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            testFormat = !testFormat;
            SetQuestionFormat(testFormat);
        }
    }
}
