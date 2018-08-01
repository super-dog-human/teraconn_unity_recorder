using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchDetector : MonoBehaviour {
    Animator animator;
    GameObject nextButton;
    GameObject prevButton;
    Vector2 nextButtonBottomLeftEnd;
    Vector2 nextButtonTopRightEnd;
    Vector2 prevButtonBottomLeftEnd;
    Vector2 prevButtonTopRightEnd;
    bool isButtonTouched;
    bool isReady;

    void Start () {
        animator = gameObject.GetComponent<Animator>();
    }

    public void initButtons () {
        nextButton = GameObject.Find("NextGraphicButton");
        prevButton = GameObject.Find("PrevGraphicButton");

        nextButtonBottomLeftEnd = nextButton.GetComponent<Button>().transform.position;
        prevButtonBottomLeftEnd = prevButton.GetComponent<Button>().transform.position;

        RectTransform nextButtonTransform = nextButton.GetComponent<RectTransform>();
        RectTransform prevButtonTransform = prevButton.GetComponent<RectTransform>();
        nextButtonTopRightEnd = nextButtonTransform.TransformPoint(nextButtonTransform.sizeDelta);
        prevButtonTopRightEnd = prevButtonTransform.TransformPoint(prevButtonTransform.sizeDelta);

        isReady = true;
    }

    void OnAnimatorIK (int layerIndex) {
        if (isButtonTouched) return;
        if (!isReady) return;

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

    bool IsTouchNextGraphButton (Vector2 rightHandPosition, Vector2 leftHandPosition) {
        if (IsPositionInRange(rightHandPosition, true)) return true;
        if (IsPositionInRange(leftHandPosition, true)) return true;

        return false;
    }

    bool IsTouchPrevGraphButton (Vector2 rightHandPosition, Vector2 leftHandPosition) {
        if (IsPositionInRange(rightHandPosition, false)) return true;
        if (IsPositionInRange(leftHandPosition, false)) return true;

        return false;
    }

    bool IsPositionInRange (Vector2 targetPosition, bool isNext) {
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

    void clickButton (GameObject target) {
        EventSystem.current.SetSelectedGameObject(target);
        ExecuteEvents.Execute(
            target:    target,
            eventData: new PointerEventData(EventSystem.current),
            functor:   ExecuteEvents.pointerClickHandler
        );
    }

    void EnableTouch () {
        EventSystem.current.SetSelectedGameObject(null);
        isButtonTouched = false;
    }
}