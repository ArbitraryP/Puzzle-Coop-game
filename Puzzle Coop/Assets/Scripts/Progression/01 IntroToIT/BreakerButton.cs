using UnityEngine;
using TMPro;

public class BreakerButton : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonText = null;
    [SerializeField] private int[] selectionSet;
    [SerializeField] private int correctValue = 0;
    private int currentSelectedIndex = 0;
    

    private void Start()
    {
        currentSelectedIndex = Random.Range(0, selectionSet.Length - 1);
        buttonText.text = selectionSet[currentSelectedIndex].ToString();
    }

    public void OnClickChangeValue()
    {
        currentSelectedIndex = currentSelectedIndex < (selectionSet.Length - 1) ? currentSelectedIndex + 1 : 0;
        buttonText.text = selectionSet[currentSelectedIndex].ToString();
    }

    public bool isSelectedCorrect()
    {
        return selectionSet[currentSelectedIndex] == correctValue;
        // To see correct answer go the MapObjectManager_L
    }

}
