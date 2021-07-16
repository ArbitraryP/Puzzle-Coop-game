using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace TangentNodes.Network
{
    // This object will instantiate when player is disconnected
    public class DisconnectPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text textTitle = null;
        [SerializeField] private TMP_Text textDescription = null;
        private bool isHostLostMode = true;

        /// <summary>
        /// Changes the text to tell the Host lost connection
        /// </summary>
        public void ChangeTextToLostHost()
        {
            textTitle.text = "DISCONNECTED";
            textDescription.text = "Lost connection to server.";
            isHostLostMode = true;
        }

        /// <summary>
        /// Changes the text to tell the Client lost connection
        /// </summary>
        public void ChangeTextToLostClient()
        {
            textTitle.text = "DISCONNECTED";
            textDescription.text = "Your partner lost connection.";
            isHostLostMode = false;
        }

        public void OnClickClose()
        {
            if (isHostLostMode)
            {
                // Clicking Continue will change scene for the Client
                SceneManager.LoadScene("Scene_Lobby");
            }
            else
            {
                // Clicking Continue will stop host for the Host
                FindObjectOfType<NetworkManagerTN>()?.StopHost();
            }
            
        }

    }
}