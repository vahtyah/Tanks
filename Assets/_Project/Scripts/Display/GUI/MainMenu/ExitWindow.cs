using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExitWindow : MonoBehaviour
{
    [SerializeField] private Button openPanelButton;
    
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button confirmButton;
    
    private Animator anim;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        cancelButton.onClick.AddListener(OnCancelButtonClick);
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        openPanelButton.onClick.AddListener(OnOpenPanelButtonClick);
        logoutButton.onClick.AddListener(OnLogoutButtonClick);
    }

    private void OnLogoutButtonClick()
    {
        anim.Play("Out");
        StartCoroutine(DisableWindow());
        PlayerPrefs.SetInt(GlobalString.REMEMBER_ME, 0);
        GameEvent.Trigger(GameEventType.GameAuthentication);
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

    private IEnumerator DisableWindow()
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