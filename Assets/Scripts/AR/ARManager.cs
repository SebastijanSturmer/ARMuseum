using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


public class ARManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _spawnSpeed;

    [Header("Prefabs")]
    [SerializeField] private GameObject _trackedImageManagerPrefab;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _arStarted;
    [SerializeField] private ScriptableEvent _arAnimalFocused;

    private List<AnimalData> _animals;

    private ARTrackedImageManager _trackedImageManager;
    private ARSession _arSession;
    private List<AnimalTrackedImageAndGameObject> _spawnedAnimals = new List<AnimalTrackedImageAndGameObject>();

    private bool _loadingAnimalsCompleted;



    /// <summary>
    /// Event function that will start AR
    /// </summary>
    public void OnStartAR()
    {
        StartCoroutine(StartARCoroutine());

    }

    /// <summary>
    /// Event function that will stop AR and reset AR Session
    /// </summary>
    public void OnExitAR()
    {
        for (int i = 0; i < _spawnedAnimals.Count; i++)
        {
            if (_spawnedAnimals[i].GameObject != null)
                Destroy(_spawnedAnimals[i].GameObject);
        }

        _spawnedAnimals.Clear();
        _arSession.Reset();

        _trackedImageManager.trackedImagesChanged -= OnImageRecognized;

        Destroy(_trackedImageManager.gameObject);
    }

    /// <summary>
    /// Event function that is called by InputManager when user clicks on 3D animal object
    /// </summary>
    /// <param name="animalDataMessage"></param>
    public void OnAnimalClicked(EventMessage animalDataMessage)
    {
        AnimalData animalData = ((AnimalDataMessage)animalDataMessage).AnimalData;

        if (animalData == null)
            return;

        _arAnimalFocused.RaiseEvent(new AnimalDataMessage(animalData));
    }

    /// <summary>
    /// Event function that receives list of animal data from data manager when they are loaded from json file
    /// </summary>
    /// <param name="listOfAnimalDataMessage"></param>
    public void OnAnimalListReceived(EventMessage listOfAnimalDataMessage)
    {
        _animals = ((ListOfAnimalDataMessage)listOfAnimalDataMessage).AnimalDataList;
        _loadingAnimalsCompleted = true;
    }

    /// <summary>
    /// Event function that is called when TrackedImageManager updates! Used for spawning,positioning and destroying objects
    /// </summary>
    /// <param name="args"></param>
    public void OnImageRecognized(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage trackedImage in args.added)
        {
            StartCoroutine(SpawnObjectOnImage(trackedImage));
        }
        foreach (ARTrackedImage trackedImage in args.updated)
        {
            UpdateObject(trackedImage, args.updated.Count);
        }
        foreach (ARTrackedImage trackedImage in args.removed)
        {
            RemoveObjectFromImage(trackedImage);
        }

    }

    /// <summary>
    /// Coroutine that requests animals from json from DataManager, loads them and starts AR
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartARCoroutine()
    {
        _loadingAnimalsCompleted = false;

        DataManager.Instance.RequestAnimalsFromJSON();

        yield return new WaitUntil(() => _loadingAnimalsCompleted == true);

        _trackedImageManager = Instantiate(_trackedImageManagerPrefab,this.transform).GetComponent<ARTrackedImageManager>();
        _arSession = _trackedImageManager.GetComponentInChildren<ARSession>();

        _trackedImageManager.trackedImagesChanged += OnImageRecognized;

        _arStarted.RaiseEvent();
        
    }

    /// <summary>
    /// Coroutine that will spawn object depending on tracked image and it will animate its spawn (growing)
    /// </summary>
    /// <param name="trackedImage"></param>
    /// <returns></returns>
    private IEnumerator SpawnObjectOnImage(ARTrackedImage trackedImage)
    {
        GameObject prefabToSpawn = DataManager.Instance.GetAnimalPrefabByIdentifier(trackedImage.referenceImage.name);

        //If we havent found animal in data manager then break
        if (prefabToSpawn == null)
            yield break;

        //Spawning object
        GameObject spawnedObject = Instantiate(prefabToSpawn, trackedImage.transform.position, trackedImage.transform.rotation, this.gameObject.transform);
        spawnedObject.transform.localScale = Vector3.zero;
        spawnedObject.name = trackedImage.referenceImage.name;

        //Adding AnimalController and adding animal data to that controller
        spawnedObject.GetComponent<ARAnimalController>().SetAnimalData(FindAnimalDataByIdentifier(trackedImage.referenceImage.name));


        _spawnedAnimals.Add(new AnimalTrackedImageAndGameObject(trackedImage, spawnedObject));
        //Animating spawn
        float lerpValue = 0;
        while (true)
        {
            lerpValue += Time.fixedDeltaTime * _spawnSpeed;
            if (lerpValue > 1)
                lerpValue = 1;

            if (spawnedObject.transform.localScale.x >= 1)
                break;

            spawnedObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, lerpValue);

            yield return new WaitForFixedUpdate();
        }

        FindAnimalDataAndCallAnimalFocusedEvent(trackedImage.referenceImage.name);
    }

    /// <summary>
    /// Function that updates object position and rotation aswell as call AnimalDetectedEvent if we started to track new animal.
    /// </summary>
    /// <param name="trackedImage"></param>
    private void UpdateObject(ARTrackedImage trackedImage, int howManyObjectsWillGetUpdated)
    {
        
        for (int i = 0; i < _spawnedAnimals.Count; i++)
        {
            if (_spawnedAnimals[i].Image == trackedImage)
            {
                _spawnedAnimals[i].GameObject.transform.position = trackedImage.transform.position;
                _spawnedAnimals[i].GameObject.transform.rotation = trackedImage.transform.rotation;
            }
        }
    }

    /// <summary>
    /// Function that is called when tracked image was removed. It will destroy gameobject that was on that image and it will remove it from spawnedAnimals list
    /// </summary>
    /// <param name="trackedImage"></param>
    void RemoveObjectFromImage(ARTrackedImage trackedImage)
    {
        List<AnimalTrackedImageAndGameObject> spawnedAnimalsListCopy = new List<AnimalTrackedImageAndGameObject>();

        for (int i = 0; i < _spawnedAnimals.Count; i++)
        {
            spawnedAnimalsListCopy.Add(_spawnedAnimals[i]);
        }

        for (int i = 0; i < spawnedAnimalsListCopy.Count; i++)
        {
            if (spawnedAnimalsListCopy[i].Image == trackedImage)
            {
                Destroy(spawnedAnimalsListCopy[i].GameObject);
                _spawnedAnimals.Remove(spawnedAnimalsListCopy[i]);
            }

        }
    }
    
    /// <summary>
    /// Function that finds animal in data by name and calls animal focused event
    /// </summary>
    /// <param name="name">Animal name</param>
    private void FindAnimalDataAndCallAnimalFocusedEvent(string name)
    {
        AnimalData animalData = FindAnimalDataByIdentifier(name);

        if (animalData != null)
        {
            //Raising event about detected animal
            //Used for GUI info about that animal.
            _arAnimalFocused.RaiseEvent(new AnimalDataMessage(animalData));
        }
    }

    private AnimalData FindAnimalDataByIdentifier(string identifier)
    {
        //Find animal in data
        for (int i = 0; i < _animals.Count; i++)
        {
            if (_animals[i].AnimalIdentifier == identifier)
            {
                return(_animals[i]);
            }
        }
        return null;
    }
}

public struct AnimalTrackedImageAndGameObject
{
    public ARTrackedImage Image;
    public GameObject GameObject;

    public AnimalTrackedImageAndGameObject(ARTrackedImage image, GameObject gameObject)
    {
        Image = image;
        GameObject = gameObject;
    }
}
