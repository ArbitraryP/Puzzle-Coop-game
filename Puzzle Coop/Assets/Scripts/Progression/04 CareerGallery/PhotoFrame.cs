using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Experimental.Rendering.Universal;

public class PhotoFrame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text[] textCode = null;
    [SerializeField] private Button[] buttonCodes = null;
    [SerializeField] private Image image = null;
    [SerializeField] private Light2D ticketLight = null;
    [SerializeField] private GameObject ticket = null;

    [Header("Data")]
    [SerializeField] private Career careerSolution = null;
    [SerializeField] private int[] code = new int[4];
    [SerializeField] private bool codeEnabled = false;
    


    public void ChangeCode(int index)
    {
        code[index]++;
        if (code[index] > 9)
            code[index] = 0;

        textCode[index].text = code[index].ToString();

        CheckSolution();
    }

    private void CheckSolution()
    {
        if (!codeEnabled || !careerSolution) return;
        for (int i = 0; i < 4; i++)
        {
            if (code[i] != careerSolution.code[i])
                return;  
        }

        ticket.SetActive(true);
        ticketLight.color = Color.green;
        // play ticket release correct sound 

        foreach (Button button in buttonCodes)
            button.interactable = false;
    }

}
