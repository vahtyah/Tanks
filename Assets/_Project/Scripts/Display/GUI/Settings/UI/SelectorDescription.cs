using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectorDescription : MonoBehaviour, IEventListener<OptionSelectorEvent>
{
    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Initialize(Sprite preview, string title, string description)
    {
        if (preview == null)
            previewImage.gameObject.SetActive(false);
        else
            previewImage.sprite = preview;
        titleText.text = title;
        descriptionText.text = description;
    }

    public void OnEvent(OptionSelectorEvent e)
    {
        Initialize(e.Preview, e.SettingName, e.SettingDescription);   
    }

    private void OnEnable()
    {
        this.StartListening();
    }
    
    private void OnDisable()
    {
        this.StopListening();
    }
}