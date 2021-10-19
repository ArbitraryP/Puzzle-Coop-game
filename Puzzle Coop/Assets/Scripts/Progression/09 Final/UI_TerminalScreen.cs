using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TerminalScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text textTerminalName = null;
    [SerializeField] private TMP_Text textMessage = null;
    [SerializeField] private TMP_Text textStatus = null;
    [SerializeField] private Image imageProgressFill = null;
    [SerializeField] private Image imageButton = null;
    [SerializeField] private Button buttonDownload = null;

    [Header("Data")]
    [SerializeField] private int currentMessage = 1;
    [SerializeField] private int maxMessage = 1;
    [SerializeField] private bool isTerminalA = true;
    
    [Header("Data: ProgressFill")]
    [SerializeField] private bool isDownloading = false;
    [SerializeField] private float currentPercentage = 0f;
    [Range(0.01f, 3f)]
    [SerializeField] private float downloadSpeed = 0.1f;
    [Range(0.001f, 1f)]
    [SerializeField] private float bufferDifference = 0.02f;
    [SerializeField] private float bufferLastPercentage = 0f;

    [Header("Data: Text Display")]
    [SerializeField] private bool isDisplayingText = false;
    [SerializeField] private string textToDisplay = null;
    [Range(0.01f, 1f)]
    [SerializeField] private float textMaxPercentage = 0.60f;
    [Range(0.001f, 1f)]
    [SerializeField] private float bufferTxtDiff = 0.02f;
    [SerializeField] private float bufferTxtLast = 0f;
    [Range(0, 10)]
    [SerializeField] private int extraCharacters = 0;

    [Header("Data: Button")]
    [SerializeField] private Sprite spriteDownloading = null;
    [SerializeField] private Sprite spriteDownloaded = null;


    private void Update()
    {
        if (!isDownloading) return;
        UpdateDownloadingProgress();
        UpdateTextDisplay();
            
    }

    public void SetupTerminal(bool terminalA, int maxMsg)
    {
        isTerminalA = terminalA;
        textTerminalName.text = "Terminal " + (isTerminalA ? "A" : "B");
        maxMessage = maxMsg;

        ResetProgress();
    }

    public void SetTerminalMessage(int currentMsg, string newMessage, bool isFinalMessage)
    {
        currentMessage = currentMsg;
        textToDisplay = newMessage;

        if (isFinalMessage)
            textMaxPercentage = 1f;
    }

    public void ResetProgress()
    {
        bufferLastPercentage = 0;
        bufferTxtLast = 0;
        currentPercentage = 0;
        imageProgressFill.fillAmount = 0;
        textMessage.text = "";

        textStatus.text =
            "File " +
            currentMessage +
            " out of " +
            maxMessage +
            ":  Click button to download file.";
    }

    public void OnClickDownload()
    {
        buttonDownload.interactable = false;
        imageButton.sprite = spriteDownloading;

        ResetProgress();

        char partnerTerminal = isTerminalA? 'B' : 'A';
        textStatus.text =
            "File " +
            currentMessage +
            " out of " +
            maxMessage +
            ":  Waiting for Terminal " +
            partnerTerminal + ".";

        // Tell Server that Player is Ready to Download
    }

    public void StartDownload()
    {
        isDownloading = true;
        isDisplayingText = true;
    }

    private void UpdateDownloadingProgress()
    {
        // Code to slow down display of percentage downloading text 
        if(currentPercentage == 0 || bufferLastPercentage < currentPercentage - bufferDifference)
        {
            bufferLastPercentage = currentPercentage;

            textStatus.text =
            "File " +
            currentMessage +
            " out of " +
            maxMessage +
            ":  Downloading... " +
            (currentPercentage * 100).ToString("0.0") +
            "%";

            
        }

        currentPercentage = Mathf.Clamp(currentPercentage + downloadSpeed * Time.deltaTime, 0, 1.0f);
        imageProgressFill.fillAmount = currentPercentage;
        isDownloading = currentPercentage < 1.0f;

        // --- Code that runs when download is completed


        if (isDownloading) return;
        
        buttonDownload.interactable = true;
        imageButton.sprite = spriteDownloaded;
        textStatus.text =
            "File " +
            currentMessage +
            " out of " +
            maxMessage +
            ":  Download completed.";
        
        // Call Server to Unlock Hallway


    }

    private void UpdateTextDisplay()
    {
        float currentTextPerc = currentPercentage / textMaxPercentage;
        
        if (!isDisplayingText) return;
        

        if (currentPercentage == 0 || bufferTxtLast < currentTextPerc - bufferTxtDiff)
        {
            bufferTxtLast = currentTextPerc;

            string tempString = "";


            for (int i = 0; i < textToDisplay.Length; i++)
            {
                if(i <= Mathf.RoundToInt(currentTextPerc * 100f))
                {
                    tempString += textToDisplay[i];
                }
            }

            isDisplayingText = textToDisplay != tempString;

            // Calculates The number of Char left to be extra chars
            int tempExtraCharLength = Mathf.Clamp(extraCharacters, 0, textToDisplay.Length - tempString.Length);

            if(tempExtraCharLength != 0)
            {
                tempString += "<color=#ggggggaa>";
                for (int i = 0; i < tempExtraCharLength; i++)
                {
                    tempString += (char)Random.Range('A', 'z');
                }
                tempString += "</color>";
            } 

            textMessage.text = tempString;
            
        }

    }

}
