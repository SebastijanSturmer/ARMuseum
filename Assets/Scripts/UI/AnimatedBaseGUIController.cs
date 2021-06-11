using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedBaseGUIController : BaseGUIController
{
    [Header("Animation Settings")]
    [SerializeField] private bool _shouldAnimate = true;
    [SerializeField] private float _animationSpeed = 1;
    [SerializeField] private Vector3 _closedPosition;
    [SerializeField] private Vector3 _openedPosition;

    private Coroutine _openCloseCoroutine;

    /// <summary>
    /// Animates opening and closing of panel
    /// </summary>
    /// <param name="shouldDisplay">Should we display that panel?</param>
    /// <param name="shouldBypassAnimation">Should we bypass animation and just set position?</param>
    /// <param name="shouldDeactivatePanelAfterAnimation">Should we set active state of panel to false if we are closing it?</param>
    /// <returns></returns>
    public virtual void TogglePanel(bool shouldDisplay, bool shouldBypassAnimation = false, bool shouldDeactivatePanelAfterAnimation = true)
    {
        if (_shouldAnimate)
        {
            if (_openCloseCoroutine != null)
                StopCoroutine(_openCloseCoroutine);

            _openCloseCoroutine = StartCoroutine(AnimateOpenClosePanel(shouldDisplay, shouldBypassAnimation, shouldDeactivatePanelAfterAnimation));
           
        }
        else
        {
            if (shouldDisplay)
                _mainPanel.SetActive(true);
            else
                _mainPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Animates opening and closing of panel
    /// </summary>
    /// <param name="shouldDisplay">Should we display that panel?</param>
    /// <param name="shouldBypassAnimation">Should we bypass animation and just set position?</param>
    /// <param name="shouldDeactivatePanelAfterAnimation">Should we set active state of panel to false if we are closing it?</param>
    /// <returns></returns>
    private IEnumerator AnimateOpenClosePanel(bool shouldDisplay, bool shouldBypassAnimation, bool shouldDeactivatePanelAfterAnimation)
    {
        Vector2 targetPosition;
        Vector2 startPosition;
        RectTransform panelTransform = _mainPanel.GetComponent<RectTransform>();

        //Turn on panel before animation if we are opening it
        if (shouldDisplay)
            _mainPanel.SetActive(true);

        //Set target position to coresponding value
        if (shouldDisplay)
            targetPosition = _openedPosition;
        else
            targetPosition = _closedPosition;

        //Set start position to current position
        startPosition = panelTransform.anchoredPosition;

        //If we are bypassing animation just set it to target
        if (shouldBypassAnimation)
        {
            panelTransform.anchoredPosition = targetPosition;
        }
        else
        {
            //Animate
            float lerpValue = 0;
            while (true)
            {
                if (panelTransform.anchoredPosition == targetPosition)
                    break;

                lerpValue += Time.fixedDeltaTime * _animationSpeed;
                if (lerpValue > 1)
                    lerpValue = 1;

                panelTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, lerpValue);

                yield return new WaitForFixedUpdate();
            }
        }

        //Turn off panel after animation if we are closing it
        if (!shouldDisplay && shouldDeactivatePanelAfterAnimation)
            _mainPanel.SetActive(false);
    }
}
