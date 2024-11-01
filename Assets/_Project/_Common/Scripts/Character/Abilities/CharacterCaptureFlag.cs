using Photon.Pun;
using UnityEngine;

public class CharacterCaptureFlag : CharacterAbility
{
    [SerializeField] private GameObject flagInTank;
    [SerializeField] private Renderer rd;
    [SerializeField] private Flag flagCaptured;
    [SerializeField] private PhotonView photonView;
    
       
    protected override void Initialize()
    {
        Controller.AddOnTriggerEnter(TriggerEnter);
        photonView = GetComponent<PhotonView>();
    }

    private void TriggerEnter(Collider other)
    {
        if (other.CompareTag("Flag"))
        {
            var flag = other.GetComponent<Flag>();
            if (flag.Team != Character.Team)
            {
                flagCaptured = flag;
                Debug.Log(PhotonView);
                photonView.RPC(nameof(CaptureFlag), RpcTarget.All);
            }
        }
    }
    
    [PunRPC]
    private void CaptureFlag()
    {
        if (flagCaptured == null)
        {
            Debug.LogError("Flag is null");
            return;
        }
        flagCaptured.Capture(Character.Team);
        flagInTank.SetActive(true);
        rd.material = flagCaptured.Rd.material;
    }
}