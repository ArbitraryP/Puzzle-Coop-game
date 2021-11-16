using System.Collections.Generic;
using UnityEngine;

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

        [Header("Achievements")]
        [SerializeField] private Panel_Achievement panelAchievementPrefab = null;
        [SerializeField] private Transform panelAchievementParent = null;
        [SerializeField] private List<Achievement> achievements = null;

        [Header("Feedback")]
        [SerializeField] private Feedback feedbackPrefab = null;

        private void Awake()
        {
            achievements = new List<Achievement>(Resources.LoadAll<Achievement>("ScriptableObjects/Achievements"));
            DisplayAchievements();
        }

        public void HostLobby()
        {
            networkManager = FindObjectOfType<NetworkManagerTN>();

            networkManager.StartHost();

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
            }
        }

        public void ShowFeedbackPanel()
        {
            GameObject.Instantiate(feedbackPrefab);
            PlayUIButtonClick();
        }

        public void PlayUIButtonClick()
        {
            FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_GEN_MenuButtonClick);
        }

    }
}
