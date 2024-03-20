using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class PunSystem : MonoBehaviour
{
    public PhotonView M_Photonview; //That object we want to use this script, must be have photon view component
    public GameViewDecoder GameViewDecoder; //The component of video decoder for convert the bytes to screen
    public string YourName; //The name of you !!!
    public GameObject Track;
    public Toggle WebCamToggle;
    private void Start()
    {
        YourName = PlayerPrefs.GetString("UserName");
    }
    public void WebCam()
    {
        if (WebCamToggle.isOn == true)
        {
            Track.SetActive(false);
        }
        if (WebCamToggle.isOn == false)
        {
            Track.SetActive(true);
        }
    }
    public void SendMessage(byte[] _byteData, string message/*, string username*/)
    {
        M_Photonview.RPC("RPC_SendMessage", RpcTarget.All, _byteData, message);
    }
    [PunRPC]
    private void RPC_SendMessage(byte[] _byteData, string message/*, string username*/)
    {
        //if (YourName != username)
        //{
            if (message.Contains("VideoShare"))
            {
                GameViewDecoder.Action_ProcessImageData(_byteData);
            }
        //}
    }
}