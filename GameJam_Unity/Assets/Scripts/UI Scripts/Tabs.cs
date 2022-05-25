using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tabs : MonoBehaviour
{
    public List<TabsButton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public TabsButton selectedTab;
    public List<GameObject> objToSwap;


    public void Subscribe(TabsButton button)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabsButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(TabsButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
        button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(TabsButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabsButton button)
    {
        if (selectedTab != null)
        {
            selectedTab.DeSelect();
        }

        selectedTab = button;

        selectedTab.Select();

        ResetTabs();
        button.background.sprite = tabActive;
        int index = button.transform.GetSiblingIndex();
        for(int i=0; i < objToSwap.Count; i++)
        {
            if(i == index)
            {
                objToSwap[i].SetActive(true);
            }
            else
            {
                objToSwap[i].SetActive(true);
            }
        }
    }

    public void ResetTabs()
    {
        foreach(TabsButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) { continue; }
            button.background.sprite = tabIdle;
        }
    }
}
