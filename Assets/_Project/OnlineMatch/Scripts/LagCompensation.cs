using Photon.Pun;
using UnityEngine;

public class LagCompensation : MonoBehaviour, IPunObservable
{
    Rigidbody rb;
    PhotonView photonView;
    private Vector3 networkPosition;
    private Vector3 networkVelocity;
    private Quaternion networkRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.velocity);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            networkPosition += rb.velocity * lag;
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rb.position = Vector3.MoveTowards(rb.position, networkPosition, Time.fixedDeltaTime);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
        }
    }
}