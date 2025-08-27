using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator menuAnimator;

    [Header("Menu Pages")]
    [SerializeField] private GameObject[] menuPages; // assign Shop, Settings, Upgrades, etc.

    private int currentPage = -1;   // -1 = no page open
    private bool isOpen = false;    // tracks if the menu is open

   
    public void TogglePage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= menuPages.Length)
        {
            Debug.LogWarning("Invalid page index!");
            return;
        }

        if (!isOpen)
        {
            // Menu closed → open it
            OpenMenu(pageIndex);
        }
        else
        {
            if (currentPage == pageIndex)
            {
                // Same page pressed again → close menu
                CloseMenu();
            }
            else
            {
                // Different page → switch pages (no need to close)
                ShowPage(pageIndex);
            }
        }
    }

    private void OpenMenu(int pageIndex)
    {
        menuAnimator.SetTrigger("Open");
        ShowPage(pageIndex);
        isOpen = true;
    }

    private void CloseMenu()
    {
        menuAnimator.SetTrigger("Close");
       
        isOpen = false;
    }

    private void ShowPage(int pageIndex)
    {
        HideAllPages();
        menuPages[pageIndex].SetActive(true);
        currentPage = pageIndex;
    }

    private void HideAllPages()
    {
        foreach (var page in menuPages)
        {
            if (page != null)
                page.SetActive(false);
        }
    }
}
