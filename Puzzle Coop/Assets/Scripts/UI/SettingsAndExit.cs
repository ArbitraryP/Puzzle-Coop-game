using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TangentNodes.Network;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;
using System;

public class SettingsAndExit : MonoBehaviour
{
    [SerializeField] private NetworkManagerTN networkManager = null;
    public NetworkIdentity myPlayerIdentity = null;

    [Header("UI")]
    [SerializeField] private Toggle toggleFullScreen = null;
    [SerializeField] private TMP_Text textDescription = null;
    [SerializeField] private GameObject panelQuitConfirm = null;
    [SerializeField] private GameObject panelHowTo = null;
    [SerializeField] private GameObject panelSettings = null;
    [SerializeField] private Toggle toggleHowTo = null;
    [SerializeField] private Toggle toggleSettings = null;
    [SerializeField] private Toggle toggleQuitConfirm = null;
    [SerializeField] private Slider sliderSFX = null;
    [SerializeField] private Slider sliderBGM = null;

    [Header("How To Play")]
    [SerializeField] private GameObject[] panelHowToPages = null;
    [SerializeField] private int currentPage = 0;
    [SerializeField] private bool isAlreadyOpened = false;

    [Header("Loading Screen")]
    [SerializeField] private GameObject panelLoadingScreen = null;

    [Header("Sound Volume")]
    [SerializeField] private AudioManager audioManager = null;


    private NetworkManagerTN room;
    private NetworkManagerTN Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerTN;
        }
    }

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

        SceneManager.activeSceneChanged += OnSceneChanged;
    }


    private void OnDestroy() => SceneManager.activeSceneChanged -= OnSceneChanged;

    public void SetFullScreen(bool isFullScreen)
    {
        //Screen.fullScreen = isFullScreen;

        if (isFullScreen)
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        
        else
            Screen.SetResolution(1024, 576, false);
            
    }


    private void CloseOtherToggles(int toggleIndex)
    {
        // 0 - ToggleHowTo / 1 - ToggleSettings / 2 - ToggleQuitConfirm

        switch (toggleIndex)
        {
            case 0:
                toggleSettings.isOn = false;
                toggleQuitConfirm.isOn = false;
                break;

            case 1:
                toggleHowTo.isOn = false;
                toggleQuitConfirm.isOn = false;
                break;

            case 2:
                toggleSettings.isOn = false;
                toggleHowTo.isOn = false;
                break;

            default:
                break;
        }

    }

    public void OnSceneChanged(Scene currentScene, Scene nextScene)
    {
        toggleHowTo.isOn = false;
        if (nextScene.name == "Scene_Map_Select" && !isAlreadyOpened)
        {
            toggleHowTo.isOn = true;
            isAlreadyOpened = true;
        }
    }

    public void EnableMenu(bool active)
    {
        toggleHowTo.gameObject.SetActive(active);
        toggleSettings.gameObject.SetActive(active);
        toggleQuitConfirm.gameObject.SetActive(active);
    }

    public void OnToggleHowTo(bool value)
    {
        PlayUIClickSound();
        panelHowTo.SetActive(value);
        if(value)
            CloseOtherToggles(0);

        foreach (GameObject panel in panelHowToPages)
            panel.SetActive(false);

        panelHowToPages[0].SetActive(true);

    }

    public void OnToggleSettings(bool value)
    {
        PlayUIClickSound();
        panelSettings.SetActive(value);
        if (value) 
            CloseOtherToggles(1);
    }


    public void OnToggleQuit(bool isToggleOn)
    {
        PlayUIClickSound();
        panelQuitConfirm.SetActive(isToggleOn);
        if (isToggleOn) 
            CloseOtherToggles(2);

        // If Identity not null means it is InGame
        if (myPlayerIdentity)
        {
            // If is Server, return to Map Select

            if (myPlayerIdentity.isServer &&
                SceneManager.GetActiveScene().name.StartsWith("Scene_Map") &&
                SceneManager.GetActiveScene().name != "Scene_Map_Select")
            {

                textDescription.text = "Are you sure you want to\nRETURN to Map Hub?";
                return;
            }

            textDescription.text = "Are you sure you want to\nRETURN to Main Menu?";
            return;
        }
        
        textDescription.text = "Are you sure you want to\nQUIT the Game?";
        
    }

    public void OnClickQuitYes()
    {
        //Do some Autosaving first

        // If Identity is null means it is not InGame
        if (!myPlayerIdentity)
        {
            Application.Quit();
            return;
        }

        if (!myPlayerIdentity.isServer)
        {
            networkManager.StopClient();
            SceneManager.LoadScene("Scene_Lobby");
            return;
        }

        if (myPlayerIdentity.isServer &&
            SceneManager.GetActiveScene().name.StartsWith("Scene_Map") &&
            SceneManager.GetActiveScene().name != "Scene_Map_Select")
        {
            Room.ServerChangeScene("Scene_Map_Select");
            return;
        }

        networkManager.StopHost();

    }



    // How To Play Page Navigation
    public void OnClickHowToNext()
    {
        if (panelHowToPages == null)
            return;

        if (currentPage >= panelHowToPages.Length - 1)
            toggleHowTo.isOn = false;

        currentPage = currentPage >= panelHowToPages.Length - 1 ? 0 : currentPage + 1;
        foreach (GameObject panel in panelHowToPages)
            panel.SetActive(false);

        panelHowToPages[currentPage].SetActive(true);
    }
   

    public void EnableLoadingScreen(bool state)
    {
        panelLoadingScreen.SetActive(state);
        EnableMenu(!state);
    }


    public void ChangeVolumeBGM(float value)
    {
        audioManager.masterVolumeBGM = value;
    }

    public void ChangeVolumeSFX(float value)
    {
        audioManager.masterVolumeSFX = value;
    }

    // Code to call when loading the volume settings from save
    public void SetVolume(float volumeBGM = 1f, float volumeSFX = 1f)
    {
        sliderBGM.value = volumeBGM;
        sliderSFX.value = volumeSFX;
    }

    // Code to call to play UI Button Click Sound
    public void PlayUIClickSound()
    {
        // This will catch the exception since Fullscreen toggle will change before AudioManager would initialize
        try { FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_GEN_MenuButtonClick); }
        catch(NullReferenceException e) { } 

    }

}
