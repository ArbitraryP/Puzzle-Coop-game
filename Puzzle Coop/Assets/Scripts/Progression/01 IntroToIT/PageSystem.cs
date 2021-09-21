using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] pages = null;
    [SerializeField] private int currentPage;
    [SerializeField] private bool isLoopable = false;
    [SerializeField] private Button buttonPrevious = null;
    [SerializeField] private Button buttonNext = null;


    private void Start()
    {
        ShowPage(currentPage);
    }

    private void CheckIfDisableButton()
    {
        buttonNext.interactable = true;
        buttonPrevious.interactable = true;

        if (isLoopable)
            return;

        if (currentPage == pages.Length - 1)
            buttonNext.interactable = false;

        else if (currentPage == 0)
            buttonPrevious.interactable = false;

    }

    public void ShowPage(int pageIndex)
    {
        if (pageIndex >= pages.Length || pageIndex < 0)
            return;

        foreach (GameObject page in pages)
            page.SetActive(false);

        pages[pageIndex].SetActive(true);

        currentPage = pageIndex;
        CheckIfDisableButton();
    }

    public void NextPage()
    {
        if (currentPage >= pages.Length - 1)
        {
            if (!isLoopable)
                return;

            ShowPage(0);
            return;
        }
        ShowPage(currentPage + 1);

    }

    public void PreviousPage()
    {
        if (currentPage == 0)
        {
            if (!isLoopable)
                return;

            ShowPage(pages.Length - 1);
            return;
        }
        ShowPage(currentPage - 1);

    }

   
}
