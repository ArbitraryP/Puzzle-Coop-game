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
    [SerializeField] private Light2D ticket2DLight = null;
    [SerializeField] private GameObject ticket = null;

    [Header("Data")]
    [SerializeField] private Career careerSolution = null;
    [SerializeField] private CareerSet careerSet = null;
    [SerializeField] private int[] code = new int[4];
    public bool codeEnabled = false;

    private void Start()
    {
        //NotSync to Network: No need. and Does not detect correct solution at the start: RNG safe.

        for (int i = 0; i < code.Length; i++)
        {
            code[i] = Random.Range(0, 10);
            textCode[i].text = code[i].ToString();
        }
    }

    public void SetCareerSolution(int index)
    {
        careerSolution = careerSet.Careers[index];
        image.sprite = careerSolution.image;
    }

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
        ticket2DLight.color = Color.green;
        // play ticket release correct sound 

        foreach (Button button in buttonCodes)
            button.interactable = false;
    }

}
