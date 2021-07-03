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

        public void HostLobby()
        {
            networkManager.StartHost();

            landingPagePanel.SetActive(false);
        }


        public void BackToMainMenu(GameObject paneltoClose)
        {
            landingPagePanel.SetActive(true);
            paneltoClose.SetActive(false);
        }

    }
}
