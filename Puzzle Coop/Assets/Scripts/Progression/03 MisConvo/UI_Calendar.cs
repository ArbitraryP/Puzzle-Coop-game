using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Experimental.Rendering.Universal;

public class UI_Calendar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button monthButton = null;
    [SerializeField] private TMP_Text monthText = null;
    [SerializeField] private Button[] dateButtons;
    [SerializeField] private GameObject inputBlocker = null;
    public Light2D calendarLight = null;
    public GameObject iconButton = null;

    [Header("Data")]
    [SerializeField] private string[] months;
    [Range(1,12)]
    [SerializeField] private int selectedMonth = 0;
    [Range(1,31)]
    [SerializeField] private int selectedDate = 1;

    [Header("Solution (Do not Edit here)")]
    [SerializeField] private int correctMonth;
    [SerializeField] private int correctDate;

    private void Start()
    {
        // Generate Random Starting value (may start with correct value)

        selectedMonth = Random.Range(1,12);
        monthText.text = months[selectedMonth - 1];

        selectedDate = Random.Range(1, 31);
        foreach (Button button in dateButtons)
            button.interactable = true;

        dateButtons[selectedDate - 1].interactable = false;
    }

    public void SetSolution(int month, int date)
    {
        correctMonth = month;
        correctDate = date;
    }

    private void CheckIfSelectedCorrect()
    {
        if(selectedMonth == correctMonth &&
            selectedDate == correctDate)
        {
            calendarLight.color = Color.green;
            inputBlocker.SetActive(true);
            FindObjectOfType<MapObjectManager_S>()?.CmdM03_CalendarCompleted();
            
        }
    }

    public void OnClickMonthButton()
    {
        selectedMonth = selectedMonth >= 12 ? 1 : selectedMonth + 1;
        monthText.text = months[selectedMonth-1];
        CheckIfSelectedCorrect();
    }

    public void OnClickDateButton(int date)
    {
        foreach(Button button in dateButtons)
            button.interactable = true;

        dateButtons[date - 1].interactable = false;
        selectedDate = date;
        CheckIfSelectedCorrect();
    }

}
