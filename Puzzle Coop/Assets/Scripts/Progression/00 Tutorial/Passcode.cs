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
    

    public void OnClickInput(string character)
    {
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
        if(currentString != correctCode)
        {
            currentString = "XXXX";
            inputField.text = currentString;

            // Play sound error
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

            localObjectManager.serverObjectManager.CmdM00_PassCodeOn();

            
        }


        // Code to check if there are errors
        // Code to tell server to Unlock Door (enable gameobject) for both players
    }

    public void OnClickDelete()
    {
        currentString = "";
        inputField.text = "****";
    }


}
