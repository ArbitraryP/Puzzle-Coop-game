using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TangentNodes.Network
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private NetworkManagerTN networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;
        [SerializeField] private GameObject achievementsPanel = null;
        [SerializeField] private GameObject creditsPanel = null;
        [SerializeField] private GameObject hostjoinPanel = null;
        [SerializeField] private GameObject joinWaitPanel = null;

        [Header("Achievements")]
        [SerializeField] private Panel_Achievement panelAchievementPrefab = null;
        [SerializeField] private Transform panelAchievementParent = null;
        [SerializeField] private List<Achievement> achievements = null;
        [SerializeField] private List<Panel_Achievement> panelAchievements = null;

        [Header("Feedback")]
        [SerializeField] private Feedback feedbackPrefab = null;

        [Header("Join Wait UI")]
        [SerializeField] private TMP_Text textWaitingArea = null;
        [SerializeField] private Button buttonReturn = null;
        private string waitingAreaDefaultText = null;


        private void Awake()
        {
            achievements = new List<Achievement>(Resources.LoadAll<Achievement>("ScriptableObjects/Achievements"));
            waitingAreaDefaultText = textWaitingArea.text;
        }


        private void Start()
        {
            DisplayAchievements();
        }

        public void HostLobby()
        {
            // Add Code that determines if it is LAN mode or Steam Mode

            // Code for FizzySteamworks Transport
            
            SteamLobby steamLobby = FindObjectOfType<SteamLobby>();
            if (!steamLobby)
            {
                Debug.Log("SteamLobby not found");
            }
            else
            {
                steamLobby.HostLobby();
            }
            /*
            */

            // Code for Telepathy Transport (LAN)
            /*
            networkManager = FindObjectOfType<NetworkManagerTN>();
            networkManager.StartHost();
            
            */
            landingPagePanel.SetActive(false);
        }


        public void BackToMainMenu(GameObject paneltoClose)
        {
            landingPagePanel.SetActive(true);
            paneltoClose.SetActive(false);
        }

        private void DisplayAchievements()
        {
            for (int i = 0; i < achievements.Count; i++)
            {
                var panelAchiv = GameObject.Instantiate(panelAchievementPrefab, panelAchievementParent);
                panelAchiv.DisplayAchievement(achievements[i]);
                panelAchievements.Add(panelAchiv);
            }
        }

        public void RefreshAchievements()
        {
            foreach (Panel_Achievement panel in panelAchievements)
                panel.RefreshShowUnlockedAchievement();
        }

        public void ShowFeedbackPanel()
        {
            GameObject.Instantiate(feedbackPrefab);
            PlayUIButtonClick();
        }

        public void JumpToJoinWait(string message, bool showButton = false)
        {
            landingPagePanel.SetActive(false);
            achievementsPanel.SetActive(false);
            creditsPanel.SetActive(false);
            hostjoinPanel.SetActive(false);
            joinWaitPanel.SetActive(true);

            textWaitingArea.text = message;
            buttonReturn.gameObject.SetActive(showButton);
        }

        public void OnClickJoinWaitReturn()
        {
            hostjoinPanel.SetActive(true);
            joinWaitPanel.SetActive(false);
            textWaitingArea.text = waitingAreaDefaultText;
        }

        public void PlayUIButtonClick()
        {
            FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_GEN_MenuButtonClick);
        }



    }
}
