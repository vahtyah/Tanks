using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PanelItem
{
    public GameObject panelObject;
    public Button buttonObject;
    public NavigationType navigationType;
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

    private void Awake()
    {
        foreach (var panel in panels)
        {
            panel.panelObject.SetActive(false);
            panel.buttonObject.onClick.AddListener(() => ChangePanel(panels.IndexOf(panel)));
        }
    }

    void OnEnable()
    {
        ActivateNewPanel(defaultPanelIndex);
    }

    public void ChangePanel(int index)
    {
        if (index < 0 || index >= panels.Count || panels[index] == currentPanel)
            return;

        if (currentPanel != null)
        {
            DeactivateCurrentPanel(index);
        }

        ActivateNewPanel(index);
    }

    private void DeactivateCurrentPanel(int index)
    {
        PlayAnimation(currentPanel, currentPanelIndex < index ? panelOutRight : panelOutLeft, buttonFadeOut);
        StartCoroutine(DisablePreviousPanel(currentPanel));
    }

    private void ActivateNewPanel(int index)
    {
        PlayAnimation(panels[index], currentPanelIndex <= index ? panelInLeft : panelInRight, buttonFadeIn);
        currentPanel = panels[index];
        currentPanelIndex = index;
        NavigationEvent.Trigger(currentPanel.navigationType);
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