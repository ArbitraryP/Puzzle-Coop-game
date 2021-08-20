using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TangentNodes.Network;

public class QuestionsManager : NetworkBehaviour
{
    public QuestionSet questionSet = null;
    public List<Question> remainingQuestions = null;

    [SerializeField] private UI_QuestionScreen questionScreen = null;
    [SerializeField] private bool isDefaultFormat = true;

    public int currentScore = 0;
    public int maxScore = 1;

    private Question currentQuestion = null;

    private void Awake()
    {
        questionScreen = FindObjectOfType<UI_QuestionScreen>();
    }

    [Server]
    public void InitializeQuestions(int currentMapIndex)
    {
        var AllSets = Resources.LoadAll<QuestionSet>("ScriptableObjects/Questions");

        foreach (var set in AllSets)
        {
            if (set.AssociateMap.Index != currentMapIndex) continue;
            questionSet = set;
        }
    } 




    [Server]
    public void ResetQuestions()
    {
        currentScore = 0;
        maxScore = questionSet.Questions.Count;

        remainingQuestions.Clear();
        remainingQuestions.AddRange(questionSet.Questions);
        NextQuestion();
    }

    [Server]
    private void NextQuestion()
    {
        if (remainingQuestions.Count <= 0) return;

        int randomIndex = Random.Range(0, remainingQuestions.Count - 1);
        float progressAmount = (float)currentScore / (float)maxScore;
        int[] shuffledAnswersOrder = ShuffleIntArray.Shuffe( new int[] { 0, 1, 2, 3 } );

        currentQuestion = remainingQuestions[randomIndex];

        RpcSetUpQuestion(currentQuestion.name, isDefaultFormat, progressAmount, shuffledAnswersOrder);

        isDefaultFormat = !isDefaultFormat;
        remainingQuestions.RemoveAt(randomIndex);

    }

    [ClientRpc]
    private void RpcSetUpQuestion(string questionName, bool defaultFormat,  float progressAmount, int[] arrangement)
    {
        questionScreen.SetUpQuestion(questionName, defaultFormat, arrangement);
        questionScreen.SetProgressBar(progressAmount);


    }


    [Command(requiresAuthority = false)]
    public void CmdSelectAnswer()
    {
        //Temp Code just testing to move questions
        
        currentScore++;

        if (remainingQuestions.Count <= 0)
            ResetQuestions();
        else
            NextQuestion();
    }

}
