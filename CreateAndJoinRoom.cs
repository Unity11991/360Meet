using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{

    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    // Start is called before the first frame update
    
    public void CreateRoom()
    {
        //PlayerPrefs.SetString("UserName", createInput.text);
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        //PlayerPrefs.SetString("UserName", joinInput.text);
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}

