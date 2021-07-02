using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TangentNodes.Network
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        private NetworkManagerTN networkManager = null;

        [Header("UI")]
        //[SerializeField] private MainMenu mainMenuPanel = null;
        [SerializeField] private GameObject landingPagePanel = null;
        [SerializeField] private TMP_InputField ipAddressInputField = null;
        [SerializeField] private Button joinButton = null;

        private void OnEnable()
        {
            NetworkManagerTN.OnClientConnected += HandleClientConnected;
            NetworkManagerTN.OnClientDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            NetworkManagerTN.OnClientConnected -= HandleClientConnected;
            NetworkManagerTN.OnClientDisconnected -= HandleClientDisconnected;
        }

        public void JoinLobby()
        {
            string ipAddress = ipAddressInputField.text;


            networkManager = FindObjectOfType<NetworkManagerTN>();
            networkManager.networkAddress = ipAddress;
            networkManager.StartClient();

            joinButton.interactable = false;

        }

        private void HandleClientConnected()
        {
            joinButton.interactable = true;

            gameObject.SetActive(false);
            landingPagePanel.SetActive(false);
        }

        private void HandleClientDisconnected()
        {
            joinButton.interactable = true;
            //mainMenuPanel.backEnabled = true;
        }
    }
}
