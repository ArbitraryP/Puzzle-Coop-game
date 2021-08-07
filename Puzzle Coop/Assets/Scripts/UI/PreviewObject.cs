using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour, IClickable
{
    [SerializeField] private GameObject panelTarget = null;
    [SerializeField] private CameraControl cameraControl = null;

    public void Click()
    {
        panelTarget.SetActive(true);
        cameraControl.button_up.gameObject.SetActive(false);
        cameraControl.button_down.gameObject.SetActive(false);
    }
}
