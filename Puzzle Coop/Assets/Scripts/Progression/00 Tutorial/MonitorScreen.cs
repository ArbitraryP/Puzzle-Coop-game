using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorScreen : MonoBehaviour, IClickable
{
    [SerializeField] private GameObject panelPasscode = null;
    [SerializeField] private GameObject[] buttonsUpDown = null;

    public void Click()
    {
        panelPasscode.SetActive(true);
        foreach (GameObject button in buttonsUpDown)
            button.SetActive(false);
    }

}
