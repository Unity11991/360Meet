using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    private List<Transform> unusedSpawnPoints;

    //public float minX;
    //public float maxX;
    //public float minY;
    //public float maxY;


    private void Start()
    {
        unusedSpawnPoints = new List<Transform>(spawnPoints);

        //Vector2 randomPos = new Vector3(0, 0, 0);
        //PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity);

        if (unusedSpawnPoints.Count > 0)
        {
            // Generate a random index for the list of unused spawn points
            int index = Random.Range(0, unusedSpawnPoints.Count);

            // Spawn the player at the randomly selected spawn point
            PhotonNetwork.Instantiate(playerPrefab.name, unusedSpawnPoints[index].position, Quaternion.identity);

            // Remove the used spawn point from the list
            unusedSpawnPoints.RemoveAt(index);
        }
        else
        {
            Debug.LogError("No unused spawn points available!");
        }
    }
    //public GameObject playerPrefab;
    //public Transform[] spawnPoints; // Set these points in the Unity Inspector

    //// List to keep track of unused spawn points
    //private List<Transform> unusedSpawnPoints;

    //private void Start()
    //{
    //    // Initialize the list with all spawn points
    //    unusedSpawnPoints = new List<Transform>(spawnPoints);

    //    SpawnPlayer();
    //}

    //private void SpawnPlayer()
    //{
    //    if (unusedSpawnPoints.Count > 0)
    //    {
    //        // Generate a random index for the list of unused spawn points
    //        int index = Random.Range(0, unusedSpawnPoints.Count);

    //        // Spawn the player at the randomly selected spawn point
    //        PhotonNetwork.Instantiate(playerPrefab.name, unusedSpawnPoints[index].position, Quaternion.identity);

    //        // Remove the used spawn point from the list
    //        unusedSpawnPoints.RemoveAt(index);
    //    }
    //    else
    //    {
    //        Debug.LogError("No unused spawn points available!");
    //    }
    //}

}
