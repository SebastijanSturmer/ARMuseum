using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGUIController : MonoBehaviour
{
    [SerializeField] protected GameObject _mainPanel;

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
