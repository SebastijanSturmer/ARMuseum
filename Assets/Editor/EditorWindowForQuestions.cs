using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;

public class EditorWindowForQuestions : EditorWindow
{
    private List<QuizQuestion> _questions;

    private string _pathToQuestionsJsons = "";
    private bool _needsRefresh = true;

    private TextAsset _jsonFile;
    private Enums.Language _currentLanguage = Enums.Language.English;

    private Vector2 _scrollPosition = new Vector2();

    [MenuItem("Custom/Questions")]
    public static void ShowWindow()
    {
        GetWindow<EditorWindowForQuestions>("Questions");

    }

    private void OnGUI()
    {
        minSize = new Vector2(100, 100);


        if (GUILayout.Button("Refresh"))
        {
            RefreshFiles();
        }

        //Language buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("English"))
        {
            _currentLanguage = Enums.Language.English;
            RefreshFiles();
        }
        if(GUILayout.Button("Croatian"))
        {
            _currentLanguage = Enums.Language.Croatian;
            RefreshFiles();
            
        }
        GUILayout.EndHorizontal();

        //Questions
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        if (_questions != null)
        {
            for (int i = 0; i < _questions.Count; i++)
            {

                GUILayout.BeginHorizontal();
                //Remove animal
                if (GUILayout.Button("-", GUILayout.Width(50), GUILayout.Height(50)))
                {
                    _questions.RemoveAt(i);
                    break;
                }


                GUILayout.BeginVertical();

                GUILayout.Label("Question number : " + i);
                _questions[i].Question = EditorGUILayout.TextField("Question : ", _questions[i].Question);

                //Answers
                GUILayout.Label("Answers");
                for (int j = 0; j < _questions[i].Answers.Length; j++)
                {
                    GUILayout.BeginHorizontal();
                    _questions[i].Answers[j] = EditorGUILayout.TextField(_questions[i].Answers[j]);

                    if (GUILayout.Button("-", GUILayout.MaxWidth(30)))
                    {
                        string[] newAnswers = new string[_questions[i].Answers.Length - 1];

                        int index = 0;
                        for (int k = 0; k < _questions[i].Answers.Length; k++)
                        {
                            if (k == j)
                                continue;

                            newAnswers[index] = _questions[i].Answers[k];
                            index++;
                        }

                        _questions[i].Answers = newAnswers;
                    }
                    GUILayout.EndHorizontal();
                }

                //New answer
                if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    string[] newAnswers = new string[_questions[i].Answers.Length + 1];
                    _questions[i].Answers.CopyTo(newAnswers,0);
                }

                //Correct answer
                GUILayout.Space(10);
                _questions[i].CorrectAnswer = EditorGUILayout.TextField("Correct answer : ", _questions[i].CorrectAnswer);


                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                //If we have another question after this one then add some space between them
                if (i+1 < _questions.Count)
                    GUILayout.Space(50);
            }

            //New question
            if (GUILayout.Button("New question"))
            {
                QuizQuestion newQuestion = new QuizQuestion();
                newQuestion.Answers = new string[0];
                _questions.Add(newQuestion);
            }

        }

        EditorGUILayout.EndScrollView();

        //Save
        if (GUILayout.Button("Save"))
        {
            SaveFiles();
        }

    }


    /// <summary>
    /// Loads list of questions from json file and refreshes questions variable
    /// </summary>
    private void RefreshFiles()
    {
        _needsRefresh = false;

        _pathToQuestionsJsons = Application.dataPath + "/Data/Quiz/";

        using (StreamReader r = new StreamReader(_pathToQuestionsJsons + "questions" + _currentLanguage + ".json"))
        {
            string json = r.ReadToEnd();

            ListOfQuizQuestions list = JsonConvert.DeserializeObject<ListOfQuizQuestions>(json);

            _questions = list.Questions;
        }

    }

    /// <summary>
    /// Saves questions variable to json file
    /// </summary>
    private void SaveFiles()
    {
        _needsRefresh = false;

        _pathToQuestionsJsons = Application.dataPath + "/Data/Quiz/";

        using (StreamWriter w = new StreamWriter(_pathToQuestionsJsons + "questions" + _currentLanguage+".json"))
        {

            ListOfQuizQuestions list = new ListOfQuizQuestions();
            list.Questions = _questions;
            
            w.Write(JsonConvert.SerializeObject(list));
        }

    }
}
