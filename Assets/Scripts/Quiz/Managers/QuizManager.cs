using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class QuizManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _numberOfQuestions;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _quizStarted;
    [SerializeField] private ScriptableEvent _quizFinished;
    [SerializeField] private ScriptableEvent _newQuizQuestion;

    private List<QuizQuestion> _questions;
    private int _currentQuestionIndex = -1;
    private int _numberOfCorrectAnswers = 0;

    private bool _loadingQuestionsCompleted;


    /// <summary>
    /// Event function that responds to new quiz request. It will start new quiz coroutine
    /// </summary>
    public void OnNewQuizRequest()
    {
        StartCoroutine(StartQuiz());
    }

    /// <summary>
    /// Event function that receives list of quiz questions from data manager when they are loaded from json file and selected randomly
    /// </summary>
    /// <param name="listOfAnimalDataMessage"></param>
    public void OnListOfQuestionsReceived(EventMessage listOfQuizQuestionsMessage)
    {
        _questions = ((ListOfQuizQuestionsMessage)listOfQuizQuestionsMessage).QuizQuestionList;
        _loadingQuestionsCompleted = true;
    }

    /// <summary>
    /// Event function that is called when player selects an answer. It will check is that answer correct and if it is it will increase number of correct answers
    /// </summary>
    /// <param name="intMessage"></param>
    public void OnAnswerSelected(EventMessage intMessage)
    {
        int selectedAnswerIndex = ((IntMessage)intMessage).IntValue;

        switch(LocalizationManager.Instance.Language)
        {
            case Enums.Language.English:
                if (_questions[_currentQuestionIndex].CorrectAnswerEN == _questions[_currentQuestionIndex].AnswersEN[selectedAnswerIndex])
                {
                    _numberOfCorrectAnswers++;
                }
                break;
            case Enums.Language.Croatian:
                if (_questions[_currentQuestionIndex].CorrectAnswerHR == _questions[_currentQuestionIndex].AnswersHR[selectedAnswerIndex])
                {
                    _numberOfCorrectAnswers++;
                }
                break;
        }
        


        if (_currentQuestionIndex + 1 > _numberOfQuestions - 1) //If next question is bigger then number of questions (since its index we need to reduce it by 1) then call quiz finished
        {
            QuizFinished();
        }
        else //else get new question
        {
            GetNewQuestion();
        }
        
    }

    /// <summary>
    /// Function that is called when quiz has come to an end. It will raise quiz finished event.
    /// </summary>
    private void QuizFinished()
    {
        _quizFinished.RaiseEvent(new QuizStatsMessage(_numberOfQuestions, _numberOfCorrectAnswers));
    }

    /// <summary>
    /// Function that will get new question for quiz and raise new quiz question event with that question
    /// </summary>
    private void GetNewQuestion()
    {
        _currentQuestionIndex++;
        _newQuizQuestion.RaiseEvent(new QuizQuestionMessage(_questions[_currentQuestionIndex]));
    }

    /// <summary>
    /// Function that selects new questions from json file, raises event that quiz has started and gets first question.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartQuiz()
    {
        //Reseting variables
        _loadingQuestionsCompleted = false;

        _questions = new List<QuizQuestion>();
        _currentQuestionIndex = -1;
        _numberOfCorrectAnswers = 0;

        //Requsting quiz questions
        DataManager.Instance.RequestRandomQuestionsFromJSON(_numberOfQuestions);

        yield return new WaitUntil(() => _loadingQuestionsCompleted == true);

        _quizStarted.RaiseEvent();

        GetNewQuestion();
    }

    /// <summary>
    /// Debug function for creating JSON file
    /// </summary>
    void GenerateQuestion()
    {
        ListOfQuizQuestions listOfQuestions = new ListOfQuizQuestions();
        listOfQuestions.Questions = new List<QuizQuestion>();
        for (int i = 0; i < 15; i++)
        {
            QuizQuestion question = new QuizQuestion();
            question.QuestionEN = "Lalal sajgihasjgasj  ahisgjan ashgiajjs?";
            question.QuestionHR = "SAGsaignaks ioasikgmas asgoj?";

            question.AnswersEN = new string[4];
            question.AnswersEN[0] = "aaaaa";
            question.AnswersEN[1] = "bbbbbbbb";
            question.AnswersEN[2] = "cccccccccccccc";
            question.AnswersEN[3] = "ddddd";

            question.AnswersHR = new string[4];
            question.AnswersHR[0] = "aaaaa";
            question.AnswersHR[1] = "bbbbbbbb";
            question.AnswersHR[2] = "cccccccccccccc";
            question.AnswersHR[3] = "ddddd";

            question.CorrectAnswerEN = "cccccccccccccc";
            question.CorrectAnswerHR = "cccccccccccccc";

            listOfQuestions.Questions.Add(question);
        }

        string json = JsonUtility.ToJson(listOfQuestions);
        Debug.Log(json);
    }
}
