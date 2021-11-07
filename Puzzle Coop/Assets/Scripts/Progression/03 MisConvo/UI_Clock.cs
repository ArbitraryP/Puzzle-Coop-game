using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Experimental.Rendering.Universal;

public class UI_Clock : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button hourButton = null;
    [SerializeField] private Button min10Button = null;
    [SerializeField] private Button min1Button = null;
    [SerializeField] private Button meridiemButton = null;
    [SerializeField] private GameObject inputBlocker = null;
    public Light2D clockLight = null;
    public GameObject iconButton = null;

    [SerializeField] private TMP_Text hourText = null;
    [SerializeField] private TMP_Text min10Text = null;
    [SerializeField] private TMP_Text min1Text = null;
    [SerializeField] private TMP_Text meridiemText = null;

    [Header("Data")]
    [Range(1, 12)]
    [SerializeField] private int selectedHour = 1;
    [Range(0, 5)]
    [SerializeField] private int selectedMin10 = 1;
    [Range(0, 9)]
    [SerializeField] private int selectedMin1 = 1;
    [SerializeField] private bool selectedMeridiemAM = true;

    [Header("Solution (Do not Edit here)")]
    [SerializeField] private int correctHour;
    [SerializeField] private int correctMinutes;
    [SerializeField] private bool correctMeridiemAM;

    private void Start()
    {
        // Generate Random Starting value (may start with correct value)

        selectedHour = Random.Range(1, 13);
        selectedMin10 = Random.Range(0, 6);
        selectedMin1 = Random.Range(0, 10);
        selectedMeridiemAM = Random.Range(0, 2) == 1;

        hourText.text = selectedHour.ToString();
        min10Text.text = selectedMin10.ToString();
        min1Text.text = selectedMin1.ToString();
        meridiemText.text = selectedMeridiemAM ? "AM" : "PM";
    }

    public void SetSolution(int hour, int minutes, bool meridiemAM)
    {
        correctHour = hour;
        correctMinutes = minutes;
        correctMeridiemAM = meridiemAM;
    }

    public void OnClickClockButton(string buttonName)
    {
        PlaySoundButtonClick();
        if (buttonName.ToLower() == "hour")
        {
            selectedHour = selectedHour >= 12 ? 1 : selectedHour + 1;
            hourText.text = selectedHour.ToString();
        }

        else if (buttonName.ToLower() == "min10")
        {
            selectedMin10 = selectedMin10 >= 5 ? 0 : selectedMin10 + 1;
            min10Text.text = selectedMin10.ToString();
        }

        else if (buttonName.ToLower() == "min1")
        {
            selectedMin1 = selectedMin1 >= 9 ? 0 : selectedMin1 + 1;
            min1Text.text = selectedMin1.ToString();
        }

        else if (buttonName.ToLower() == "meridiem")
        {
            selectedMeridiemAM = !selectedMeridiemAM;
            meridiemText.text = selectedMeridiemAM ? "AM" : "PM";
        }

        CheckIfSelectedCorrect();
    }

    private void CheckIfSelectedCorrect()
    {
        int selectedMinutes = (selectedMin10 * 10) + selectedMin1;
        if (selectedHour == correctHour &&
            selectedMinutes == correctMinutes &&
            selectedMeridiemAM == correctMeridiemAM)
        {
            inputBlocker.SetActive(true);
            FindObjectOfType<MapObjectManager_S>()?.CmdM03_ClockCompleted();
        }
    }

    public void PlaySoundButtonClick()
    {
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M03_CalendarClockButtonClick);
    }
}
