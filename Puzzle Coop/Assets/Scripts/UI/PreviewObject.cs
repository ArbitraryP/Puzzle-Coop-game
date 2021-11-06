using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour, IClickable
{
    [SerializeField] private GameObject panelTarget = null;
    [SerializeField] private CameraControl cameraControl = null;

    [SerializeField] private bool isCloseButton = false;

    [SerializeField] private string soundToPlay = "SFX_MAP_Paper";


    public void Click()
    {
        if (isCloseButton) return;

        PlaySound();

        panelTarget.SetActive(true);
        cameraControl.button_up.gameObject.SetActive(false);
        cameraControl.button_down.gameObject.SetActive(false);
    }

    public void OnClickCloseButton()
    {
        panelTarget.SetActive(false);
        cameraControl.button_up.gameObject.SetActive(true);
        cameraControl.button_down.gameObject.SetActive(true);
    }

    public void PlaySound()
    {
        if (soundToPlay == "") return;
        FindObjectOfType<AudioManager>()?.Play(soundToPlay);
    }

}
