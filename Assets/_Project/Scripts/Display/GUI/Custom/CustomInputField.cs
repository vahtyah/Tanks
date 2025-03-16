using System;
using TMPro;
using UnityEngine;

public class CustomInputField : MonoBehaviour
{
    [SerializeField] private string Name;
    [SerializeField] private TextMeshProUGUI placeholderText;
    
    private PlaceHolder placeholder;
    private TMP_InputField inputField;
    private Animator anim;
    
    private string inAim = "In";
    private string outAim = "Out";

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        anim = GetComponent<Animator>();
        placeholder = GetComponentInChildren<PlaceHolder>();
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
        if (inputField.text.Length != 0) return;
        anim.Play(inAim);
    }

    private void OnValidate()
    {
        placeholderText.SetText(Name);
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
    
    public void Error()
    {
        placeholder?.Error();
    }
}
