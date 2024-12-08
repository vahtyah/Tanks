using System;
using TMPro;
using UnityEngine;

public class CustomInputField : MonoBehaviour
{
    [SerializeField] private string Name;
    [SerializeField] private TextMeshProUGUI placeholder;
    
    private TMP_InputField inputField;
    private Animator anim;
    
    private string inAim = "In";
    private string outAim = "Out";

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        anim = GetComponent<Animator>();
        
        inputField.onSelect.AddListener(OnSelect);
        inputField.onDeselect.AddListener(OnDeselect);
    }

    private void OnDeselect(string arg0)
    {
        if(inputField.text.Length == 0)
            anim.Play(outAim);
    }

    private void OnSelect(string arg0)
    {
        anim.Play(inAim);
    }

    private void OnValidate()
    {
        placeholder.text = Name;
        gameObject.name = Name + " Input Field";
    }

    public string Text
    {
        get => inputField.text;
        set
        {
            inputField.text = value;
            if(inputField.text.Length > 0)
                anim.Play(inAim);
        }
    }
}
