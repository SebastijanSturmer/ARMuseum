using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizUIManager : MonoBehaviour
{
    [Header("Internal References")]
    [SerializeField] private Transform _questionPanelParent;
    [SerializeField] private QuizStartPanelController _quizStartPanelController;
    [SerializeField] private QuizEndPanelController _quizEndPanelController;

    [Header("External References")]
    [SerializeField] private GameObject _quizPanelPrefab;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _requestNewQuiz;
    [SerializeField] private ScriptableEvent _loadMainMenu;


    private QuestionPanelController _questionPanelController;


    private void Start()
    {
        //Toggling start panel and deactivating end panel.
        _quizEndPanelController.TogglePanel(false);
        _quizStartPanelController.TogglePanel(true);
    }

    /// <summary>
    /// Event function that is called when quiz starts. It will close start and end panels and instantiate question panel.
    /// </summary>
    public void OnQuizStarted()
    {
        _quizStartPanelController.TogglePanel(false);
        _quizEndPanelController.TogglePanel(false);

        _questionPanelController = Instantiate(_quizPanelPrefab, _questionPanelParent).GetComponent<QuestionPanelController>();
        _questionPanelController.TogglePanel(true);
    }

    public void OnDidUserAnswerCorrectly(EventMessage intAndBoolMessage)
    {
        IntAndBoolMessage intAndBoolValue = (IntAndBoolMessage)intAndBoolMessage;
        bool didUserAnswerCorrectly = intAndBoolValue.BoolValue;
        int answerButtonIndex = intAndBoolValue.IntValue;

        _questionPanelController.UpdateAnswerColor(answerButtonIndex, didUserAnswerCorrectly);
    }

    /// <summary>
    /// Event function that is called when Quiz manager selects new question. It will assign new question to question panel.
    /// </summary>
    /// <param name="quizQuestionMessage"></param>
    public void OnNewQuestion(EventMessage quizQuestionMessage)
    {
        QuizQuestion question = ((QuizQuestionMessage)quizQuestionMessage).Question;

        _questionPanelController.ResetAnswerColors();
        _questionPanelController.AssignNewQuestion(question);
    }

    /// <summary>
    /// Event function that is called when quiz finishes. It will show end panel with end results and destroy questions panel.
    /// </summary>
    /// <param name="quizStatsMessage"></param>
    public void OnQuizFinished(EventMessage quizStatsMessage)
    {
        var statsMessage = (QuizStatsMessage)quizStatsMessage;

        QuizEndResults endResults = new QuizEndResults();
        endResults.NumberOfCorrectAnswers = statsMessage.NumberOfCorrectAnswers;
        endResults.NumberOfQuestions = statsMessage.NumberOfQuestions;

        _quizEndPanelController.SetEndResults(endResults);

        Destroy(_questionPanelController.gameObject);

        _quizEndPanelController.TogglePanel(true);
    }

    /// <summary>
    /// Function that will request new quiz from quiz manager
    /// </summary>
    public void OnStartNewQuizButtonPressed()
    {
        _requestNewQuiz.RaiseEvent();
    }

    /// <summary>
    /// Loads Main Menu scene
    /// </summary>
    public void OnBackToMainMenuButtonPressed()
    {
        _loadMainMenu.RaiseEvent();
    }

}
