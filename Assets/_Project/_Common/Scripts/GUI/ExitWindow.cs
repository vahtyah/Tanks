using UnityEngine;
using UnityEngine.UI;

public class ExitWindow : MonoBehaviour
{
    [SerializeField] private Button openPanelButton;
    
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;
    
    private Animator anim;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        cancelButton.onClick.AddListener(OnCancelButtonClick);
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        openPanelButton.onClick.AddListener(OnOpenPanelButtonClick);
    }

    private void OnOpenPanelButtonClick()
    {
        anim.Play("In");
    }

    private void OnCancelButtonClick()
    {
        anim.Play("Out");
        StartCoroutine(DisableWindow());
    }

    private System.Collections.IEnumerator DisableWindow()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    private void OnConfirmButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    
    
}