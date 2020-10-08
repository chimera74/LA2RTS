using LA2RTS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SelectionRect : MonoBehaviour {

    // Draggable inspector reference to the Image GameObject's RectTransform.
    public RectTransform selectionBox;
    public int delta;
    public bool deselectOnZero;
    
    public Camera mainCamera;

    // This variable will store the location of wherever we first click before dragging.
    private Vector2 startPosition = Vector2.zero;
    private Image image;

    private bool started = false;
    private bool isVisible = false;

    private L2RTSServerManager SM;

    // Use this for initialization
    void Start() {
        image = GetComponent<Image>();
        SM = FindObjectOfType<L2RTSServerManager>();
    }

    void Update()
    {
        ProcessSelectRect();
        image.enabled = isVisible;
    }

    private void ProcessSelectRect()
    {
        // Click somewhere in the Game View.
        if (Input.GetMouseButtonDown(0))
        {
            // Get the initial click position of the mouse. No need to convert to GUI space
            // since we are using the lower left as anchor and pivot.
            startPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            // If click position was on UI element, then do not start drawing rectangle.
            if (CheckIfUIClick(startPosition))
                return;
            else
                started = true;

            // The anchor is set to the same place.
            selectionBox.anchoredPosition = startPosition;
        }

        // While we are dragging.
        if (Input.GetMouseButton(0))
        {
            if (started)
            {
                // Store the current mouse position in screen space.
                Vector2 currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                // How far have we moved the mouse?
                Vector2 difference = currentMousePosition - startPosition;

                // If the mouse moved more than delta value, set rectangle visible and process it's position
                if (difference.magnitude > delta)
                    isVisible = true;
                else
                {
                    isVisible = false;
                    return;
                }

                // Copy the initial click position to a new variable. Using the original variable will cause
                // the anchor to move around to wherever the current mouse position is,
                // which isn't desirable.
                Vector2 startPoint = startPosition;

                // The following code accounts for dragging in various directions.
                if (difference.x < 0)
                {
                    startPoint.x = currentMousePosition.x;
                    difference.x = -difference.x;
                }
                if (difference.y < 0)
                {
                    startPoint.y = currentMousePosition.y;
                    difference.y = -difference.y;
                }

                // Set the anchor, width and height every frame.
                selectionBox.anchoredPosition = startPoint;
                selectionBox.sizeDelta = difference;
            }
        }

        // After we release the mouse button.
        if (Input.GetMouseButtonUp(0))
        {   
            if (isVisible)
            {
                OnSelectionEnd();
            }

            started = false;
            isVisible = false;
        }
    }

    // returns true if it was a click on UI
    private bool CheckIfUIClick(Vector2 point)
    {
        bool res = false;

        // Iterate through all visible objects that accept raycasts and check if click position is insade any of them
        foreach (Graphic g in transform.parent.transform.GetComponentsInChildren<Graphic>())
        {   
            if (g.enabled && g.raycastTarget && IsInsideSelectRect(point, g.rectTransform))
            {   
                res = true;
                break;
            }
        }
        return res;
    }

    private void OnSelectionEnd()
    {
        LinkedList<RTSClient> selectedList = new LinkedList<RTSClient>();

        foreach (var kv in SM.userActorManager.clientProperties)
        {
            var actor = kv.Value.actor;
            if (actor == null)
                continue;

            Vector3 screenPos = mainCamera.WorldToScreenPoint(actor.transform.position);
            if (IsInsideSelectRect(screenPos, selectionBox))
            {
                selectedList.AddLast(kv.Key);
            }
        }

        if (deselectOnZero || selectedList.Count > 0)
        {
            SM.selectionManager.SelectMultipleUsers(selectedList);
        }
            
    }

    private bool IsInsideSelectRect(Vector3 screenPos, RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Rect rect = new Rect(corners[0], corners[2] - corners[0]);

        return rect.Contains(screenPos);

        //var diff1 = screenPosV2 - selectionBox.anchoredPosition;
        //var diff2 = selectionBox.anchoredPosition + selectionBox.sizeDelta - screenPosV2;
        //return diff1.x > 0 && diff1.y > 0 && diff2.x > 0 && diff2.y > 0;
    }
}
