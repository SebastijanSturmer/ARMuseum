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


    [Header("References")]
    [SerializeField] private AssetReference _jsonQuestions;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _quizStarted;
    [SerializeField] private ScriptableEvent _quizFinished;
    [SerializeField] private ScriptableEvent _newQuizQuestion;

    private List<QuizQuestion> _selectedQuestions;
    private int _currentQuestionIndex = -1;
    private int _numberOfCorrectAnswers = 0;

    private bool _loadingQuestionsCompleted;



    public void OnNewQuizRequest()
    {
        StartCoroutine(StartQuiz());
    }

    public void OnAnswerSelected(EventMessage intMessage)
    {
        int selectedAnswerIndex = ((IntMessage)intMessage).IntValue;

        switch(LocalizationManager.Instance.Language)
        {
            case Enums.Language.English:
                if (_selectedQuestions[_currentQuestionIndex].CorrectAnswerEN == _selectedQuestions[_currentQuestionIndex].AnswersEN[selectedAnswerIndex])
                {
                    _numberOfCorrectAnswers++;
                }
                break;
            case Enums.Language.Croatian:
                if (_selectedQuestions[_currentQuestionIndex].CorrectAnswerHR == _selectedQuestions[_currentQuestionIndex].AnswersHR[selectedAnswerIndex])
                {
                    _numberOfCorrectAnswers++;
                }
                break;
        }
        


        if (_currentQuestionIndex + 1 > _numberOfQuestions - 1)
        {
            QuizFinished();
        }
        else
        {
            GetNewQuestion();
        }
        
    }

    private void QuizFinished()
    {
        _quizFinished.RaiseEvent(new QuizStatsMessage(_numberOfQuestions, _numberOfCorrectAnswers));
    }

    private void GetNewQuestion()
    {
        _currentQuestionIndex++;
        _newQuizQuestion.RaiseEvent(new QuizQuestionMessage(_selectedQuestions[_currentQuestionIndex]));
    }

    /// <summary>
    /// Reads questions from json and randomly assigns questions that will be used in quiz
    /// </summary>
    private void GetQuestionsFromJSON()
    {
        _loadingQuestionsCompleted = false;

        _selectedQuestions = new List<QuizQuestion>();
        _currentQuestionIndex = -1;
        _numberOfCorrectAnswers = 0;

        if (!_jsonQuestions.RuntimeKeyIsValid())
        {
            Debug.LogError("QuizManager : Json Questions runtime key is not valid!");
            return;
        }


        _jsonQuestions.LoadAssetAsync<TextAsset>().Completed += handle =>
        {
            ListOfQuizQuestions questions = JsonUtility.FromJson<ListOfQuizQuestions>(handle.Result.text);
            

            int tryAttempts = 0;
            while (true)
            {
                if (tryAttempts > 100) //If we tried 100 times to get random question but failed lets just go by order and assign rest of them
                {
                    for (int i = 0; i < questions.Questions.Count; i++)
                    {
                        if (_selectedQuestions.Contains(questions.Questions[i])) //If we already have that question then dont add it
                            continue;

                        if (_selectedQuestions.Count >= _numberOfQuestions) //If we filled all the questions break!
                            break;

                        _selectedQuestions.Add(questions.Questions[i]);
                    }

                    break;
                }

                QuizQuestion randomSelectedQuestion = questions.Questions[UnityEngine.Random.Range(0, questions.Questions.Count)];
                if (_selectedQuestions.Contains(randomSelectedQuestion)) //If we already have that question then dont add it
                {
                    tryAttempts++;
                    continue;
                }

                if (_selectedQuestions.Count >= _numberOfQuestions) //If we filled all the questions break!
                    break;

                _selectedQuestions.Add(randomSelectedQuestion);

            }

            _loadingQuestionsCompleted = true;

            Addressables.Release(handle);
        };

    }

    private IEnumerator StartQuiz()
    {
        GetQuestionsFromJSON();
        yield return new WaitUntil(() => _loadingQuestionsCompleted == true);

        _quizStarted.RaiseEvent();

        GetNewQuestion();
    }

    /// <summary>
    /// Debug function for looking at JSON format
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
