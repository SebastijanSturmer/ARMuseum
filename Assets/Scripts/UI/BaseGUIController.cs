using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGUIController : MonoBehaviour
{
    [SerializeField] protected GameObject _mainPanel;

    /// <summary>
    /// Sets main panel active state to true or false
    /// </summary>
    /// <param name="shouldDisplay"></param>
    public virtual void TogglePanel(bool shouldDisplay)
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
