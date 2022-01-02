using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [Header("PlayerInfo")]
    public int playerId;
    private int currentAttackerId; // The Id of teh Player Object currently attacking the local Player.

    [Header("PlayerStats")]
    public float playerMoveSpeed;
    public float playerJumpForce;
    public int currentHealth;
    public int maxHealth;
    public int playerKills;
    public bool isDead;

    private bool flashingDamage; // Have the localPlayer flash a different color when it's being hit.

    [Header("PlayerComponents")]
    public Rigidbody rigidbody;
    public Player photonPlayer;
    public PlayerWeapon playerWeapon;
    public MeshRenderer meshRenderer;

    /// <summary>
    /// Initialize a Player Object
    /// </summary>
    /// <param name="player">The reference for the Player Object to Initialize.</param>
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
        // Check if this is the local photonView, or if the Player is dead.
        if (!photonView.IsMine || isDead)
        {
            // Return out of the Update method without doing anything if this is not the local photonView.
            return;
        }

        // Move the Player Object
        MovePlayer();

        // Try and Jump with the Player Object.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryPlayerJump();
        }

        if (Input.GetMouseButtonDown(0))
        {
            playerWeapon.TryShoot();
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

    /// <summary>
    /// Player Object takes damage
    /// </summary>
    /// <param name="attackerId">Reference for the Unique Id of the Player attacking the LocalPlayer.</param>
    /// <param name="damageAmount">The amount of damage being taken by the local Player.</param>
    [PunRPC]
    public void TakeDamage(int attackerId, int damageAmount)
    {
        // make sure that if the Player is dead the Player does not take damage.
        if (isDead)
        {
            return;
        }

        currentHealth -= damageAmount;      // Damage the Player.
        currentAttackerId = attackerId;     // Update the currentAttackerId.

        // Flash the player in a different color to indicate damage being done. Player is Flash on remotePlayers not on the Local instance of the Game.
        photonView.RPC("FlashPlayer", RpcTarget.Others);

        // Update the HealtBar in the UI.


        // Die/Destroy the Player if the Health drops to zero or below.
        if (currentHealth <= 0)
        {
            photonView.RPC("PlayerDie", RpcTarget.All);
        }
    }

    /// <summary>
    /// Briefly change the color of the local Player if the Local Player has been hit and is taking damage.
    /// </summary>
    [PunRPC]
    private void FlashPlayer()
    {
        // Check if Player is already flashing to prevent overlapping.
        if (flashingDamage)
        {
            return;
        }

        StartCoroutine(PlayerDamageFlashCorutine());

        // Setup a corutine
        IEnumerator PlayerDamageFlashCorutine ()
        {
            flashingDamage = true;

            Color defaultColor = meshRenderer.material.color;   // Keep a refrence to the Players Default color.
            meshRenderer.material.color = Color.black;          // Change the color of the Player.

            yield return new WaitForSeconds(0.05f);             // Pause the method.

            meshRenderer.material.color = defaultColor;         // Change back to the default color.

            flashingDamage = false;
        }
    }

    /// <summary>
    /// Destroy a Player object.
    /// </summary>
    [PunRPC]
    private void PlayerDie()
    {
        currentHealth = 0;
        isDead = true;

        GameManager.gameInstance.alivePlayers--;

        // Have the Master Client/Game Host check the win condition.
        if (PhotonNetwork.IsMasterClient)
            GameManager.gameInstance.CheckWinCondition();

        // Check if the Local Player is the Player that died.
        if (photonView.IsMine)
        {
            // Check to make sure Player died to another Player and not the forcefield.
            if (currentAttackerId != 0)
            {
                GameManager.gameInstance.GetPlayer(currentAttackerId).photonView.RPC("AddKill", RpcTarget.All);
            }

            // Set the Camera to Spectator mode.
            GetComponentInChildren<CameraController>().MakeSpectator();

            // Disable the physics and hide the Player object.
            rigidbody.isKinematic = true;
            transform.position = new Vector3(0, -50, 0);    // Player is moved to 50 Unity Units below the Map.
        }
    }

    /// <summary>
    /// Add a kill to a Players Kills count.
    /// </summary>
    [PunRPC]
    public void AddKill()
    {
        playerKills++;

        // Update the UI to reflect the Player getting a kill.
    }

    [PunRPC]
    public void HealPlayer(int amountToHeal)
    {
        // Heal the Play and make sure that the Player cannot get more that the Players allowed maxHealth.
        currentHealth = Mathf.Min(currentHealth + amountToHeal, maxHealth);

        // Update the health bar in the Game UI.

    }
}