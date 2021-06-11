using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARMainMenuPanelController : BaseGUIController
{
    [Header("Events")]
    [SerializeField] private ScriptableEvent _startAR;
    [SerializeField] private ScriptableEvent _loadMainMenu;


    public void OnStartARButtonPressed()
    {
        _startAR.RaiseEvent();
    }


    public void OnExitARButtonPressed()
    {
        _loadMainMenu.RaiseEvent();
    }
}
