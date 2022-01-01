using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    // Reference the Menu Screens.
    [Header("Screens")]
    public GameObject mainMenuScreen;
    public GameObject createLobbyScreen;
    public GameObject LobbyScreen;
    public GameObject LobbyBrowserScreen;

    // Reference the Menu variables.
    [Header("Main Screen")]
    public Button createLobbyButton;    // Reference the buttons so that we can disable the buttons as needed to prevent
    public Button findLobbyButton;      // Player from doing any network commands before we are connected to the master server.

    [Header("Lobby")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI lobbyInfoText;
    public Button startGameButton;

    [Header("LobbyBrowser")]
    public RectTransform lobbyListContainer;
    public GameObject lobbyButtonPrefab;

    private List<GameObject> lobbyButtons = new List<GameObject>();
    private List<RoomInfo> lobbyList = new List<RoomInfo>();

    private void Start()
    {
        // Disable the menu buttons at start so no network communication can take place before we have connected to the Master Server.
        createLobbyButton.interactable = false;
        findLobbyButton.interactable = false;

        // Make sure the Cursor is enabled.
        Cursor.lockState = CursorLockMode.None;

        // Check if the Player is returning to the menu from a game.
        if (PhotonNetwork.InRoom)
        {
            // Return the Player to the Lobby.

            // Make the Lobby visible in the LobbyListScreen also make sure that the Lobby is open so people can join the Lobby.
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }

    /// <summary>
    /// Switches the current visible screen.
    /// </summary>
    /// <param name="targetScreen">The Screen to be made visible.</param>
    private void SetScreen(GameObject targetScreen)
    {
        // Disable all the Screens.
        mainMenuScreen.SetActive(false);
        createLobbyScreen.SetActive(false);
        LobbyScreen.SetActive(false);
        LobbyBrowserScreen.SetActive(false);

        // Enable the target screen.
        targetScreen.SetActive(true);
    }

    /// <summary>
    /// Change the player nickname.
    /// </summary>
    /// <param name="playerNameInput">The new player nickname.</param>
    public void OnPlayerNameChanged(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    /// <summary>
    /// Enable the menu buttons disabled in the Start method.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        // Enable the menu buttons when a connection to the Master Server has been established.
        createLobbyButton.interactable = true;
        findLobbyButton.interactable = true;
    }

    /// <summary>
    /// Switch to the createLobbyScreen.
    /// </summary>
    public void OnCreateLobbyButton()
    {
        SetScreen(createLobbyScreen);
    }

    /// <summary>
    /// Switch to the LobbyBrowserScreen.
    /// </summary>
    public void OnFindLobbyButton()
    {
        SetScreen(LobbyBrowserScreen);
    }

    /// <summary>
    /// Returns the Player to the MainMenuScreen.
    /// </summary>
    public void OnMainManuButton()
    {
        SetScreen(mainMenuScreen);
    }

    /// <summary>
    /// Create a Lobby when the "Create" Button in the CreateLobbyScreen is pressed.
    /// </summary>
    /// <param name="lobbyNameInput">The name of the Lobby being created.</param>
    public void OnCreateButton(TMP_InputField lobbyNameInput)
    {
        NetworkController.instance.CreateLobby(lobbyNameInput.text);
    }

    /* ------- LOBBY SCREEN -------*/

    /// <summary>
    /// Switch to the LobbyScreen, and use a RPC call to tell every other Player to update their LobbyUI.
    /// This Method is only called on the Client Terminal.
    /// </summary>
    public override void OnJoinedRoom()
    {
        SetScreen(LobbyScreen);
        // When the Player has joined a Lobby tell all the other Players to update their LobbyUi.
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    /// <summary>
    /// Update the Lobby UI when a player leaves the Lobby.
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    /// <summary>
    /// Update the Lobby UI to show connected Players and disable Start Game button if the Player is NOT the Lobby host.
    /// This Method is remote-callable as a PunRPC.
    /// </summary>
    [PunRPC]
    private void UpdateLobbyUI()
    {
        // Check if the Player is the Lobby host and either disable or enable the Start Game button accordingly.
        startGameButton.interactable = PhotonNetwork.IsMasterClient;

        // Display all the Players currently in the Lobby
        playerListText.text = "";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }

        // Update the Lobby info text.
        lobbyInfoText.text = "<b>Lobby Name</b>\n" + PhotonNetwork.CurrentRoom.Name;
    }

    public void OnStartGameButton()
    {
        // Hide the Lobby so people cannot se or join the Lobby when the game has begun.
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        // Tell all the Players in the Lobby to load into the Game scene.
        NetworkController.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Battlefield");
    }

    public void OnLeaveLobbyButton()
    {
        // Disconnect the Player from the Lobby.
        PhotonNetwork.LeaveRoom();
        // Return to the MainMenuScreen.
        SetScreen(mainMenuScreen);
    }
}