using UnityEngine;
using Photon.Pun;

public class SyncMaterial : MonoBehaviourPunCallbacks, IPunObservable
{
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // Update material properties on local client
            // For example, change material color based on user input
            rend.material.color = Color.Lerp(Color.red, Color.blue, Mathf.PingPong(Time.time, 1));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send material color over the network
            stream.SendNext(rend.material.color);
        }
        else
        {
            // Receive material color from the network and update material
            rend.material.color = (Color)stream.ReceiveNext();
        }
    }
}

