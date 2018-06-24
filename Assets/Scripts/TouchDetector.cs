using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchDetector : MonoBehaviour {

    private Animator animator;
    private GameObject nextButton;
    private GameObject prevButton;
    private Vector2 nextButtonBottomLeftEnd;
    private Vector2 nextButtonTopRightEnd;
    private Vector2 prevButtonBottomLeftEnd;
    private Vector2 prevButtonTopRightEnd;
    private bool isButtonTouched;

    void Start ()
    {
        animator = gameObject.GetComponent<Animator>();

        nextButton = GameObject.Find("NextImageButton");
        prevButton = GameObject.Find("PrevImageButton");

        nextButtonBottomLeftEnd = nextButton.GetComponent<Button>().transform.position;
        prevButtonBottomLeftEnd = prevButton.GetComponent<Button>().transform.position;

        RectTransform nextButtonTransform = nextButton.GetComponent<RectTransform>();
        RectTransform prevButtonTransform = prevButton.GetComponent<RectTransform>();
        nextButtonTopRightEnd = nextButtonTransform.TransformPoint(nextButtonTransform.sizeDelta);
        prevButtonTopRightEnd = prevButtonTransform.TransformPoint(prevButtonTransform.sizeDelta);
    }

    void OnAnimatorIK (int layerIndex)
    {
        if (isButtonTouched) return;

        Vector2 rightHandPosition = Camera.main.WorldToScreenPoint(animator.GetIKPosition(AvatarIKGoal.RightHand));
        Vector2 leftHandPosition  = Camera.main.WorldToScreenPoint(animator.GetIKPosition(AvatarIKGoal.LeftHand));

        if (IsTouchNextGraphButton(rightHandPosition, leftHandPosition)) {
            isButtonTouched = true;
            clickButton(nextButton);
        } else if (IsTouchPrevGraphButton(rightHandPosition, leftHandPosition)) {
            isButtonTouched = true;
            clickButton(prevButton);
        }

        if (isButtonTouched) Invoke("EnableTouch", 1);
    }

    private bool IsTouchNextGraphButton (Vector2 rightHandPosition, Vector2 leftHandPosition)
    {
        if (IsPositionInRange(rightHandPosition, true)) return true;
        if (IsPositionInRange(leftHandPosition, true)) return true;

        return false;
    }

    private bool IsTouchPrevGraphButton (Vector2 rightHandPosition, Vector2 leftHandPosition)
    {
        if (IsPositionInRange(rightHandPosition, false)) return true;
        if (IsPositionInRange(leftHandPosition, false)) return true;

        return false;
    }

    private bool IsPositionInRange (Vector2 targetPosition, bool isNext)
    {
        Vector2 leftEndPosition;
        Vector2 rightEndPosition;
        if (isNext) {
            leftEndPosition  = nextButtonBottomLeftEnd;
            rightEndPosition = nextButtonTopRightEnd;
        } else {
            leftEndPosition  = prevButtonBottomLeftEnd;
            rightEndPosition = prevButtonTopRightEnd;
        }

        if (targetPosition.x >= leftEndPosition.x  && targetPosition.y >= leftEndPosition.y &&
            targetPosition.x <= rightEndPosition.x && targetPosition.y <= rightEndPosition.y) {

            return true;
        }

        return false;
    }

    private void clickButton (GameObject target)
    {
        EventSystem.current.SetSelectedGameObject(target);
        ExecuteEvents.Execute(
            target:    target,
            eventData: new PointerEventData(EventSystem.current),
            functor:   ExecuteEvents.pointerClickHandler
        );
    }

    private void EnableTouch ()
    {
        EventSystem.current.SetSelectedGameObject(null);
        isButtonTouched = false;
    }
}