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

    public virtual void TogglePanel(bool shouldDisplay, bool shouldBypassAnimation = false)
    {
        if (_shouldAnimate)
        {
            if (_openCloseCoroutine != null)
                StopCoroutine(_openCloseCoroutine);

            _openCloseCoroutine = StartCoroutine(AnimateOpenClosePanel(shouldDisplay, shouldBypassAnimation));
           
        }
        else
        {
            if (shouldDisplay)
                _mainPanel.SetActive(true);
            else
                _mainPanel.SetActive(false);
        }
    }

    private IEnumerator AnimateOpenClosePanel(bool shouldDisplay, bool shouldBypassAnimation)
    {
        Vector3 targetPosition;
        Vector3 startPosition;
        //Turn on panel before animation if we are opening it
        if (shouldDisplay)
            _mainPanel.SetActive(true);

        //Set target position to coresponding value
        if (shouldDisplay)
            targetPosition = _openedPosition;
        else
            targetPosition = _closedPosition;

        //Set start position to current position
        startPosition = _mainPanel.transform.localPosition;

        //If we are bypassing animation just set it to target
        if (shouldBypassAnimation)
        {
            _mainPanel.transform.localPosition = targetPosition;
        }
        else
        {
            //Animate
            float lerpValue = 0;
            while (true)
            {
                if (_mainPanel.transform.localPosition == targetPosition)
                    break;

                lerpValue += Time.fixedDeltaTime * _animationSpeed;
                if (lerpValue > 1)
                    lerpValue = 1;

                _mainPanel.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, lerpValue);

                yield return new WaitForFixedUpdate();
            }
        }

        //Turn off panel after animation if we are closing it
        if (!shouldDisplay)
            _mainPanel.SetActive(false);
    }
}
