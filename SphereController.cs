using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections;

public class SphereController : MonoBehaviour, IPunObservable
{
    public float rotationSpeed = 5f; // Speed of rotation
    public float moveSpeed = 3f; // Speed of horizontal movement
    Renderer rend;
    PhotonView view;
    private Color PlayerColors;


    void Awake()
    {
        PhotonPeer.RegisterType(typeof(Color), (byte)'C', SerializeColor, DeserializeColor);
    }
    private void Start()
    {
        view = GetComponent<PhotonView>();
        rend = GetComponent<Renderer>();
    }
    void Update()
    {
        if (view.IsMine)
        {
            // Rotate the sphere based on mouse movement
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseX * rotationSpeed);

            // Move the sphere horizontally based on button input
            float horizontalInput = Input.GetAxis("Horizontal");
            Vector3 movement = new Vector3(horizontalInput, 0f, 0f) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);

            // Start the ChangeColor coroutine
            StartCoroutine(ChangeColor());
        }
        else
        {
            // Apply the synchronized color received from the network
            rend.material.color = PlayerColors;
        }
    }

    IEnumerator ChangeColor()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Change the color
        rend.material.color = Color.Lerp(Color.red, Color.blue, Mathf.PingPong(Time.time, 1));

        // Send the color change over the network
        if (PhotonNetwork.IsConnected)
        {
            view.RPC("SyncColorRPC", RpcTarget.OthersBuffered, rend.material.color);
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Convert the Color to Vector4 since Vector4 is serializable
            Vector4 colorAsVector = new Vector4(PlayerColors.r, PlayerColors.g, PlayerColors.b, PlayerColors.a);
            stream.SendNext(colorAsVector);
        }
        else
        {
            // Receive the Vector4 from the network and convert it back to Color
            Vector4 colorAsVector = (Vector4)stream.ReceiveNext();
            PlayerColors = new Color(colorAsVector.x, colorAsVector.y, colorAsVector.z, colorAsVector.w);
        }
    }

    // RPC method to synchronize color across network
    [PunRPC]
    void SyncColorRPC(Color newColor)
    {
        PlayerColors = newColor;
    }


    // Method to convert Color to byte[]
    private static byte[] SerializeColor(object customobject)
    {
        Color color = (Color)customobject;
        byte[] colorBytes = new byte[4];
        colorBytes[0] = (byte)(color.r * 255);
        colorBytes[1] = (byte)(color.g * 255);
        colorBytes[2] = (byte)(color.b * 255);
        colorBytes[3] = (byte)(color.a * 255);
        return colorBytes;
    }

    // Method to convert byte[] to Color
    private static object DeserializeColor(byte[] bytes)
    {
        Color color = new Color(bytes[0] / 255f, bytes[1] / 255f, bytes[2] / 255f, bytes[3] / 255f);
        return color;
    }

}


//using UnityEngine;
//using Photon.Pun;
//using ExitGames.Client.Photon;
//using System.Collections;

//public class SphereController : MonoBehaviourPunCallbacks, IPunObservable
//{
//    public float rotationSpeed = 5f; // Speed of rotation
//    public float moveSpeed = 3f; // Speed of horizontal movement
//    Renderer rend;
//    PhotonView view;
//    WebCamTexture webcamTexture;

//    private void Start()
//    {
//        view = GetComponent<PhotonView>();
//        rend = GetComponent<Renderer>();

//        // Check if webcam is available
//        if (WebCamTexture.devices.Length > 0)
//        {
//            // Get the first webcam device
//            WebCamDevice webcamDevice = WebCamTexture.devices[0];
//            // Create a WebCamTexture with the first device's name
//            webcamTexture = new WebCamTexture(webcamDevice.name);
//            // Start the webcam feed
//            webcamTexture.Play();
//        }
//        else
//        {
//            Debug.LogError("No webcam found!");
//        }
//    }

//    void Update()
//    {
//        if (view.IsMine)
//        {
//            // Rotate the sphere based on mouse movement
//            float mouseX = Input.GetAxis("Mouse X");
//            transform.Rotate(Vector3.up, mouseX * rotationSpeed);

//            // Move the sphere horizontally based on button input
//            float horizontalInput = Input.GetAxis("Horizontal");
//            Vector3 movement = new Vector3(horizontalInput, 0f, 0f) * moveSpeed * Time.deltaTime;
//            transform.Translate(movement);

//            // Start the ChangeTexture coroutine
//            StartCoroutine(ChangeTexture());
//        }
//    }

//    IEnumerator ChangeTexture()
//    {
//        // Wait for 1 second
//        yield return new WaitForSeconds(1f);

//        // Convert the WebCamTexture to a Texture2D
//        Texture2D texture2D = new Texture2D(webcamTexture.width, webcamTexture.height);
//        texture2D.SetPixels(webcamTexture.GetPixels());
//        texture2D.Apply();

//        // Convert the Texture2D to a JPG
//        byte[] textureBytes = texture2D.EncodeToJPG();

//        // Send the texture bytes over the network
//        if (PhotonNetwork.IsConnected)
//        {
//            view.RPC("SyncTextureRPC", RpcTarget.OthersBuffered, textureBytes);
//        }
//    }

//    // RPC method to synchronize texture across network
//    [PunRPC]
//    void SyncTextureRPC(byte[] textureBytes)
//    {
//        // Convert the byte array back to a Texture2D
//        Texture2D texture = new Texture2D(2, 2);
//        texture.LoadImage(textureBytes);

//        // Apply the texture
//        rend.material.mainTexture = texture;
//    }

//    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//    {
//        if (stream.IsWriting)
//        {
//            // We own this player: send the others our data
//            Texture2D texture2D = new Texture2D(webcamTexture.width, webcamTexture.height);
//            texture2D.SetPixels(webcamTexture.GetPixels());
//            texture2D.Apply();

//            // Resize the texture to a lower resolution
//            TextureScale.Bilinear(texture2D, texture2D.width / 4, texture2D.height / 4);

//            byte[] textureBytes = texture2D.EncodeToJPG();
//            stream.SendNext(textureBytes);

//            // Destroy the Texture2D object to free up memory
//            Destroy(texture2D);
//        }
//        else
//        {
//            // Network player, receive data
//            byte[] textureBytes = (byte[])stream.ReceiveNext();
//            Texture2D texture = new Texture2D(2, 2);
//            texture.LoadImage(textureBytes);
//            texture.Apply();

//            // Update the Renderer's texture with the received image data
//            rend.material.mainTexture = texture;

//            // Destroy the old texture to free up memory
//            if (rend.material.mainTexture != null)
//            {
//                Destroy(rend.material.mainTexture);
//            }
//        }
//    }



//}


