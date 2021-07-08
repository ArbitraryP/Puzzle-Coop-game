using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TangentNodes.Network
{
    // This object will instantiate when player is disconnected
    public class DisconnectPanel : MonoBehaviour
    {
        public void OnClickClose()
        {
            // Clicking Continue will change scene for the Client

            SceneManager.LoadScene("Scene_Lobby");
        }

    }
}