using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

public class SteamNotRunningMessage : MonoBehaviour
{
    public void OnClickRetry()
    {
        //SceneManager.LoadScene("Scene_Lobby");
        Application.Quit();
    }
}
