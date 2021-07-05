using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsAndExit : MonoBehaviour
{
    [SerializeField] private Toggle toggleFullScreen = null;

    public void Start()
    {
        //Set the toggle based on if it is Fullscreen or not
        toggleFullScreen.isOn = Screen.fullScreen;
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
}
