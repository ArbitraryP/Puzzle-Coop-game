using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using TMPro;

public class Feedback : MonoBehaviour
{
    [SerializeField] private GameObject panelRate = null;
    [SerializeField] private GameObject panelFeedback = null;
    
    [Header("Rate")]
    [SerializeField] private Button buttonConfirmRate = null;
    [SerializeField] private Image[] imageRateButtons = null;
    [SerializeField] private Sprite spriteStarLine = null;
    [SerializeField] private Sprite spriteStarFilled = null;
    [SerializeField] private Achievement achievementToUnlock = null;

    [Header("Feedback")]
    [SerializeField] private TMP_InputField inputFieldFeedback = null;
    [SerializeField] private Button buttonFeedbackClose = null;
    [SerializeField] private Button buttonFeedbackSend = null;
    [SerializeField] private TMP_Text textButtonClose = null;
    [SerializeField] private TMP_Text textTitle = null;
    [SerializeField] private TMP_Text textStatus = null;



    [Header("Data")]
    [SerializeField] private int rating = 0;
    [SerializeField] private bool hasRated = false;
    [SerializeField] private Coroutine coroutine = null;

    private readonly string formURL = "https://docs.google.com/forms/d/e/1FAIpQLSeGOTplCSc-Zihutuyd3Dus2UQYhmDXaL48j7QpcN6zaDENFA/formResponse";

    private void Start()
    {
        // Use start to load after PlayerProgress has been initialized
        PlayerProgress playerProgress = FindObjectOfType<PlayerProgress>();
        if (!playerProgress) return;

        SetRating(playerProgress.gameRating);

        // Skip Rating Page when player already rated.
        hasRated = playerProgress.gameRating > 0;
        

        // Proceeds to text Feedback when playerProgress has already rated
        panelRate.SetActive(!hasRated);
        panelFeedback.SetActive(hasRated);
        buttonFeedbackClose.gameObject.SetActive(hasRated);
        buttonFeedbackSend.interactable = !hasRated;


    }
   

    public void SetRating(int newRating)
    {
        rating = newRating;

        // enable Confirm Rate Button
        buttonConfirmRate.interactable = rating > 0;

        foreach (Image image in imageRateButtons)
            image.sprite = spriteStarLine;

        for (int i = 0; i < rating; i++)
            imageRateButtons[i].sprite = spriteStarFilled;

    }

    public void OnClickLater()
    {
        PlayUIButtonClick();
        GameObject.Destroy(this.gameObject);
    }

    
    public void OnClickRate()
    {
        PlayUIButtonClick();
        if (rating <= 0)
            return;

        // rating will be set when feedback is sent completely

        panelRate.SetActive(false);
        panelFeedback.SetActive(true);
    }



    public void OnClickSendFeedback()
    {
        PlayUIButtonClick();
        // Call Google forms, send Rate and Feedback 

        // Setup Build Version Response
        string buildVersion = "internal";

        // Setup Is Game Finished Response
        string isFinished = "No";

        PlayerProgress playerProgress = FindObjectOfType<PlayerProgress>();
        if (playerProgress)
            isFinished = playerProgress.IsGameFinished ? "Yes" : "No";

        // Setup Rating Response. dont send rate if hasRated already.
        string rate = "";
        if (!hasRated)
            rate = rating.ToString();

        // Setup Feedback Text Response
        string feedback = inputFieldFeedback.text.Trim();

        
        // Show Sending Message
        textTitle.gameObject.SetActive(false);
        inputFieldFeedback.gameObject.SetActive(false);
        buttonFeedbackSend.gameObject.SetActive(false);

        buttonFeedbackClose.gameObject.SetActive(false);
        textStatus.gameObject.SetActive(true);
        textStatus.text = "Sending feedback...";


        // Post Data to Google Forms
        coroutine = StartCoroutine(Post(
            buildVersion,
            isFinished,
            rate,
            feedback
            ));

    }

    public void OnTextValueChange()
    {
        // Will not need to be filled if player has not rated yet.
        if (!hasRated) return;
        buttonFeedbackSend.interactable = !string.IsNullOrWhiteSpace(inputFieldFeedback.text);
    }

    private IEnumerator Post(string buildVersion, string isFinished, string rating, string feedback)
    {
        WWWForm form = new WWWForm();

        // Build Version
        form.AddField("entry.1135759899", buildVersion);

        // Finished the Game
        form.AddField("entry.1329305712", isFinished);

        // Rating
        form.AddField("entry.1850899743", rating);

        // Feedback
        form.AddField("entry.129508756", feedback);


        UnityWebRequest www = UnityWebRequest.Post(formURL, form);
        
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Feedback send failed: " + www.error);

            // Used to make a general error term for connection error
            if(www.result == UnityWebRequest.Result.ConnectionError)
                textStatus.text = "Send Failed: Unable to connect to server.";

        }
        else
        {
            Debug.Log("Form upload completed!");
            textStatus.text = "Feedback sent!\nThank you for playing!";

            // Sets that the player has already rated if there is an included rating in sent data
            // Also Unlocks Achievement if send data has an included rating.
            PlayerProgress playerProgress = FindObjectOfType<PlayerProgress>();
            if (playerProgress && rating != "")
            {
                FindObjectOfType<PlayerProgress>().gameRating = int.Parse(rating);
                playerProgress.UnlockAchievement(achievementToUnlock);
            }
        }

        buttonFeedbackClose.gameObject.SetActive(true);
        textButtonClose.text = "Close";
    }

    public void PlayUIButtonClick()
    {
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_GEN_MenuButtonClick);
    }




}
