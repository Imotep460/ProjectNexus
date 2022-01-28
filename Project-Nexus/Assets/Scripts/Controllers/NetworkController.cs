using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// The NetworkController connects to the Photon Master Server.
/// It allows Players/clients to create and Join game Lobbies.
/// The NetworkController is setup to work as a Singleton.
/// </summary>
public class NetworkController : MonoBehaviourPunCallbacks
{
    // NetworkController Variables
    public int maxPlayers = 10;     // Maximum amount of player allowed on the server.

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
        PhotonNetwork.JoinLobby(); // Get a list of all the lobbies available to a Player.
        Debug.Log("Connection to the Master Server established!");

        //CreateLobby("TestRoom");  // Create a test Lobby.
    }

    ///// <summary>
    ///// Test that Player can connect to a Lobby.
    ///// </summary>
    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("Player Joined lobby: " + PhotonNetwork.CurrentRoom.Name); // Check if Player joined a Lobby.
    //}

    /// <summary>
    /// Takes a string and creates the game Lobby with the nessesarry lobby settings; maximum Player count etc.
    /// String input becomes the name of the Lobby.
    /// </summary>
    /// <param name="lobbyName">The name of the Lobby being created.</param>
    public void CreateLobby(string lobbyName)
    {
        RoomOptions lobbyOptions = new RoomOptions();
        lobbyOptions.MaxPlayers = (byte)maxPlayers;         // Set the maximum Player count.
                                                            // NOTE; most of the data set through Photon,
                                                            // and over the network is sent as byte thus we need to cast "maxPlayers" as a byte.

        PhotonNetwork.CreateRoom(lobbyName, lobbyOptions);  // Create the Lobby with the specified lobby options.
    }

    /// <summary>
    /// Let's a Player join a Lobby.
    /// The Lobby to Join is the input string value.
    /// </summary>
    /// <param name="lobbyName">The name of the Lobby being joined.</param>
    public void JoinLobby(string lobbyName)
    {
        PhotonNetwork.JoinRoom(lobbyName);
    }

    /// <summary>
    /// Load a new Scene.
    /// The Scene to Load is given as input.
    /// This Method is remote-callable.
    /// </summary>
    /// <param name="sceneName">The name of the Scene to Load.</param>
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName); // Load a new Scene.
    }

    /// <summary>
    /// Called when a Player disconnects from Photon.
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }

    /// <summary>
    /// Update the UI for all the Players in the Battle.
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Subtract a Player from alivePlayers.
        GameManager.gameInstance.alivePlayers--;
        // Update the BattleUI
        BattleUI.uIInstance.UpdateBattleInfoText();

        // Check if the player is the MasterClient.
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.gameInstance.CheckWinCondition();
        }
    }
}
