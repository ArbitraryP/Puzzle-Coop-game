using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Passcode : MonoBehaviour
{
    private string currentString = "";
    [SerializeField] private TMP_InputField inputField = null;

    public void OnClickInput(string character)
    {
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
        // Code to check if there are errors
        // Code to tell server to Unlock Door (enable gameobject) for both players
    }

    public void OnClickDelete()
    {
        currentString = "";
        inputField.text = "****";
    }


}
