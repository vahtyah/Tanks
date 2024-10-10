using TMPro;
using Unity.Netcode;

public class PlayerLength : NetworkBehaviour
{
    private  TextMeshProUGUI text;
    
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    
    private void Update()
    {
        if (!IsOwner)
            return;
        
        SetLengthServerRpc();
    }
    
    [ServerRpc]
    private void SetLengthServerRpc()
    {
        text.text = $"Length: {NetworkManager.Singleton.ConnectedClients.Count}";
    }
}