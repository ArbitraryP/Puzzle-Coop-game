using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class UI_TerminalScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private MapObjectManager_L localObjectManager = null;
    [SerializeField] private TMP_Text textTerminalName = null;
    [SerializeField] private TMP_Text textMessage = null;
    [SerializeField] private TMP_Text textStatus = null;
    [SerializeField] private Image imageProgressFill = null;
    [SerializeField] private Image imageButton = null;
    [SerializeField] private Button buttonDownload = null;
    [SerializeField] private VideoPlayer videoPlayerCred = null;
    

    [Header("Data")]
    [SerializeField] private int currentMessage = 1;
    [SerializeField] private int maxMessage = 1;
    [SerializeField] private bool isTerminalA = true;
    [SerializeField] private bool isRetryButton = false;
    
    [Header("Data: ProgressFill")]
    [SerializeField] private bool isDownloading = false;
    [SerializeField] private bool isFinalMessage = false;
    [SerializeField] private float currentPercentage = 0f;
    [Range(0.01f, 1f)]
    [SerializeField] private float finalMaxProgress = 0f;
    [Range(0.01f, 3f)]
    [SerializeField] private float downloadSpeed = 0.1f;
    [Range(0.001f, 1f)]
    [SerializeField] private float bufferDifference = 0.02f;
    [SerializeField] private float bufferLastPercentage = 0f;
    private bool hasPlayedErrorSound = false;

    [Header("Data: Text Display")]
    [SerializeField] private bool isDisplayingText = false;
    [SerializeField] private string textToDisplay = null;
    [Range(0.01f, 1f)]
    [SerializeField] private float textPercentToProgress = 0.60f;
    [Range(0.01f, 1f)]
    [SerializeField] private float finalMaxTextPerc = 0f;
    [Range(0.001f, 1f)]
    [SerializeField] private float bufferTxtDiff = 0.02f;
    [SerializeField] private float bufferTxtLast = 0f;
    [Range(0, 10)]
    [SerializeField] private int extraCharacters = 0;

    [Header("Data: Button")]
    [SerializeField] private Sprite spriteDownload = null;
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

    public void SetTerminalMessage(int currentMsg, string newMessage, bool isFinalMsg)
    {
        currentMessage = currentMsg;
        textToDisplay = newMessage;

        isFinalMessage = isFinalMsg;
        if (isFinalMessage) downloadSpeed = 0.05f;
    }

    public void ResetProgress()
    {
        buttonDownload.interactable = true;
        imageButton.sprite = spriteDownload;

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
        PlayUIButtonClick();
        if (isRetryButton) return;

        ResetProgress();
        buttonDownload.interactable = false;
        imageButton.sprite = spriteDownloading;
               

        char partnerTerminal = isTerminalA? 'B' : 'A';
        textStatus.text =
            "File " +
            currentMessage +
            " out of " +
            maxMessage +
            ":  Waiting for Terminal " +
            partnerTerminal + ".";

        // Tell Server that Player is Ready to Download
        TerminalManager terminalManager = FindObjectOfType<TerminalManager>();
        if (!terminalManager) return;

        terminalManager.CmdPlayerReadytoDownload();

    }

    public void OnClickRetryDownload()
    {
        buttonDownload.interactable = false;
        imageButton.sprite = spriteDownloading;

        char partnerTerminal = isTerminalA ? 'B' : 'A';
        textStatus.text =
            "File " +
            currentMessage +
            " out of " +
            maxMessage +
            ":  Waiting for Terminal " +
            partnerTerminal + ".";

        // Call Server Command to spawn BSOD Cutscene when both players clicked
        
        TerminalManager terminalManager = FindObjectOfType<TerminalManager>();
        if (!terminalManager) return;

        terminalManager.CmdPlayerReadyToRetry();
    }

    public void StartDownload()
    {
        isDownloading = true;
        isDisplayingText = true;
        FindObjectOfType<AudioManager>()?.PlayNonRepeat(AudioManager.SoundNames.SFX_M09_TerminalTyping);
    }

    public void StartRetry()
    {
        // Sets the Volume of AudioSource first before playing the cutscene
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager)
        {
            audioManager.ApplySoundSettings();
            videoPlayerCred.SetTargetAudioSource(0, audioManager.GetCutsceneAudioSource());
        }
        else Debug.LogWarning("Audio Manager not found.");

        localObjectManager.M09_HideUI();
        videoPlayerCred.enabled = true;
        videoPlayerCred.loopPointReached += localObjectManager.M09_OnVideoPlayerEnded;
    }


    private void UpdateDownloadingProgress()
    {
        // Code to slow down display of percentage downloading text 
        if(currentPercentage == 0 || bufferLastPercentage < currentPercentage - bufferDifference)
        {
            bufferLastPercentage = currentPercentage;

            if (isFinalMessage && currentPercentage >= finalMaxProgress)
                textStatus.text =
                    "File " +
                    currentMessage +
                    " out of " +
                    maxMessage +
                    ":  Download interrupted. Retrying... " +
                    (char) Random.Range('a', 'z');

            else
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

        // Code that limits shown percentage if isFinalMessage
        imageProgressFill.fillAmount = isFinalMessage?
            Mathf.Clamp(currentPercentage, 0f, finalMaxProgress) : currentPercentage;

        
        Color color = new Color32(186, 40, 54, 255);
        imageProgressFill.color = isFinalMessage && currentPercentage > finalMaxProgress?
            color : Color.white;

        // Code that will play sound when it reached FinalMessage Limit
        if(!hasPlayedErrorSound && isFinalMessage && currentPercentage > finalMaxProgress)
        {
            FindObjectOfType<AudioManager>()?.PlayNonRepeat(AudioManager.SoundNames.SFX_M09_TerminalError);
            hasPlayedErrorSound = true;
        }

        isDownloading = currentPercentage < 1.0f;



        // --- Code that runs when download is completed ---

        if (isDownloading) return;

        FindObjectOfType<AudioManager>()?.Stop(AudioManager.SoundNames.SFX_M09_TerminalTyping);

        if (isFinalMessage)
        {
            // Add Function to Button that will run the Cutscene Method Instead
            buttonDownload.onClick.RemoveAllListeners();
            buttonDownload.onClick.AddListener(OnClickRetryDownload);
            isRetryButton = true;

            imageButton.sprite = spriteDownload;
            buttonDownload.interactable = true;
            textStatus.text =
                "File " +
                currentMessage +
                " out of " +
                maxMessage +
                ":  Download failed. Click button to retry.";

            return;
        }

        imageButton.sprite = spriteDownloaded;
        textStatus.text =
            "File " +
            currentMessage +
            " out of " +
            maxMessage +
            ":  Download completed.";

        // Call Server to Unlock Hallway
        FindObjectOfType<TerminalManager>()?.CmdDownloadCompleted();

    }

    private void UpdateTextDisplay()
    {
        float currentTextPerc = currentPercentage / textPercentToProgress;
        
        if (!isDisplayingText) return;


        if (currentPercentage == 0 || bufferTxtLast < currentTextPerc - bufferTxtDiff || currentPercentage == 1f)
        {
            bufferTxtLast = currentTextPerc;

            string tempString = "";

            // Code that Limits the textDisplay if isFinalMessage
            int maxString = textToDisplay.Length;
            if (isFinalMessage)
                maxString = Mathf.RoundToInt(finalMaxTextPerc * textToDisplay.Length);

            // Code that displays message
            for (int i = 0; i < textToDisplay.Length; i++)
            {
                //if (i > Mathf.RoundToInt(currentTextPerc * textToDisplay.Length))
                if (i > Mathf.Clamp( Mathf.RoundToInt(currentTextPerc * textToDisplay.Length), 0, maxString) )
                    break;
                                
                tempString += textToDisplay[i];
            }

            isDisplayingText = textToDisplay != tempString;

            // Code that stops typing sounds when it is not displaying text anymore
            if(!isDisplayingText)
                FindObjectOfType<AudioManager>()?.Stop(AudioManager.SoundNames.SFX_M09_TerminalTyping);

            // Calculates The number of Char left to be extra chars
            int tempExtraCharLength = Mathf.Clamp(extraCharacters, 0, textToDisplay.Length - tempString.Length);
            

            // Add Code that Halts the Text Display when 80% and continues to place extraCharacters instead
            // if (isFinalMessage) tempExtraCharLength = Mathf.Clamp(tempExtraCharLength, 1, tempExtraCharLength);

            if(tempExtraCharLength != 0)
            {
                tempString += "<color=#ffffffaa>";
                for (int i = 0; i < tempExtraCharLength; i++)
                {
                    tempString += (char)Random.Range('A', 'z');
                }
                tempString += "</color>";
            } 

            textMessage.text = tempString;
            
        }

    }

    private void PlayUIButtonClick()
    {
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_GEN_MenuButtonClick);
    }

}
