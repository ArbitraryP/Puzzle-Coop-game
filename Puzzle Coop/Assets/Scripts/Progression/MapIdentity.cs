using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapIdentity : MonoBehaviour
{
    [SerializeField]
    private int mapIndexNumber;
    public int MapIndexNumber
    {
        get { return mapIndexNumber; }
    }

    public void SetMapAsSelectable(bool isUnlocked)
    {
        Button button = GetComponent<Button>();
        if (button == null) { return; }

        button.interactable = isUnlocked;

    }
}
