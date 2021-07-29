using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private EventSystem eventSystem;

    private PointerEventData pointerData = new PointerEventData(null);







    public bool CheckObjectIsBlocked()
    {
        pointerData.position = Input.mousePosition;
        /*
        List<RaycastResult> results = new List<RaycastResult>();

        graphicRaycaster.Raycast(pointerData, results);

        return (results.Count > 0);
        */


        // Gets All Graphic Raycasters from all Canvasses
        List<GraphicRaycaster> graphicRaycasters =
            new List<GraphicRaycaster>(FindObjectsOfType<GraphicRaycaster>());

        // Loops Through it if any has count then return true, otherwise false after loop
        foreach (GraphicRaycaster gr in graphicRaycasters)
        {
            List<RaycastResult> results = new List<RaycastResult>();

            gr.Raycast(pointerData, results);

            if (results.Count > 0)
                return true;
        }
        return false;

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CheckObjectIsBlocked())
                return;


            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit)
            {
                hit.collider.GetComponent<IClickable>()?.Click();
            }
        }
    }



}
