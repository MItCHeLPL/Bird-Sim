using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoScroll : MonoBehaviour
{
    private ScrollRect scrollRect;
    private GameObject lastSelection;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();

        lastSelection = EventSystem.current.currentSelectedGameObject; //save last selection to detect change
    }

	private void Update()
	{
        if(lastSelection != EventSystem.current.currentSelectedGameObject) //If changed selection
		{
            OnSelectionChange(); //changed selection
            lastSelection = EventSystem.current.currentSelectedGameObject; //save new selection
        }
    }

    public void OnSelectionChange()
    {
        Scroll();
    }

private void Scroll()
    {
        if(EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform)) //If selection is child of scrollRect
		{
            RectTransform selectionRect = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>(); //get selected rect

            //Check if selected rect is in view
            //Get all corners of scrollRect
            Vector3[] scrollRectCorners = new Vector3[4];
            scrollRect.viewport.GetWorldCorners(scrollRectCorners);

            //Get all corners of selection
            Vector3[] selectionCorners = new Vector3[4];
            selectionRect.GetWorldCorners(selectionCorners);

            /* corner layout
            03
            12
            */

            //if selection is contained in scroll rect
            bool selectionIsInView = (
                scrollRectCorners[0].y <= selectionCorners[0].y && 
                scrollRectCorners[1].y >= selectionCorners[1].y &&
                scrollRectCorners[1].x <= selectionCorners[1].x &&
                scrollRectCorners[2].x >= selectionCorners[2].x
            );


            //Scroll to selection
            //if selected element isn't fully in view
            if (!selectionIsInView)
            {
                Canvas.ForceUpdateCanvases(); //Refresh UI

                Vector2 scrollRectPos = (Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position); //Get scrollRect position
                Vector2 selectionRectPos = (Vector2)scrollRect.transform.InverseTransformPoint(selectionRect.position); //Get selection position

                //Default values
                float xPos = scrollRect.content.anchoredPosition.x;
                float yPos = scrollRect.content.anchoredPosition.y;

                //if horizontal scrolling is enabled
                if (scrollRect.horizontal)
				{
                    xPos = (scrollRectPos.x - selectionRectPos.x) - selectionRect.sizeDelta.x; //calculate x new position
                }

                //if vertical scrolling is enabled
                if (scrollRect.vertical)
				{
                    yPos = (scrollRectPos.y - selectionRectPos.y) - selectionRect.sizeDelta.y; //calculate y new position
                }

                scrollRect.content.anchoredPosition = new Vector2(xPos, yPos); //apply new position
            }
        }
    }
}
