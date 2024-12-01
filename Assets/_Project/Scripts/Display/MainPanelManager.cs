using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PanelItem
{
    public string panelName;
    public GameObject panelObject;
    public Button buttonObject;
}

public class MainPanelManager : MonoBehaviour
{
    [SerializeField] private List<PanelItem> panels = new();
    [SerializeField, Range(0, 3)] private int defaultPanelIndex = 0;
    
    private PanelItem currentPanel;
    private int currentPanelIndex;

    string panelInRight = "InRight";
    string panelOutRight = "OutRight";
    string panelInLeft = "InLeft";
    string panelOutLeft = "OutLeft";

    string buttonFadeIn = "Normal To Pressed";
    string buttonFadeOut = "Pressed To Dissolve";
    // string buttonFadeNormal = "Pressed To Normal";

    void OnEnable()
    {
        ChangePanel(defaultPanelIndex);
    }

    public void ChangePanel(int index)
    {
        if (index < 0 || index >= panels.Count || panels[index] == currentPanel)
            return;

        if (currentPanel != null)
        {
            PlayAnimation(currentPanel, currentPanelIndex < index ? panelOutRight : panelOutLeft, buttonFadeOut);
            StartCoroutine(DisablePreviousPanel(currentPanel));
        }

        PlayAnimation(panels[index], currentPanelIndex <= index ? panelInLeft : panelInRight, buttonFadeIn);
        currentPanel = panels[index];
        currentPanelIndex = index;
        GUIMainMenuManager.Instance.HandleHomePanel();
    }

    private void PlayAnimation(PanelItem panelItem, string panelAnimation, string buttonAnimation)
    {
        panelItem.panelObject.SetActive(true);
        panelItem.panelObject.GetComponent<Animator>().Play(panelAnimation);
        panelItem.buttonObject.GetComponent<Animator>().Play(buttonAnimation);
    }

    IEnumerator DisablePreviousPanel(PanelItem panel)
    {
        yield return new WaitForSeconds(.5f);
        if (panel == currentPanel)
            yield break;
        panel.panelObject.SetActive(false);
    }

    public PanelItem GetCurrentPanelName()
    {
        return currentPanel;
    }
}