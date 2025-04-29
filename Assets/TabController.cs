using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActivateTab(0);
    }

    // Activate tab
    public void ActivateTab(int tabNo)
    {
        // Deactivate all tabs
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey;
        }
        // Activate selected tab
        pages[tabNo].SetActive(true);
        tabImages[tabNo].color = Color.white;
    }
}
