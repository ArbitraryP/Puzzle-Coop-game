using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Passcode : MonoBehaviour
{
    private string currentString = "";
    [SerializeField] private TMP_InputField inputField = null;
    [SerializeField] private Button closeButton = null;
    [SerializeField] private string correctCode = "0831";
    [SerializeField] private MonitorScreen monitorScreen = null;

    public void OnClickInput(string character)
    {
        PlayUIKeyTone();
        if (currentString == "XXXX") currentString = "";

        if(currentString.Length >= 4)
        {
            Debug.Log("Passcode input field full. Cannot add more");
            // Play sound
        }
        else
        {
            currentString += character;
            inputField.text = currentString;
        }
    }

    public void OnClickEnter()
    {
        PlayUIKeyTone();
        if (currentString != correctCode)
        {
            currentString = "XXXX";
            inputField.text = currentString;

            // Play sound error
            FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M00_WrongCode);
        }
        else
        {
            closeButton.onClick.Invoke();

            MapObjectManager_L localObjectManager = FindObjectOfType<MapObjectManager_L>();
            if (!localObjectManager)
            {
                Debug.Log("Local Map Object Manager Missing!");
                return;
            }

            // Play sound correct code
            FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M00_CorrectCode);
            monitorScreen.Completed();
            localObjectManager.serverObjectManager.CmdUnlockDoors();

            
        }


        // Code to check if there are errors
        // Code to tell server to Unlock Door (enable gameobject) for both players
    }

    public void OnClickDelete()
    {
        PlayUIKeyTone();
        currentString = "";
        inputField.text = "****";
    }

    public void PlayUIKeyTone()
    {
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_MAP_KeyTone);
    }

}
