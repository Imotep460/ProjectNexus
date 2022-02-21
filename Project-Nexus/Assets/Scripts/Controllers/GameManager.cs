using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;
using System.Data.SqlClient;

public class GameManager : MonoBehaviourPun
{
    public string leaderboardConnectionString = "Data Source=HASSE-DESKTOP;Initial Catalog=ProjectNexus;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    
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
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

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

        //string playerNickName = GetPlayer(WinningPlayerId).photonPlayer.NickName;
        //string lobbyName = PhotonNetwork.CurrentRoom.Name;
        //int playerKills = players.First(x => !x.isDead).playerKills;
        //string gamePlayedDate = string.Format("{0}/{1}/{2} - {3}:{4}:{5}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        //string gameDuration = string.Format("{0} Minutes", Time.timeSinceLevelLoad / 60);

        //photonView.RPC("PostToLeaderBoard", RpcTarget.MasterClient, playerNickName, lobbyName, playerKills, gamePlayedDate, gameDuration);

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

    //[PunRPC]
    //public void PostToLeaderBoard(string playerNickName, string lobbyName, int playerKills, string gamePlayedDate, string gameDuration)
    //{
    //    StartCoroutine(PostSQL(playerNickName, lobbyName, playerKills, gamePlayedDate, gameDuration));
    //}

    //public IEnumerator PostSQL(string playerNickName, string lobbyName, int playerKills, string gamePlayedDate, string gameDuration)
    //{

    //    SqlConnection sqlConnection = new SqlConnection(leaderboardConnectionString);

    //    try
    //    {
    //        sqlConnection.Open();
    //        Debug.Log(sqlConnection.State);
    //        SqlCommand sqlCommand = new SqlCommand("CreateLeaderboardPost", sqlConnection);

    //        sqlCommand.Parameters.Add(new SqlParameter("@PlayerNickName", playerNickName));
    //        sqlCommand.Parameters.Add(new SqlParameter("@LobbyName", lobbyName));
    //        sqlCommand.Parameters.Add(new SqlParameter("@PlayerKills", playerKills));
    //        sqlCommand.Parameters.Add(new SqlParameter("@GamePlayedDate", gamePlayedDate));
    //        sqlCommand.Parameters.Add(new SqlParameter("@GameDuration", gameDuration));

    //        sqlCommand.ExecuteNonQuery();
    //        sqlConnection.Close();

    //        Debug.Log("Entry created in the leaderboard database.");
    //    }
    //    catch (System.Exception _exception)
    //    {
    //        Debug.LogWarning("Could not post to the Leaderboard server! Please try again." + _exception);
    //    }

    //    yield return new WaitForSeconds(1);
    //}
}
