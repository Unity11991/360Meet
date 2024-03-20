using UnityEngine;
using System.Collections;
using Photon.Pun;
using System;

public class WebcamController : MonoBehaviourPunCallbacks, IPunObservable
{
    private WebCamTexture webcamTexture;
    private Texture2D textureToSend;
    private byte[] textureBytes;
    private float rotationSpeed = 5f; // Speed of rotation
    private float moveSpeed = 3f; // Speed of horizontal movement
    private float timeSinceLastUpdate;
    private Vector3 initialCameraPosition;


    void Start()
    {

        // Store the initial camera position
        initialCameraPosition = Camera.main.transform.position;
        // Start the webcam
        //webcamTexture = new WebCamTexture();
        if (photonView.IsMine)
        {
            // Start the webcam with a lower resolution
            //webcamTexture = new WebCamTexture(320, 240);
            webcamTexture = new WebCamTexture();

            webcamTexture.Play();

            // Initialize the texture to send
            textureToSend = new Texture2D(webcamTexture.width, webcamTexture.height);
        }

    }

    void Update()
    {
        // If this is our local player
        if (photonView.IsMine)
        {
            // Only send a new texture every 0.5 seconds
            if (timeSinceLastUpdate > 0.1f)
            {
                // Update the texture to send with the current webcam frame
                textureToSend.SetPixels(webcamTexture.GetPixels());
                textureToSend.Apply();

                // Convert the texture to a byte array (compressed as JPG)
                textureBytes = textureToSend.EncodeToJPG();

                // Send the byte array over the network
                photonView.RPC("SyncWebcamTextureRPC", RpcTarget.OthersBuffered, textureBytes);

                // Create a new Texture2D to hold the received texture
                Texture2D receivedTexture = new Texture2D(2, 2);

                // Load the byte array into the texture
                receivedTexture.LoadImage(textureBytes);

                // Apply the texture to a material (for example)
                GetComponent<Renderer>().material.mainTexture = receivedTexture;
                // Clear the byte array
                textureBytes = null;

                // Reset the timer
                timeSinceLastUpdate = 0f;
            }
            // Increment the timer
            timeSinceLastUpdate += Time.deltaTime;
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Move the sphere horizontally based on button input
            float horizontalInput = Input.GetAxis("Horizontal");
            Vector3 movement = new Vector3(horizontalInput, 0f, 0f) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);


            if (Camera.main.transform.position == initialCameraPosition)
            {
                // Only allow rotation around Y axis when the camera is at initial position
                Camera.main.transform.Rotate(Vector3.up, mouseX * rotationSpeed);
            }
            else
            {
                // Allow rotation around both X and Y axes
                transform.Rotate(Vector3.up, mouseX * rotationSpeed);
                transform.Rotate(transform.right, -mouseY * rotationSpeed);
            }

            // Check for mouse click
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    // If the clicked object is a player, move the camera to its center
                    if (hit.transform.GetComponent<WebcamController>() != null)
                    {
                        Camera.main.transform.position = hit.transform.position + new Vector3(0f, 0f, -10f);
                    }
                }
            }

            if(Input.GetMouseButtonDown(1))
            {
                Camera.main.transform.position = initialCameraPosition;
            }

        }
    }

    [PunRPC]
    void SyncWebcamTextureRPC(byte[] newTextureBytes)
    {
        // Create a new Texture2D to hold the received texture
        Texture2D receivedTexture = new Texture2D(2, 2);

        // Load the byte array into the texture
        receivedTexture.LoadImage(newTextureBytes);

        // Apply the texture to a material (for example)
        GetComponent<Renderer>().material.mainTexture = receivedTexture;

        // Clear the byte array
        newTextureBytes = null;
    }

    public void MoveCameraToCenter()
    {
        // Move the scene camera to the center of the game object where this script is attached
        Camera.main.transform.position = transform.position + new Vector3(0f, 0f, -10f); // Adjust the z position according to your scene setup
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Nothing to do here in this example
    }
}
