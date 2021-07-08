using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TangentNodes.Network;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class SettingsAndExit : MonoBehaviour
{
    [SerializeField] private NetworkManagerTN networkManager = null;
    public NetworkIdentity myPlayerIdentity = null;

    [Header("UI")]
    [SerializeField] private Toggle toggleFullScreen = null;
    [SerializeField] private TMP_Text textDescription = null;
    [SerializeField] private GameObject panelQuitConfirm = null;

    

    private static SettingsAndExit instance;

    private void Awake()
    {
        //Find netWorkManager if returned to the Scene
        if (!networkManager) { networkManager = FindObjectOfType<NetworkManagerTN>(); }

        //Set the toggle based on if it is Fullscreen or not
        toggleFullScreen.isOn = Screen.fullScreen;


        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else { instance = this; }

        DontDestroyOnLoad(gameObject);
    }


    public void SetFullScreen(bool isFullScreen)
    {
        //Screen.fullScreen = isFullScreen;
       
        if (isFullScreen)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        
        }
        else
        {
            Screen.SetResolution(1024, 576, false);
            
        }
           

    }

    public void OnToggleQuit(bool isToggleOn)
    {
        panelQuitConfirm.SetActive(isToggleOn);

        // If Identity not null means it is InGame
        if (myPlayerIdentity)
        {
            textDescription.text = "Are you sure you want to\nRETURN to Main Menu?";
        }
        else
        {
            textDescription.text = "Are you sure you want to\nQUIT the Game?";
        }
    }

    public void OnClickQuitYes()
    {
        // If Identity not null means it is InGame
        if (myPlayerIdentity)
        {
            if (myPlayerIdentity.isServer) { networkManager.StopHost(); }
            else if (myPlayerIdentity.isClient)
            {
                networkManager.StopClient();
                SceneManager.LoadScene("Scene_Lobby");
            }
        }
        else
        {
            //Do some Autosaving first
            Application.Quit();
        }
    }
}
