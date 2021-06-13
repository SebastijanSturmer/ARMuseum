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
    private Coroutine _startQuizCoroutine;


    /// <summary>
    /// Event function that responds to new quiz request. It will start new quiz coroutine
    /// </summary>
    public void OnNewQuizRequest()
    {
        if (_startQuizCoroutine != null)
            StopCoroutine(_startQuizCoroutine);

        _startQuizCoroutine = StartCoroutine(StartQuiz());
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

        if (_questions[_currentQuestionIndex].CorrectAnswer == _questions[_currentQuestionIndex].Answers[selectedAnswerIndex])
            _numberOfCorrectAnswers++;
        


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

}
