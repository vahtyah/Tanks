using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPlayLocal : MonoBehaviour
{
    private Button _button;
    [SerializeField] private GameObject selectAmountPanel;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    
    private void Start()
    {
        _button.onClick.AddListener((() => selectAmountPanel.SetActive(true)));
    }
}
