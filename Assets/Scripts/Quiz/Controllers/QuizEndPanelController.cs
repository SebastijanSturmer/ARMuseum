using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuizEndPanelController : BaseGUIController
{
    [Header("Internal references")]
    [SerializeField] private TextMeshProUGUI _endResultText;

    private QuizEndResults _endResults;

    public void SetEndResults(QuizEndResults endResults)
    {
        _endResults = endResults;

        UpdateGUI();
    }

    private void UpdateGUI()
    {
        _endResultText.text = _endResults.NumberOfCorrectAnswers + " / " + _endResults.NumberOfQuestions;
    }
}
