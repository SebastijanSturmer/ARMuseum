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
    [SerializeField] private List<LanguageAndAssetReference> _listOfQuestionJSONFilesByLanguage;
    [SerializeField] private List<LanguageAndAssetReference> _listOfAnimalsJSONFilesByLanguage;

    [Header("Animals additional data")]
    [SerializeField] private List<AnimalIndentifierAndAdditionalData> _animalAdditionalData;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _listOfQuestionsFromJSONCompleted;
    [SerializeField] private ScriptableEvent _listOfAnimalsFromJSONCompleted;
    [SerializeField] private ScriptableEvent _onImageAndMapOfAnimalCompleted;
    [SerializeField] private ScriptableEvent _requestSound;

    public static DataManager Instance;
    private AsyncOperationHandle<Texture2D> _lastAnimalImageHandle;
    private AsyncOperationHandle<Texture2D> _lastAnimalMapHandle;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Gets animal prefab for specified animal identifier
    /// </summary>
    /// <returns></returns>
    public GameObject GetAnimalPrefabByIdentifier(string identifier)
    {
        for (int i = 0; i < _animalAdditionalData.Count; i++)
        {
            if (_animalAdditionalData[i].AnimalIndentifier == identifier)
            {
                return _animalAdditionalData[i].AnimalPrefab;
            }
        }
        Debug.LogError("DataManager : There was no animal prefab for " + identifier);
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
    public void RequestAnimalImageAndMapByIdentifier(string identifier)
    {
        StartCoroutine(LoadAnimalImageAndMapCoroutine(identifier));
    }

    /// <summary>
    /// Loads animal image and map from addressables and then raises OnImageAndMapOfAnimalCompleted event with them
    /// </summary>
    /// <param name="identifier">Animal indentifier (it should be animal name on english)</param>
    /// <returns></returns>
    private IEnumerator LoadAnimalImageAndMapCoroutine(string identifier)
    {
        AssetReference imageRef = null;
        AssetReference mapRef = null;

        for (int i = 0; i < _animalAdditionalData.Count; i++)
        {
            if (_animalAdditionalData[i].AnimalIndentifier == identifier)
            {
                imageRef = _animalAdditionalData[i].AnimalImage;
                break;
            }
        }
        for (int i = 0; i < _animalAdditionalData.Count; i++)
        {
            if (_animalAdditionalData[i].AnimalIndentifier == identifier)
            {
                mapRef = _animalAdditionalData[i].AnimalMap;
                break;
            }
        }

        if (imageRef == null || mapRef == null)
        {
            Debug.LogError("DataManager : Image reference or map reference for " + identifier + " is null!");
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
        AsyncOperationHandle<Texture2D> newAnimalImageHandle = Addressables.LoadAssetAsync<Texture2D>(imageRef);
        yield return newAnimalImageHandle;

        image = Sprite.Create(newAnimalImageHandle.Result, new Rect(0, 0, newAnimalImageHandle.Result.width, newAnimalImageHandle.Result.height), new Vector2(0.5f, 0.5f), 100.0f);

        AsyncOperationHandle<Texture2D> newAnimalMapHandle = Addressables.LoadAssetAsync<Texture2D>(mapRef);
        yield return newAnimalMapHandle;

        map = Sprite.Create(newAnimalMapHandle.Result, new Rect(0, 0, newAnimalMapHandle.Result.width, newAnimalMapHandle.Result.height), new Vector2(0.5f, 0.5f), 100.0f);

        _onImageAndMapOfAnimalCompleted.RaiseEvent(new AnimalImageAndMapMessage(image, map));

        //Release last used addresables for image and map
        if (_lastAnimalMapHandle.IsValid())
            Addressables.Release(_lastAnimalMapHandle);
        if (_lastAnimalImageHandle.IsValid())
            Addressables.Release(_lastAnimalImageHandle);

        //Store new ones so we can release them when we scan new animal
        _lastAnimalImageHandle = newAnimalImageHandle;
        _lastAnimalMapHandle = newAnimalMapHandle;

    }

    /// <summary>
    /// Function that reads animals data from Animals Addressable asset defined in settings and raises event with that list.
    /// </summary>
    public void RequestAnimalsFromJSON()
    {
        AssetReference animalsJSON = null;

        for (int i = 0; i < _listOfAnimalsJSONFilesByLanguage.Count; i++)
        {
            if (_listOfAnimalsJSONFilesByLanguage[i].Language == LocalizationManager.Instance.Language)
            {
                animalsJSON = _listOfAnimalsJSONFilesByLanguage[i].AssetReference;
            }
        }

        if (animalsJSON == null)
        {
            Debug.LogError("DataManager : Animals for " + LocalizationManager.Instance.Language + " was not found");
            return;
        }


        List<AnimalData> animalsFromJSON = new List<AnimalData>();

        if (!animalsJSON.RuntimeKeyIsValid())
        {
            Debug.LogError("DataManager : Json animals runtime key is not valid!");
            return;
        }

        animalsJSON.LoadAssetAsync<TextAsset>().Completed += handle =>
        {

            var dictionaryOfObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(handle.Result.text);
            
            ListOfAnimalData animals = new ListOfAnimalData();
            animals.Animals = JsonConvert.DeserializeObject<List<AnimalData>>(dictionaryOfObject["Animals"].ToString()); ;

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
        AssetReference questionsJSON = null;

        for (int i = 0; i < _listOfQuestionJSONFilesByLanguage.Count; i++)
        {
            if (_listOfQuestionJSONFilesByLanguage[i].Language == LocalizationManager.Instance.Language)
            {
                questionsJSON = _listOfQuestionJSONFilesByLanguage[i].AssetReference;
            }
        }
            
        if (questionsJSON == null)
        {
            Debug.LogError("DataManager : Questions for " + LocalizationManager.Instance.Language + " was not found");
            return;
        }

        if (!questionsJSON.RuntimeKeyIsValid())
        {
            Debug.LogError("DataManager : Json Questions runtime key is not valid!");
            return;
        }

        List<QuizQuestion> randomSelectedQuestions = new List<QuizQuestion>();

        questionsJSON.LoadAssetAsync<TextAsset>().Completed += handle =>
        {

            var dictionaryOfObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(handle.Result.text);
            
            ListOfQuizQuestions questions = new ListOfQuizQuestions();
            questions.Questions = JsonConvert.DeserializeObject<List<QuizQuestion>>(dictionaryOfObject["Questions"].ToString());


            //ListOfQuizQuestions questions = JsonUtility.FromJson<ListOfQuizQuestions>(handle.Result.text);
            
            //If we requested more questions then we have in json file then dont try to load them and log error!
            if (numberOfQuestions > questions.Questions.Count)
            {
                Debug.LogError("There are not enough questions in json file " + questionsJSON.Asset.name);
                Addressables.Release(handle);
                return;
            }

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
/// Struct used for holding additional animal data with identifier
/// </summary>
[Serializable]
public struct AnimalIndentifierAndAdditionalData
{
    public string AnimalIndentifier;
    public GameObject AnimalPrefab;
    public AssetReference AnimalImage;
    public AssetReference AnimalMap;
}

/// <summary>
/// Struct used for holding asset reference by language
/// </summary>
[Serializable]
public struct LanguageAndAssetReference
{
    public Enums.Language Language;
    public AssetReference AssetReference;
}