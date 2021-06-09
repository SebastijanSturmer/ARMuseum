using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARUIManager : MonoBehaviour
{
    [SerializeField] private ScriptableEvent _loadMainMenu;

    public void OnReturnToMainMenuButtonPressed()
    {
        _loadMainMenu.RaiseEvent();
    }
}
