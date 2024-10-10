using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : Character
{
    [SerializeField] private string playerID = "Player1";
    public GameObject CameraTarget;

    public HealthNetwork HealthNetwork;

    public string PlayerID => playerID;

    protected override void Awake()
    {
        base.Awake();
        HealthNetwork = GetComponent<HealthNetwork>();
        if(NetworkManager == null) return;
        NetworkManager.OnClientDisconnectCallback += b =>
        {
            if (b == OwnerClientId)
            {
                if (!NetworkManager.Singleton.IsServer)
                    NotifyHostDisconnected();
            }
        };
    }

    private void Update() { ProcessAbilities(); }

    private void FixedUpdate() { FixedProcessAbilities(); }

    protected override void ProcessAbilities()
    {
        base.ProcessAbilities();
        if (!IsOwner && NetworkManager != null) return;
        foreach (var ability in characterAbilities.Where(ability => ability.enabled))
        {
            ability.ProcessAbility();
        }
    }

    protected override void FixedProcessAbilities()
    {
        base.FixedProcessAbilities();
        if (!IsOwner && NetworkManager != null) return;
        foreach (var ability in characterAbilities.Where(ability => ability.enabled))
        {
            ability.FixedProcessAbility();
        }
    }

    [ClientRpc]
    public void NotifyWinClientRpc()
    {
        if (!IsOwner) return;
        StartCoroutine(IENotifyWin());
    }

    private IEnumerator IENotifyWin()
    {
        yield return new WaitForSecondsRealtime(1);
        GUIManagerNetcode.Instance.SetWinnerPanel(true);
        LevelManagerNetCode.Instance.isGameOver = true;
    }

    private void NotifyHostDisconnected()
    {
        GUIManagerNetcode.Instance.SetDiePanel(false);
        GUIManagerNetcode.Instance.SetWinnerPanel(false);
        GUIManagerNetcode.Instance.SetHostDisconnectedPanel(true);
        LevelManagerNetCode.Instance.isGameOver = true;
    }
}