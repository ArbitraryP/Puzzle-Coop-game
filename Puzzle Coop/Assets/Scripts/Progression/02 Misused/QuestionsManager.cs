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

    private int currentScore = 0;
    private int maxScore = 1;

    

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
        remainingQuestions.AddRange(questionSet.Questions);

        NextQuestion();
    }

    [Server]
    private void NextQuestion()
    {
                int randomIndex = Random.Range(0, remainingQuestions.Count - 1);
        float progressAmount = currentScore / maxScore;
        int[] shuffledAnswersOrder = ShuffleIntArray.Shuffe( new int[] { 0, 1, 2, 3 } );
                
        RpcSetUpQuestion(remainingQuestions[randomIndex], isDefaultFormat, progressAmount, shuffledAnswersOrder);

        isDefaultFormat = !isDefaultFormat;
        remainingQuestions.RemoveAt(randomIndex);

    }

    [ClientRpc]
    private void RpcSetUpQuestion(Question question, bool defaultFormat,  float progressAmount, int[] arrangement)
    {
        questionScreen.SetUpQuestion(question, defaultFormat, arrangement);
        questionScreen.SetProgressBar(progressAmount);


    }



}
