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
            // Code Used for LAN connection
            /*
            NetworkManagerTN.OnClientConnected += HandleClientConnected;
            NetworkManagerTN.OnClientDisconnected += HandleClientDisconnected;
            */
        }

        private void OnDisable()
        {
            // Code Used for LAN connection
            /*
            NetworkManagerTN.OnClientConnected -= HandleClientConnected;
            NetworkManagerTN.OnClientDisconnected -= HandleClientDisconnected;
            */
        }

        public void JoinLobby()
        {
            // Add Code that determines if it is LAN mode or Steam Mode

            // Code for LAN 
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
