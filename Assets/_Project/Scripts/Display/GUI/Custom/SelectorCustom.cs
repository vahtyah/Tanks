using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectorCustom : MonoBehaviour
{
    public int[] options;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI supportText;

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [SerializeField] private bool isLoop;
    
    private int currentIndex;
    private int previousIndex;
    private Animator anim;
    private string inAim = "In";
    private string outAim = "Out";
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        leftButton.onClick.AddListener(OnLeftClick);
        rightButton.onClick.AddListener(OnRightClick);
        currentIndex = 0;
        text.text = options[currentIndex].ToString();
        supportText.text = options[currentIndex].ToString();
    }

    private void OnRightClick()
    {
        previousIndex = currentIndex;
        currentIndex++;
        if (currentIndex >= options.Length)
            currentIndex = isLoop ? 0 : options.Length - 1;
        if (previousIndex != currentIndex)
        {
            text.text = options[currentIndex].ToString();
            supportText.text = options[previousIndex].ToString();
            anim.Play(inAim,0,0f);
        }
    }

    private void OnLeftClick()
    {
        previousIndex = currentIndex;
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = isLoop ? options.Length - 1 : 0;
        if (previousIndex != currentIndex)
        {
            text.text = options[currentIndex].ToString();
            supportText.text = options[previousIndex].ToString();
            anim.Play(outAim,0,0f);
        }
    }
    
    public int GetOption()
    {
        return options[currentIndex];
    }
    
    public int[] GetOptions()
    {
        return options;
    }

    public void SetOption(int index)
    {
        currentIndex = index;
        text.text = options[currentIndex].ToString();
        supportText.text = options[currentIndex].ToString();
    }

}
