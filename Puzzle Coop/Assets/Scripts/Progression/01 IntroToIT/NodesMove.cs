using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodesMove : MonoBehaviour
{
    [SerializeField] private NodesSlot correctSlot = null;
    [SerializeField] private Camera mainCamera = null;

    public bool isMoving = false;
    public bool isEnabled = false;

    private float startPosX;
    private float startPosY;

    private Vector3 resetPosition;
    private List<Collider2D> colliderList = new List<Collider2D>();

    private NodesSlot currentSlot = null;

    public bool isInCorrectSlot => currentSlot == correctSlot;

    private void Start()
    {
        resetPosition = this.transform.localPosition;
    }


    private void Update()
    {
        if (isMoving)
        {
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = mainCamera.ScreenToWorldPoint(mousePos);

            this.transform.localPosition = new Vector3(
                mousePos.x - startPosX,
                mousePos.y - startPosY,
                -5f);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!colliderList.Contains(collision) && collision.GetComponent<NodesSlot>())
            colliderList.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (colliderList.Contains(collision))
            colliderList.Remove(collision);
    }

    private void OnMouseDown()
    {
        if (!Input.GetMouseButtonDown(0) || !isEnabled)
            return;

        Vector3 mousePos;
        mousePos = Input.mousePosition;
        mousePos = mainCamera.ScreenToWorldPoint(mousePos);

        startPosX = mousePos.x - this.transform.localPosition.x;
        startPosY = mousePos.y - this.transform.localPosition.y;

        isMoving = true;

        if (currentSlot)
        {
            currentSlot.isOccupied = false;
            currentSlot = null;
        }
                
        
    }

    private void OnMouseUp()
    {
        if (!isEnabled)
            return;

        isMoving = false;

        if (colliderList.Count <= 0)
        {
            transform.position = resetPosition;
            return;
        }

        foreach (Collider2D collider in colliderList)
        {
            NodesSlot slot = collider.GetComponent<NodesSlot>();

            if (slot.isOccupied)
            {
                transform.position = resetPosition;
                continue;
            }

            currentSlot = slot;
            currentSlot.isOccupied = true;
            transform.position = new Vector3(
                collider.transform.position.x,
                collider.transform.position.y,
                -5f);

            FindObjectOfType<MapObjectManager_L>()?.M01_OnNodeInserted();
            break;
        }

    }

}
