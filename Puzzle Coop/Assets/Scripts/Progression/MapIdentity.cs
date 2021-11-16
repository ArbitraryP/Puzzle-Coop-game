using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class MapIdentity : MonoBehaviour
{
    public Map map = null;
    [SerializeField] private Light2D lightGlow = null;
    [SerializeField] private Image imageButton = null;

    private void Awake()
    {
        lightGlow.gameObject.SetActive(false);
    }

    public void SetMapAsSelectable(bool isUnlocked)
    {
        // lightGlow not used
        lightGlow.gameObject.SetActive(isUnlocked);
        lightGlow.color = Color.red;

        Button button = GetComponent<Button>();
        if (button == null) { return; }

        button.interactable = isUnlocked;

    }

    public void SetMapAsCompleted()
    {
        // lightGlow not used
        lightGlow.gameObject.SetActive(true);
        lightGlow.color = Color.green;
    }

    public void SetButtonImage(Sprite image)
    {
        imageButton.sprite = image;
    }
}
