using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour
{
    [SerializeField] private string Name;
    [SerializeField] private TextMeshProUGUI label;
    
    private Animator anim;
    private Toggle toggle;
    
    private string inAim = "In";
    private string outAim = "Out";
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        toggle = GetComponent<Toggle>();
        
        toggle.onValueChanged.AddListener(OnToggle);
    }

    private void OnToggle(bool isOn)
    {
        anim.Play(isOn ? inAim : outAim);
    }
    
    private void OnValidate()
    {
        label.text = Name;
        gameObject.name = Name + " Toggle";
    }
}
