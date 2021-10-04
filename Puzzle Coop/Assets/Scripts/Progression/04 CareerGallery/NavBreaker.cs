using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Experimental.Rendering.Universal;

public class NavBreaker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button[] buttons = null;
    [SerializeField] private TMP_Text textDisplay = null;

    [Header("Light Solution")]
    [SerializeField] private SpriteRenderer lightObject = null;
    [SerializeField] private Light2D light2DLight = null;
    [SerializeField] private Sprite[] lightSprites = null;
    [SerializeField] private string[] lightSolution = null;

    [Header("Door Solution")]
    [SerializeField] private string doorSolution = null;
    [SerializeField] private Light2D door2DLight = null;

    [Header("Data")]
    [SerializeField] private string currentInput = null;
    [SerializeField] private int correctLightIndex = 0;
    [SerializeField] private bool enableDoorSolution = false;

    public void SetLightSolution(int correctLight)
    {
        correctLightIndex = correctLight;
        lightObject.sprite = lightSprites[correctLight];
    }

    public void OnClickButtonDelete()
    {
        currentInput = "";
        DisplayText();
    }

    public void OnClickButton(string inputCode)
    {
        if (currentInput.Length >= 5)
            currentInput = "";

        currentInput += inputCode;

        DisplayText();
        CheckIfCorrect();
    }

    private void DisplayText()
    {
        textDisplay.text = currentInput.ToUpper();
        for (int i = 0; i < 5 - currentInput.Length; i++)
            textDisplay.text += "*";
    }

    private void CheckIfCorrect()
    {

        if (CheckLightSolution())
        {
            light2DLight.color = Color.green;
            FindObjectOfType<MapObjectManager_S>()?.CmdM04_LightCompleted();
            enableDoorSolution = true;
            OnClickButtonDelete();
            // Play breaker unlock sound
            return;
        }


        if (CheckDoorSolution())
        {
            door2DLight.color = Color.green;
            foreach (Button button in buttons)
                button.interactable = false;
            FindObjectOfType<MapObjectManager_S>()?.CmdUnlockDoors();
            // Play breaker unlock sound
        }

    }

    private bool CheckLightSolution() =>
        currentInput == lightSolution[correctLightIndex] && !enableDoorSolution;

    private bool CheckDoorSolution() =>    
        currentInput == doorSolution && enableDoorSolution;
                     

}
