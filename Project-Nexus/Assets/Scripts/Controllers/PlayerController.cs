using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("PlayerStats")]
    public float playerMoveSpeed;
    public float playerJumpForce;

    [Header("PlayerComponents")]
    public Rigidbody rigidbody;


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