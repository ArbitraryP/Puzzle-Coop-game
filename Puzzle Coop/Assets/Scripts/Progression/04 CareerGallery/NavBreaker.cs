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

    [Header("Data")]
    [SerializeField] private string currentInput = null;
    [SerializeField] private int correctIndex = 0;

    public void SetupLightSolution(int correctLight)
    {
        correctIndex = correctLight;
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
        if (currentInput != lightSolution[correctIndex])
            return;

        foreach(Button button in buttons)
            button.interactable = false;
        
        light2DLight.color = Color.green;
        // call Server to Open Lights
        // Play sound
        
    }

}
