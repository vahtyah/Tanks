using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class Locatable : LocatableComponent
{
    [SerializeField] protected LocatableIconComponent iconPrefab;
    [SerializeField] protected LocatableIconComponent iconPlayerPrefab;
    
    [SerializeField] private bool clampOnRadar = false;
    
    private PhotonView photonView;
    
    protected virtual void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public override bool ClampOnRadar
    {
        get => clampOnRadar;
        set => clampOnRadar = value;
    }
    [ShowInInspector]
    public override bool Detected { get; set; }
    
    public bool YourTeamDetected;
    
    public override void SetDetectedForTeam(Player player, bool detected)
    {
        if(YourTeamDetected) return;
        var playerOther = player.GetOtherPlayersInTeam();
        foreach (var p in playerOther)
        {
            photonView.RPC(nameof(RPC_SetDetected), p, detected);
        }
        Detected = detected;
    }
    
    [PunRPC]
    void RPC_SetDetected(bool detected)
    {
        YourTeamDetected = detected;
        Detected = detected;
    }

    public override LocatableIconComponent CreateIcon(Color color, bool isPlayer = false)
    {
        var instance = Instantiate(isPlayer ? iconPlayerPrefab : iconPrefab);
        instance.ChangeColor(isPlayer ? Color.white : color);
        return instance;
    }

}