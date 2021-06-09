using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _spawnSpeed;
    [SerializeField] private GameObject _prefabToSpawn;

    [Header("References")]
    [SerializeField] private ARTrackedImageManager _trackedImageManager;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _arAnimalDetected;

    private GameObject _spawnedObject;

    

    private void OnEnable()
    {
        _trackedImageManager.trackedImagesChanged += OnImageRecognized;
    }
    private void OnDisable()
    {
        _trackedImageManager.trackedImagesChanged -= OnImageRecognized;
    }


    public void OnImageRecognized(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage trackedImage in args.added)
        {
            StartCoroutine(SpawnObjectOnImage(trackedImage));
        }
        foreach (ARTrackedImage trackedImage in args.updated)
        {
            UpdateObjectPosition(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in args.removed)
        {
            RemoveObjectFromImage(trackedImage);
        }

    }


    IEnumerator SpawnObjectOnImage(ARTrackedImage trackedImage)
    {
        _spawnedObject = Instantiate(_prefabToSpawn, trackedImage.transform.position, trackedImage.transform.rotation, this.gameObject.transform);
        _spawnedObject.transform.localScale = Vector3.zero;
        _spawnedObject.name = trackedImage.name;

        float lerpValue = 0;
        while (true)
        {
            lerpValue += Time.deltaTime * _spawnSpeed;

            if (_spawnedObject.transform.localScale.x >= 1)
                break;

            _spawnedObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, lerpValue);

            yield return new WaitForEndOfFrame();
        }

        _arAnimalDetected.RaiseEvent(new AnimalDataMessage(new AnimalData())); //TODO
    }

    private void UpdateObjectPosition(ARTrackedImage trackedImage)
    {
        _spawnedObject.transform.position = trackedImage.transform.position;
        _spawnedObject.transform.rotation = trackedImage.transform.rotation;
    }

    void RemoveObjectFromImage(ARTrackedImage trackedImage)
    {
        Destroy(_spawnedObject);
    }
}
