using UnityEngine;

public class TestWebcamRenderer : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    private Texture2D textureToSend;
    private byte[] textureBytes;

    [SerializeField] private Material targetMaterial;

    void Start()
    {
        // Start the webcam with a lower resolution
        webcamTexture = new WebCamTexture(320, 240);
        webcamTexture.Play();

        // Initialize the texture to send
        textureToSend = new Texture2D(webcamTexture.width, webcamTexture.height);
    }

    void Update()
    {
        // Update the texture to send with the current webcam frame
        textureToSend.SetPixels(webcamTexture.GetPixels());
        textureToSend.Apply();

        // Convert the texture to a byte array (compressed as JPG)
        textureBytes = textureToSend.EncodeToJPG();

        // Load the byte array into the texture
        Texture2D receivedTexture = new Texture2D(2, 2);
        receivedTexture.LoadImage(textureBytes);

        // Apply the texture to the target material
        targetMaterial.mainTexture = receivedTexture;

    }
}
