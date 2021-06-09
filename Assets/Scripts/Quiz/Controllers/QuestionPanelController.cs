using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPanelController : BaseGUIController
{
    [Header("Internal References")]
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private Button[] _answerButtons;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _answerSelected;


    private void Start()
    {
        AssignOnClickFunctionsToAnswerButtons();
    }


    public void AssignNewQuestion(QuizQuestion question)
    {
        SetupQuestion(question);
    }


    /// <summary>
    /// Sets up question text and answer buttons
    /// </summary>
    /// <param name="question"></param>
    private void SetupQuestion(QuizQuestion question)
    {
        _questionText.text = question.Question;

        for (int i = 0; i < _answerButtons.Length; i++)
        {
            if (question.Answers.Length < i+1)
            {
                _answerButtons[i].gameObject.SetActive(false);
            }
            else
            {
                _answerButtons[i].gameObject.SetActive(true);
                _answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.Answers[i];
            }
        }

    }

    /// <summary>
    /// Called by answer button. It will trigger AnswerSelected event for QuizManager
    /// </summary>
    /// <param name="isItCorrectAnswer"></param>
    private void OnAnswerSelected(int answerIndex)
    {
        _answerSelected.RaiseEvent(new IntMessage(answerIndex));
    }

    private void AssignOnClickFunctionsToAnswerButtons()
    {
        for (int i = 0; i < _answerButtons.Length; i++)
        {
            int index = i;
            _answerButtons[i].onClick.AddListener(delegate { OnAnswerSelected(index); });
        }
    }
}
