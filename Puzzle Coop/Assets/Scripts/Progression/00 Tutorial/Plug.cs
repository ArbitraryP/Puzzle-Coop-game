using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plug : MonoBehaviour
{
    [SerializeField] private GameObject outletObject = null;
    [SerializeField] private Transform outletSlot = null;
    [SerializeField] private Sprite newPlugSprite = null;
    [SerializeField] private bool isHoldingPlug = false;
    [SerializeField] private bool isInsideOutlet = false;

    public bool IsHoldingPlug
    {
        set
        {
            TryInsertPlug();
            isHoldingPlug = value;
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonUp(0))
            TryInsertPlug();
    }


    public void TryInsertPlug()
    {
        if (isInsideOutlet && isHoldingPlug)
            PlugInserted();
    }


    private void PlugInserted()
    {
        /* Remove Draggable Tag
         * Change Sprite
         * Set to Static Rigidbody
         * Set Position
         * Set Rotation
         * Tell Server that Plug is Inserted
        */

        gameObject.layer = 0;
        GetComponent<SpriteRenderer>().sprite = newPlugSprite;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        transform.position = outletSlot.position;
        transform.rotation = Quaternion.identity;

        MapObjectManager_L localObjectManager = FindObjectOfType<MapObjectManager_L>();
        if (!localObjectManager)
        {
            Debug.Log("Local Map Object Manager Missing!");
            return;
        }
        localObjectManager.serverObjectManager.CmdM00_PowerOn();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == outletObject)
        {
            isInsideOutlet = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == outletObject)
        {
            isInsideOutlet = false;
        }
    }



}
