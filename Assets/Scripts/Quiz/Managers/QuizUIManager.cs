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
        _quizEndPanelController.TogglePanel(false);
        _quizStartPanelController.TogglePanel(true);
    }

    public void OnQuizStarted()
    {
        _quizEndPanelController.TogglePanel(false);

        _questionPanelController = Instantiate(_quizPanelPrefab, _questionPanelParent).GetComponent<QuestionPanelController>();
        _questionPanelController.TogglePanel(true);
    }

    public void OnNewQuestion(EventMessage quizQuestionMessage)
    {
        QuizQuestion question = ((QuizQuestionMessage)quizQuestionMessage).Question;

        _questionPanelController.AssignNewQuestion(question);
    }

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

    public void OnStartNewQuizButtonPressed()
    {
        _requestNewQuiz.RaiseEvent();
    }

    public void OnBackToMainMenuButtonPressed()
    {
        _loadMainMenu.RaiseEvent();
    }
}
