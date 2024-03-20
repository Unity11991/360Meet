using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VideoCall : MonoBehaviour
{
    public Material mat;
    void Start()
    {
        //Invoke("StartVideoServerRpc", 5f);
        StartVideo();
        //PrintAvailableCameras();
    }


    //[ServerRpc]
    private void StartVideo()
    {
        // Check if webcam is available
        if (WebCamTexture.devices.Length > 0)
        {
            // Get the first webcam device
            WebCamDevice webcamDevice = WebCamTexture.devices[0];
            // Create a WebCamTexture with the first device's name
            WebCamTexture webcamTexture = new WebCamTexture(webcamDevice.name);
            // Assign the webcam texture to a material
            mat = GetComponent<Renderer>().material;
            mat.mainTexture = webcamTexture;
            // Start the webcam feed
            webcamTexture.Play();
        }
        else
        {
            Debug.LogError("No webcam found!");
        }
        //StartVideoClientRpc();
    }

    private void PrintAvailableCameras()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            Debug.Log("Available cameras:");
            for (int i = 0; i < devices.Length; i++)
            {
                Debug.Log("Camera " + i + ": " + devices[i].name);
            }
        }
        else
        {
            Debug.LogError("No cameras found!");
        }
    }

    [ClientRpc]
    private void StartVideoClientRpc()
    {
        // Check if webcam is available
        if (WebCamTexture.devices.Length > 0)
        {
            // Get the first webcam device
            WebCamDevice webcamDevice = WebCamTexture.devices[0];
            // Create a WebCamTexture with the first device's name
            WebCamTexture webcamTexture = new WebCamTexture(webcamDevice.name);
            // Assign the webcam texture to a material
            mat = GetComponent<Renderer>().material;
            mat.mainTexture = webcamTexture;
            // Start the webcam feed
            webcamTexture.Play();
        }
        else
        {
            Debug.LogError("No webcam found!");
        }
    }
    //public string videoURL = "https://www.example.com/video.mp4";
    //public VideoPlayer VideoPlayer;

    //void Start()
    //{


    //    // Set the video URL
    //    VideoPlayer.url = videoURL;

    //    // Play the video
    //    VideoPlayer.Play();
    //}
}
