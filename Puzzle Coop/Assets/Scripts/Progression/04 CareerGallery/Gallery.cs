using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Gallery : MonoBehaviour
{
    [SerializeField] private CareerSet careerSet = null;

    [SerializeField] private ScrollRect panelScroll = null;
    [SerializeField] private Transform panelContent = null;
    [SerializeField] private CareerPanel careerPanelPrefab = null;
    [SerializeField] private List<CareerPanel> careerPanels = null;


    private bool showCode = true;
    public bool ShowCode
    {
        get => showCode;
        set
        {
            showCode = value;
            panelScroll.enabled = value;
            foreach (CareerPanel careerPanel in careerPanels)
                careerPanel.SetShowCode(value);
        }
    }


    private void Start()
    {
        List<Career> shuffledCareers = new List<Career>();
        shuffledCareers.AddRange(careerSet.Careers);
        
        while (shuffledCareers.Count != 0)
        {
            int randomIndex = Random.Range(0, shuffledCareers.Count);

            var newCareer = Instantiate(careerPanelPrefab, panelContent);
            careerPanels.Add(newCareer);

            newCareer.transform.localScale = new Vector3(1,1,1);
            newCareer.SetupCareerPanel(shuffledCareers[randomIndex]);
            shuffledCareers.RemoveAt(randomIndex);
        }


    }



}
