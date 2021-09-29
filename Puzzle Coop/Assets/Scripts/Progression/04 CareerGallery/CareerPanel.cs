using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CareerPanel : MonoBehaviour
{
    [SerializeField] private Image imageCareer = null;
    [SerializeField] private TMP_Text textName = null;
    [SerializeField] private TMP_Text[] textCodes = null;

    [Header("Assigned Career")]
    [SerializeField] private Career careerObject = null;


    public void SetupCareerPanel(Career career)
    {
        careerObject = career;

        imageCareer.sprite = careerObject.image;
        

        SetShowCode(true);
    }

    public void SetShowCode(bool state)
    {
        if (!state)
        {
            textName.text = "...";
            foreach (TMP_Text textCode in textCodes)
                textCode.text = "";

            return;
        }

        if (!careerObject) return;

        for (int i = 0; i < textCodes.Length; i++)
        {
            textName.text = careerObject.careerName;
            if (i >= careerObject.code.Length)
                break;

            textCodes[i].text = careerObject.code[i].ToString();
        }

    }

}
