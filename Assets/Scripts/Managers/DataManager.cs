using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataManager : MonoBehaviour
{

    [Header("JSON files")]
    [SerializeField] private AssetReference _questionsJSON;
    [SerializeField] private AssetReference _animalsJSON;

    [Header("Animals additional data")]
    [SerializeField] private List<AnimalNameAndImage> _animalImages;
    [SerializeField] private List<AnimalNameAndMap> _animalMaps;
    [SerializeField] private List<AnimalNameAndPrefab> _availableAnimalPrefabs;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _listOfQuestionsFromJSONCompleted;
    [SerializeField] private ScriptableEvent _listOfAnimalsFromJSONCompleted;
    [SerializeField] private ScriptableEvent _onImageAndMapOfAnimalCompleted;
    [SerializeField] private ScriptableEvent _requestSound;

    public static DataManager Instance;
    private AsyncOperationHandle<Texture2D> _animalImageHandle;
    private AsyncOperationHandle<Texture2D> _animalMapHandle;

    public List<AnimalNameAndPrefab> AvailableAnimalPrefabs { get => _availableAnimalPrefabs; }

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Gets animal image (texture 2d) for specified animal name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetAnimalPrefabByName(string name)
    {
        for (int i = 0; i < _animalImages.Count; i++)
        {
            if (_animalImages[i].AnimalName == name)
            {
                return _availableAnimalPrefabs[i].AnimalPrefab;
            }
        }
        Debug.LogError("DataManager : There was no animal image for " + name);
        return null;
    }

    public ScriptableEvent GetRequestSoundEvent()
    {
        return _requestSound;
    }

    /// <summary>
    /// Requests animal image and map for animal by name. It requires event listener with OnImageAndMapOfAnimalCompleted event as callback!
    /// </summary>
    /// <param name="name"></param>
    public void RequestAnimalImageAndMapByName(string name)
    {
        StartCoroutine(LoadAnimalImageAndMapCoroutine(name));
    }

    /// <summary>
    /// Loads animal image and map from addressables and then raises OnImageAndMapOfAnimalCompleted event with them
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadAnimalImageAndMapCoroutine(string name)
    {
        //Release last used addresables for image and map
        if (_animalMapHandle.IsValid())
            Addressables.Release(_animalMapHandle);
        if (_animalImageHandle.IsValid())
            Addressables.Release(_animalImageHandle);

        AssetReference imageRef = null;
        AssetReference mapRef = null;

        for (int i = 0; i < _animalImages.Count; i++)
        {
            if (_animalImages[i].AnimalName == name)
            {
                imageRef = _animalImages[i].AnimalImage;
                break;
            }
        }
        for (int i = 0; i < _animalMaps.Count; i++)
        {
            if (_animalMaps[i].AnimalName == name)
            {
                mapRef = _animalMaps[i].AnimalMap;
                break;
            }
        }

        if (imageRef == null || mapRef == null)
        {
            Debug.LogError("DataManager : Image reference or map reference for " + name + " is null!");
            yield break;
        }

        if (!imageRef.RuntimeKeyIsValid() || !mapRef.RuntimeKeyIsValid())
        {
            Debug.LogError("DataManager : Image reference or map reference runtime key is not valid!");
            yield break;
        }

        Sprite image = null;
        Sprite map = null;

        //Load image and map from Addresable reference
        _animalImageHandle = Addressables.LoadAssetAsync<Texture2D>(imageRef);
        yield return _animalImageHandle;
        image = Sprite.Create(_animalImageHandle.Result, new Rect(0, 0, _animalImageHandle.Result.width, _animalImageHandle.Result.height), new Vector2(0.5f, 0.5f), 100.0f);

        _animalMapHandle = Addressables.LoadAssetAsync<Texture2D>(mapRef);
        yield return _animalMapHandle;
        map = Sprite.Create(_animalMapHandle.Result, new Rect(0, 0, _animalMapHandle.Result.width, _animalMapHandle.Result.height), new Vector2(0.5f, 0.5f), 100.0f);

        _onImageAndMapOfAnimalCompleted.RaiseEvent(new AnimalImageAndMapMessage(image, map));
        
    }

    /// <summary>
    /// Function that reads animals data from Animals Addressable asset defined in settings and raises event with that list.
    /// </summary>
    public void RequestAnimalsFromJSON()
    {
        List<AnimalData> animalsFromJSON = new List<AnimalData>();

        if (!_animalsJSON.RuntimeKeyIsValid())
        {
            Debug.LogError("DataManager : Json animals runtime key is not valid!");
            return;
        }

        _animalsJSON.LoadAssetAsync<TextAsset>().Completed += handle =>
        {

            var dictionaryOfAnimals = JsonConvert.DeserializeObject<Dictionary<string, List<AnimalData>>>(handle.Result.text);

            ListOfAnimalData animals = new ListOfAnimalData();
            animals.Animals = dictionaryOfAnimals["Animals"];

            //ListOfAnimalData animals = JsonUtility.FromJson<ListOfAnimalData>(handle.Result.text);

            for (int i = 0; i < animals.Animals.Count; i++)
            {
                animalsFromJSON.Add(animals.Animals[i]);
            }

            _listOfAnimalsFromJSONCompleted.RaiseEvent(new ListOfAnimalDataMessage(animalsFromJSON));

            Addressables.Release(handle);
        };

    }

    /// <summary>
    /// Reads questions from json and raises event with list of questions
    /// </summary>
    public void RequestRandomQuestionsFromJSON(int numberOfQuestions)
    {
        if (!_questionsJSON.RuntimeKeyIsValid())
        {
            Debug.LogError("QuizManager : Json Questions runtime key is not valid!");
            return;
        }

        List<QuizQuestion> randomSelectedQuestions = new List<QuizQuestion>();

        _questionsJSON.LoadAssetAsync<TextAsset>().Completed += handle =>
        {

            var dictionaryOfQuestions = JsonConvert.DeserializeObject<Dictionary<string, List<QuizQuestion>>>(handle.Result.text);

            ListOfQuizQuestions questions = new ListOfQuizQuestions();
            questions.Questions = dictionaryOfQuestions["Questions"];


            //ListOfQuizQuestions questions = JsonUtility.FromJson<ListOfQuizQuestions>(handle.Result.text);


            int tryAttempts = 0;
            while (true)
            {
                if (tryAttempts > 100) //If we tried 100 times to get random question but failed lets just go by order and assign rest of them
                {
                    for (int i = 0; i < questions.Questions.Count; i++)
                    {
                        if (randomSelectedQuestions.Contains(questions.Questions[i])) //If we already have that question then dont add it
                            continue;

                        if (randomSelectedQuestions.Count >= numberOfQuestions) //If we filled all the questions break!
                            break;

                        randomSelectedQuestions.Add(questions.Questions[i]);
                    }

                    break;
                }

                QuizQuestion randomSelectedQuestion = questions.Questions[UnityEngine.Random.Range(0, questions.Questions.Count)];
                if (randomSelectedQuestions.Contains(randomSelectedQuestion)) //If we already have that question then dont add it
                {
                    tryAttempts++;
                    continue;
                }

                if (randomSelectedQuestions.Count >= numberOfQuestions) //If we filled all the questions break!
                    break;

                randomSelectedQuestions.Add(randomSelectedQuestion);

            }

            _listOfQuestionsFromJSONCompleted.RaiseEvent(new ListOfQuizQuestionsMessage(randomSelectedQuestions));

            Addressables.Release(handle);
        };

    }
}

/// <summary>
/// Struct used for holding animal prefabs with names as identification
/// </summary>
[Serializable]
public struct AnimalNameAndPrefab
{
    public string AnimalName;
    public GameObject AnimalPrefab;
}

/// <summary>
/// Struct used for holding animal image with names as identification
/// </summary>
[Serializable]
public struct AnimalNameAndImage
{
    public string AnimalName;
    public AssetReference AnimalImage;
}

/// <summary>
/// Struct used for holding animal map with names as identification
/// </summary>
[Serializable]
public struct AnimalNameAndMap
{
    public string AnimalName;
    public AssetReference AnimalMap;
}