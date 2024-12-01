using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphicSettingsPanel : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private MultiOptionSelector multiOptionSelectorPrefab;
    [SerializeField] private Button applyButton;
    private GraphicSettingsManager graphicSettingsManager;
    private Dictionary<MultiOptionSelector, IMultiOptionGraphicSetting> selectors = new();
    
    private void Awake()
    {
        graphicSettingsManager = FindObjectOfType<GraphicSettingsManager>();
        applyButton.onClick.AddListener(() =>
        {
            graphicSettingsManager.ApplySettings();
            graphicSettingsManager.SaveSettings();
        });
    }
    
    private void Start()
    {
        var settings = graphicSettingsManager.GetSettings();
        
        for (var i = 0; i < settings.Count; i++)
        {
            if (settings[i] is IMultiOptionGraphicSetting multiOptionSettings && settings[i].settingType == SettingType.Graphic)
            {
                var multiOptionSelector = Instantiate(multiOptionSelectorPrefab, container);
                multiOptionSelector.Initialize(settings[i].GetSettingName(), multiOptionSettings.GetOptionNames(), multiOptionSettings.GetIndex(), multiOptionSettings.SetIndex);  
                selectors.Add(multiOptionSelector, multiOptionSettings);
            }
        }
        // if (selectors.Count > 0)
        // {
        //     EventSystem.current.SetSelectedGameObject(selectors.Keys.First().gameObject);
        // }
    }
    
    public void ResetSettings()
    {
        SettingsManager.Instance.LoadSettings();
        foreach (var selector in selectors)
        {
            selector.Key.SetIndex(selector.Value.GetIndex());
        }
    }
}
