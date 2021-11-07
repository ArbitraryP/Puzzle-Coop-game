using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TangentNodes.Network;
using Mirror;

public class UI_QuestionScreen : MonoBehaviour
{
    [SerializeField] private Button[] buttons = null;
    [SerializeField] private TMP_Text[] buttonTexts = null;
    [SerializeField] private TMP_Text questionText = null;
    [SerializeField] private Image questionImage = null;
    [SerializeField] private Image progressBar = null;
    [SerializeField] private GameObject quizCompleteImage = null;

    [SerializeField] private MapObjectManager_L localObjectManager = null;

    private int[] shuffledChoicesIndex;
    public bool testFormat = true;
    private Question currentQuestion;

    private NetworkManagerTN room;
    private NetworkManagerTN Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerTN;
        }
    }


    // This method will be called to setup a question
    // Two parameters: Question, bool_true Question/Answer
    public void SetUpQuestion(string questionName, bool isDefaultFormat, int[] arrangement)
    {
        // loads answers to answers and shuffles it
        shuffledChoicesIndex = arrangement;
        currentQuestion = Resources.Load<Question>("ScriptableObjects/Questions/" + questionName);

        foreach (NetworkGamePlayerTN player in Room.GamePlayers)
        {
            if (!player.hasAuthority) { continue; }
            if (player.isLeader)
            {
                SetQuestionFormat(isDefaultFormat);
            }
            else
            {
                SetQuestionFormat(!isDefaultFormat);
            }
        }
        
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

            questionText.text = currentQuestion.text;
            questionImage.sprite = currentQuestion.image;
            questionImage.color = Color.white;
        }
        else
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttonTexts[i].text = currentQuestion.choices[shuffledChoicesIndex[i]];
                buttons[i].interactable = false;
            }


            questionText.text = "...";
            questionImage.sprite = null;
            questionImage.color = Color.black;

        }
    }

    public void SetProgressBar(float progressAmount)
    {
        progressBar.fillAmount = progressAmount;
    }


    public void OnClickChoice(int buttonIndex)
    {
        PlayAnswerButtonClick();
        QuestionsManager questionsManager = FindObjectOfType<QuestionsManager>();
        if (!questionsManager) return;
        
        
        questionsManager.CmdSelectAnswer(shuffledChoicesIndex[buttonIndex]);

    }

    public void ShowQuizCompleted()
    {
        quizCompleteImage.SetActive(true);
    }

    public void PlayAnswerButtonClick()
    {
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M02_QuizButton);
    }
}
