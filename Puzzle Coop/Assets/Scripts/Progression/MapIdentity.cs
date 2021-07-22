using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapIdentity : MonoBehaviour
{
    public Map map = null;

    public void SetMapAsSelectable(bool isUnlocked)
    {
        Button button = GetComponent<Button>();
        if (button == null) { return; }

        button.interactable = isUnlocked;

    }
}
