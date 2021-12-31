using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks
{
    // NetworkController Variables
    public int MaxPlayers = 10;     // Maximum amount of player allowed on the server.

    /// <summary>
    /// Setup the NetworkController to work as a Singleton for easy reference.
    /// </summary>
    public static NetworkController instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject); // Setup the gameObject so that it's not destroyed when we switch scenes.
    }

    private void Start()
    {
        // Connect to the Master Server.
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Override the OnConnectedToMaster method from the MonoBehaviorPunCallBacks class.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connection to the Master Server established!");
    }
}
