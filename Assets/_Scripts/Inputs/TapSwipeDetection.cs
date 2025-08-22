using System;

using UnityEngine;

public class TapSwipeDetection : MonoBehaviour
{
    //variables to control tapping and swiping
    float distThreshold = 0.09f;
    float dirThreshold = 0.7f;

    float tapTimeout = 0.2f;
    float swipeTimeout = 1f;
    float startTime = 0;
    float endTime = 0;

    public event Action OnTap;
    public event Action OnSwipeLeft;
    public event Action OnSwipeRight;
    public event Action OnSwipeUp;
    public event Action OnSwipeDown;

    Vector2 startPos;
    Vector2 endPos;
    InputManager im;

    private void Start()
    { 
        im = GetComponent<InputManager>();
        im.OnTouchBegin += TouchStarted;
        im.OnTouchEnd += TouchEnded;
    }

    private void TouchStarted()
    {
        startPos = im.PrimaryPosition();
        startTime = Time.time;
        //Debug.Log($"Touch Started at {startPos}");
    }

    private void TouchEnded()
    {
        endTime = Time.time;
        endPos = im.PrimaryPosition();
       // Debug.Log($"Touch Ended at {endPos}");
        DetectInput();
    }

    private void DetectInput()
    {
        float totalTime = endTime - startTime;
        if(totalTime > swipeTimeout)
        {
            //Debug.Log("Not tap or swipe");
            return;
        }
        if(totalTime < tapTimeout)
        {
            Tap();
            return;
        }
        CheckSwipe();
    }

    private void CheckSwipe()
    {
        float distance = Vector2.Distance(startPos, endPos);

        if (distance < distThreshold) return;

        Vector2 dir = (endPos - startPos).normalized;

        float checkUp = Vector2.Dot(Vector2.up, dir);
        float checkLeft = Vector2.Dot(Vector2.left, dir);

        if(checkUp >= dirThreshold)
        {
            Debug.Log("Swipe Up");
            OnSwipeUp?.Invoke();
            return;
        }
        if (checkUp <= -dirThreshold)
        {
            //Debug.Log("Swipe Down");
            OnSwipeDown?.Invoke();
            return;
        }
        if (checkLeft >= dirThreshold)
        {
            //Debug.Log("Swipe Left");
            OnSwipeLeft?.Invoke();
            return;
        }
        if (checkLeft <= -dirThreshold)
        {
            //Debug.Log("Swipe Right");
            OnSwipeRight?.Invoke();
            return;
        }
    }

    private void Tap()
    {
        //Debug.Log($"Tap at {InputManager.Instance.PrimaryPosition()}");
        OnTap?.Invoke();
    }
}
