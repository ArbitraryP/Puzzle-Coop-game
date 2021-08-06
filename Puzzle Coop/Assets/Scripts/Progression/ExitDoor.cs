using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour, IClickable
{
    private MapObjectManager_L localObjectManager = null;

    public int playerNumber = 0;
    public bool isUnlocked = false;
    
    
    public void Click()
    {
        
        if (!isUnlocked)
        {
            Debug.Log("Door is Locked.");
            return;
        }

        localObjectManager = FindObjectOfType<MapObjectManager_L>();
        if (!localObjectManager)
        {
            Debug.Log("Local Map Object Manager Missing!");
            return;
        }

        localObjectManager.serverObjectManager.CmdExitDoor(playerNumber);
        gameObject.SetActive(false);
        Debug.Log("Level Completed for this player");

        // show screen Waiting For Other Player

    }

}
