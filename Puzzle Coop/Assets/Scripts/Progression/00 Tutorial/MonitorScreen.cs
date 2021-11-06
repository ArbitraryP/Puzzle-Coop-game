using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MonitorScreen : MonoBehaviour
{
    [SerializeField] private GameObject panelPasscode = null;
    [SerializeField] private GameObject[] buttonsUpDown = null;
    [SerializeField] private Button monitorButton = null;
    [SerializeField] private TMP_Text monitorText = null;

    public void Completed()
    {
        monitorButton.interactable = false;
        monitorText.text = "Door Unlocked.";
    }

    public void OnClick()
    {
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_GEN_MenuButtonClick);
        panelPasscode.SetActive(true);
        foreach (GameObject button in buttonsUpDown)
            button.SetActive(false);
    }

}
