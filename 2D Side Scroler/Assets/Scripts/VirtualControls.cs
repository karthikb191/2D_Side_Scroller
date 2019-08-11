using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualControls : MonoBehaviour {
    public static VirtualControls Instance;

    private bool shortSwipeUp, longSwipeUp, tap, swipeDown;

    private Vector2 startPos;
    private Vector2 swipeLength;

    public bool LongSwipeUp
    {
        get
        {
            return longSwipeUp;
        }
    }
    public bool ShortSwipeUp
    {
        get
        {
            return shortSwipeUp;
        }
    }
    public bool SwipeDown
    {
        get
        {
            return swipeDown;
        }
    }
    public bool Tap
    {
        get
        {
            return tap;
        }
    }

    private void Awake()
    {
        Instance = this;
    }
	
	// Update is called once per frame
	void Update () {
        tap = false; swipeDown = false; longSwipeUp = false; shortSwipeUp = false;

        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            startPos = Input.mousePosition;
        }
        

        //Drag length must be cheked when the mouse is held down
        if(startPos != Vector2.zero)
            if(Input.GetMouseButton(0))
                swipeLength = (Vector2)(Input.mousePosition) - startPos;

        if (Input.GetMouseButtonUp(0))
        {
            if (swipeLength.magnitude > 10)
            {
                if (Mathf.Abs(swipeLength.y) > Mathf.Abs(swipeLength.x))
                {
                    if (swipeLength.y < 0)
                        swipeDown = true;
                    if (swipeLength.y > 0)
                        shortSwipeUp = true;
                }
                
            }

            //if magnitude of swipe is greater than 150 pixels
            if (swipeLength.magnitude > 250)
            {
                shortSwipeUp = false;
                if (Mathf.Abs(swipeLength.y) > Mathf.Abs(swipeLength.x))
                {
                    if (swipeLength.y < 0)
                        swipeDown = true;
                    if (swipeLength.y > 0)
                        longSwipeUp = true;
                }
            }
            startPos = Vector3.zero;
            swipeLength = Vector2.zero;
        }
        

        if (tap)
            Debug.Log("tap");
        if (longSwipeUp)
            Debug.Log("Long Swipe up");
        if (swipeDown)
            Debug.Log("Swipe down");

	}
}
