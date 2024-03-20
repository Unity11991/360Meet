using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;


public class TextureScale
{
    public static void Bilinear(Texture2D tex, int width, int height)
    {
        Texture2D newTex = new Texture2D(width, height);
        float ratioX = 1.0f / ((float)width / (tex.width - 1));
        float ratioY = 1.0f / ((float)height / (tex.height - 1));
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float x = Mathf.Floor(j * ratioX);
                float y = Mathf.Floor(i * ratioY);
                Color bl = tex.GetPixel((int)x, (int)y);
                Color br = tex.GetPixel((int)x + 1, (int)y);
                Color tl = tex.GetPixel((int)x, (int)y + 1);
                Color tr = tex.GetPixel((int)x + 1, (int)y + 1);
                float xLerp = j * ratioX - x;
                Color b = Color.Lerp(bl, br, xLerp);
                Color t = Color.Lerp(tl, tr, xLerp);
                newTex.SetPixel(j, i, Color.Lerp(b, t, i * ratioY - y));
            }
        }
        newTex.Apply();
        tex.Reinitialize(width, height);
        tex.SetPixels(newTex.GetPixels());
        tex.Apply();
    }
}

public class TestVideoSen : MonoBehaviour
{
    public Material mat;
    private string flaskAppUrl = "http://localhost:5000/upload"; // Replace with your Flask app URL
    private WebCamTexture webcamTexture;

    void Start()
    {
        StartVideo();
    }

    void Update()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            // Send the texture data to the Flask app
            StartCoroutine(SendTextureToFlask(webcamTexture));
        }
    }

    private void StartVideo()
    {
        // Check if webcam is available
        if (WebCamTexture.devices.Length > 0)
        {
            // Get the first webcam device
            WebCamDevice webcamDevice = WebCamTexture.devices[0];
            // Create a WebCamTexture with the first device's name
            webcamTexture = new WebCamTexture(webcamDevice.name);
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

    IEnumerator SendTextureToFlask(WebCamTexture webcamTexture)
    {
        yield return new WaitForEndOfFrame();

        // Convert the texture to a byte array
        Texture2D tex = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
        tex.SetPixels(webcamTexture.GetPixels());
        tex.Apply();

        // Resize the texture to reduce its quality
        TextureScale.Bilinear(tex, tex.width / 2, tex.height / 2);

        byte[] textureBytes = tex.EncodeToPNG();

        // Create a Web Request
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(flaskAppUrl);
        webRequest.Method = "POST";
        webRequest.ContentType = "application/octet-stream";
        webRequest.ContentLength = textureBytes.Length;

        // Write the data to the request stream
        Stream requestStream = webRequest.GetRequestStream();
        requestStream.Write(textureBytes, 0, textureBytes.Length);
        requestStream.Close();

        // Get the response
        HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();

        Debug.Log("Response from Flask app: " + jsonResponse);
    }
}
