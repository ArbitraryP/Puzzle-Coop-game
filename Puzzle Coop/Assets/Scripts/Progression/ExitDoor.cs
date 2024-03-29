using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour, IClickable
{
    private MapObjectManager_L localObjectManager = null;

    public int playerNumber = 0; //used to determine whose ready or not. Not Used rn
    public bool isUnlocked = false;
    
    
    public void Click()
    {
        
        if (!isUnlocked)
        {
            FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_MAP_DoorLockInteract);
            Debug.Log("Door is Locked.");
            return;
        }

        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_MAP_DoorOpened);
        localObjectManager = FindObjectOfType<MapObjectManager_L>();
        if (!localObjectManager)
        {
            Debug.Log("Local Map Object Manager Missing!");
            return;
        }


        FindObjectOfType<MapCompleteMessage>()?.ShowMessage(true);
        gameObject.SetActive(false);
        Debug.Log("Level Completed for this player");

        // show screen Waiting For Other Player

    }

}
