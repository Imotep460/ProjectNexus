using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
    public float postGameTime;

    [Header("Players")]
    public string playerPrefabLocation;     // 
    public PlayerController[] players;      // Store the Players
    public Transform[] spawnPoints;         // An array of Transforms for the available Spawnpoints.
    public int alivePlayers;                // Keeps track of how many players are alive in the game scene.
    private int playersInScene;             // Keeps track of how many players are in the scene including Spectators.


    // Setup a instance of the GameManager.
    public static GameManager gameInstance;

    /// <summary>
    /// Assign the Instance of the GameManager on awake.
    /// </summary>
    private void Awake()
    {
        gameInstance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Set the initial size of the players array.
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;

        photonView.RPC("ImInGame", RpcTarget.AllBuffered);

    }
    
    [PunRPC]
    private void ImInGame()
    {
        playersInScene++;

        // Check if Player is the MasterClient/Host and all the Players has loaded into the Game.
        if (PhotonNetwork.IsMasterClient && playersInScene == PhotonNetwork.PlayerList.Length)
        {
            // Use RPC to spawn the players.
            photonView.RPC("SpawnPlayer", RpcTarget.All);
        }


    }

    /// <summary>
    /// Spawn in a Player.
    /// Method is remote-callable.
    /// </summary>
    [PunRPC]
    private void SpawnPlayer()
    {
        // Instantiate a Player Object.
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        // Initialize the Player for all the Players.
        playerObject.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    /// <summary>
    /// Get a reference to a Player Object.
    /// </summary>
    /// <param name="playerId">The Player being referenced.</param>
    /// <returns>Returns a Player Object.</returns>
    public PlayerController GetPlayer(int playerId)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.playerId == playerId)
            {
                return player;
            }
        }

        return null;
    }

    /// <summary>
    /// Get a reference to a Player based of a Player Object.
    /// </summary>
    /// <param name="playerObject">The Player Object being referenced.</param>
    /// <returns>Return a Player Object.</returns>
    public PlayerController GetPlayer(GameObject playerObject)
    {
        //return players.First(x => x.gameObject == playerObject);
        foreach (PlayerController player in players)
        {
            if (player != null && player.gameObject == playerObject)
            {
                return player;
            }
        }

        return null;
    }

    public void CheckWinCondition()
    {
        // Check if local Player is the last Player alive.
        if (alivePlayers == 1)
        {
            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.isDead).playerId);
        }
    }

    /// <summary>
    /// Update the BattleUI to reflect that a Player has won the game.
    /// after a set amount of time return the Players to the Lobby.
    /// Method is remote-callable.
    /// </summary>
    /// <param name="WinningPlayerId"></param>
    [PunRPC]
    private void WinGame(int WinningPlayerId)
    {
        // Set the UI win text.
        BattleUI.uIInstance.SetWinText(GetPlayer(WinningPlayerId).photonPlayer.NickName);

        // Create a delay before returning to the MainMenu after the Game is won/over.
        Invoke("GoBackToMainMenu", postGameTime);
    }

    /// <summary>
    /// Return to the MainMenu Scene.
    /// </summary>
    private void GoBackToMainMenu()
    {
        Destroy(NetworkController.instance); // Avoids dublicating the NetworkController.
        NetworkController.instance.ChangeScene("MainMenu");
    }
}
