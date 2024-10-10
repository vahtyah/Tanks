using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorButton : MonoBehaviour
{
    [SerializeField] private Scene.SceneName levelName;
    [SerializeField] private bool shutdownServer = false;

    private void Awake() { GetComponent<Button>().onClick.AddListener(OnLevelSelect); }

    private void OnLevelSelect()
    {
        if (shutdownServer)
        {
            NetworkManager.Singleton.Shutdown();
        }
        Scene.Load(levelName.ToString());
    }
}