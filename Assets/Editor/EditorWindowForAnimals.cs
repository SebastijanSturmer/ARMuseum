using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;

public class EditorWindowForAnimals : EditorWindow
{
    private List<AnimalData> _animals;

    private string _pathToAnimalsJsons = "";
    private bool _needsRefresh = true;

    private TextAsset _jsonFile;
    private Enums.Language _currentLanguage = Enums.Language.English;

    private Vector2 _scrollPosition = new Vector2();

    [MenuItem("Custom/Animals")]
    public static void ShowWindow()
    {
        GetWindow<EditorWindowForAnimals>("Animals");

    }

    private void OnGUI()
    {
        GUIStyle animalNameStyle = new GUIStyle();
        animalNameStyle.fontSize = 20;
        animalNameStyle.normal.textColor = Color.white;
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

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        if (_animals != null)
        {
            for (int i = 0; i < _animals.Count; i++)
            {
                GUILayout.BeginHorizontal();
                //Remove animal
                if (GUILayout.Button("-", GUILayout.Width(50), GUILayout.Height(50)))
                {
                    _animals.RemoveAt(i);
                    break;
                }


                GUILayout.BeginVertical();
                GUILayout.Label(_animals[i].AnimalIdentifier, animalNameStyle);
                //Animal identifier
                _animals[i].AnimalIdentifier = EditorGUILayout.TextField("Animal identifier (english name) : ", _animals[i].AnimalIdentifier);

                //Animal name
                _animals[i].AnimalName = EditorGUILayout.TextField("Animal name : ", _animals[i].AnimalName);

                //Animal basic info
                GUILayout.BeginHorizontal();
                GUILayout.Label("Animal basic info : ");
                _animals[i].BasicInfo = EditorGUILayout.TextArea(_animals[i].BasicInfo, GUILayout.MinHeight(100));
                GUILayout.EndHorizontal();

                //Animal short info tabs
                for (int j = 0; j < _animals[i].ShortInfo.Length; j++)
                {
                    GUILayout.BeginHorizontal();
                    _animals[i].ShortInfo[j].Key = EditorGUILayout.TextField("", _animals[i].ShortInfo[j].Key);
                    _animals[i].ShortInfo[j].Value = EditorGUILayout.TextField("", _animals[i].ShortInfo[j].Value);

                    if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
                    {
                        ShortInfoStruct[] newStruct = new ShortInfoStruct[_animals[i].ShortInfo.Length - 1];

                        int index = 0;
                        for (int k = 0; k < _animals[i].ShortInfo.Length; k++)
                        {
                            if (k == j)
                                continue;

                            newStruct[index] = _animals[i].ShortInfo[k];
                            index++;
                        }

                        _animals[i].ShortInfo = newStruct;
                    }

                    GUILayout.EndHorizontal();
                }

                //New short info tab
                if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    ShortInfoStruct[] newStruct = new ShortInfoStruct[_animals[i].ShortInfo.Length + 1];
                    _animals[i].ShortInfo.CopyTo(newStruct,0);
                    _animals[i].ShortInfo = newStruct;
                }

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                //If we have another animal after this one then add some space between them
                if (i+1 < _animals.Count)
                    GUILayout.Space(50);
            }

            //New animal
            if (GUILayout.Button("New animal"))
            {
                AnimalData newAnimalData = new AnimalData();
                newAnimalData.ShortInfo = new ShortInfoStruct[1];
                _animals.Add(newAnimalData);
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
    /// Loads list of animals from json file and refreshes animals variable
    /// </summary>
    private void RefreshFiles()
    {
        _needsRefresh = false;

        _pathToAnimalsJsons = Application.dataPath + "/Data/AR/";

        using (StreamReader r = new StreamReader(_pathToAnimalsJsons + "animals" + _currentLanguage + ".json"))
        {
            string json = r.ReadToEnd();

            ListOfAnimalData list = JsonConvert.DeserializeObject<ListOfAnimalData>(json);

            _animals = list.Animals;
        }

    }

    /// <summary>
    /// Saves animals variable to json file
    /// </summary>
    private void SaveFiles()
    {
        _needsRefresh = false;

        _pathToAnimalsJsons = Application.dataPath + "/Data/AR/";

        using (StreamWriter w = new StreamWriter(_pathToAnimalsJsons + "animals"+_currentLanguage+".json"))
        {

            ListOfAnimalData list = new ListOfAnimalData();
            list.Animals = _animals;
            
            w.Write(JsonConvert.SerializeObject(list));
        }

    }
}
