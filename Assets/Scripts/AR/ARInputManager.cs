using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARInputManager : MonoBehaviour
{

    [SerializeField] private LayerMask _layerMaskForInput;

    [Header("Mouse Events")]
    [SerializeField] ScriptableEvent _animalClicked;

    private Ray ray;
    private RaycastHit rayHit;
    void Update()
    {
        CheckForInput();
    }

    private void CheckForInput()
    {
        if (Camera.main == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            //Handling click on lord or town!
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, _layerMaskForInput, QueryTriggerInteraction.Collide))
            {
                ARAnimalController animalController = rayHit.collider.GetComponent<ARAnimalController>(); //Get animal controller

                if (animalController != null) //If we have animal controller call event for ARManager
                { 
                    _animalClicked.RaiseEvent(new AnimalDataMessage(animalController.AnimalData));
                    return;
                }

            }
        }
    }
}
