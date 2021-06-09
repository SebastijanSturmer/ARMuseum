using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGUIController : MonoBehaviour
{
    [SerializeField] private GameObject _mainPanel;

    public void TogglePanel(bool shouldDisplay)
    {
        if (shouldDisplay)
        {
            _mainPanel.SetActive(true);
        }
        else
        {
            _mainPanel.SetActive(false);
        }
    }
}
