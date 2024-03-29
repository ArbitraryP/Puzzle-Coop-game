using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class QuestionsManager : NetworkBehaviour
{
    public QuestionSet questionSet = null;
    public List<Question> remainingQuestions = null;

    [SerializeField] private UI_QuestionScreen questionScreen = null;
    [SerializeField] private bool isDefaultFormat = true;

    public MapObjectManager_S serverObjectManager = null;

    private int currentScore = 0;
    private int maxScore = 1;

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

        int randomIndex = Random.Range(0, remainingQuestions.Count);
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
    public void CmdSelectAnswer(int answerIndex)
    {
        
        if (currentQuestion.answerIndex == answerIndex)
        {
            currentScore++;
            // Play Correct Answer sound

            if (currentScore == maxScore)
            {
                serverObjectManager.RpcM02_QuizCompleted();
                RpcPlayQuizSound(QuizSound.COMPLETE);
                RpcShowQuizComplete();
            }
            else
                RpcPlayQuizSound(QuizSound.CORRECT);


            NextQuestion();
        }
        else
        {
            // Play Wrong Answer sound
            RpcPlayQuizSound(QuizSound.INCORRECT);
            ResetQuestions();
        }
        
    }

    private enum QuizSound { CORRECT, INCORRECT, COMPLETE };

    [ClientRpc]
    private void RpcPlayQuizSound(QuizSound quizSound)
    {
        switch (quizSound)
        {
            case QuizSound.CORRECT:
                FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M02_CorrectAnswer);
                break;
            case QuizSound.INCORRECT:
                FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M02_IncorrectAnswer);
                break;
            case QuizSound.COMPLETE:
                FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M02_Complete);
                break;
        }
    }

    [ClientRpc]
    private void RpcShowQuizComplete()
    {
        questionScreen.ShowQuizCompleted();
        questionScreen.SetProgressBar(1);
    }

}
