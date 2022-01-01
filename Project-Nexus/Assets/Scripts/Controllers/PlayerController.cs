using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [Header("PlayerInfo")]
    public int playerId;

    [Header("PlayerStats")]
    public float playerMoveSpeed;
    public float playerJumpForce;

    [Header("PlayerComponents")]
    public Rigidbody rigidbody;
    public Player photonPlayer;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    [PunRPC]
    public void Initialize(Player player)
    {
        // Get the Unique Player Id Number.
        playerId = player.ActorNumber;
        // Set the reference to the Player Object.
        photonPlayer = player;

        // Add the Player to the Player Array from the GameManager. NOTE: Id's start at 1.
        GameManager.gameInstance.players[playerId - 1] = this;

        // Check if this is a romote Player or if the Player being Initialized is the local Player.
        if (!photonView.IsMine)
        {
            // Disable the Cameraobject if it is NOT the local Player.
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            // Disable the physics on the remote Player object.
            rigidbody.isKinematic = true;
        }
    }


    // Update is called every frame.
    private void Update()
    {
        // Move the Player Object
        MovePlayer();

        // Try and Jump with the Player Object.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryPlayerJump();
        }
    }

    /// <summary>
    /// Move the Player Object.
    /// </summary>
    private void MovePlayer()
    {
        // Get the Input axis values.
        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");

        // Calculate a vector direction.
        Vector3 moveDirection = (transform.forward * zMove + transform.right * xMove) * playerMoveSpeed;
        moveDirection.y = rigidbody.velocity.y; // Set the y direction to equal the current velocity of the rigidbody component on the Player obect.

        // Set the velocity of the rigidbody component on the Player object to the calculated moveDirection Vector3.
        rigidbody.velocity = moveDirection;
    }

    /// <summary>
    /// Attempt to Jump with the Player Object.
    /// </summary>
    private void TryPlayerJump()
    {
        // Cast a downward facing ray to check if the is on the ground and therefore able to jump.
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, 1.5f)) // Fire the ray 1.5f Unity Units downward.
        {
            // If the ray fired hit something we can jump
            rigidbody.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
        }
    }
}