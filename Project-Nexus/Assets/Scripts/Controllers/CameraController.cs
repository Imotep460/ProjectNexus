using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CameraController setsup the control for the Camera both as a normal Player and when a Player becomes a Spectator.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Look Sensitivity")]
    public float xSens;     // Sensitivity on the x-axis.
    public float ySens;     // Sensitivity on the y-axis.

    [Header("Clamping")]
    public float yMin;      // How far down can a PLayer look.
    public float yMax;      // How far up in the air can a PLayer look.

    [Header("Spectator")]
    public float moveSpeedSpectator;    // How fast does a Player move the camera as a Spectator.

    private bool isSpectator;           // A bool to keep track of whether or not a Player is currently a Spectator.

    // Store a PLayers current rotationvalues.
    private float currentXRot;
    private float currentYRot;

    // Start is called when the class is instantiated.
    private void Start()
    {
        // Lock the Cursor to the center of the screen.
        Cursor.lockState = CursorLockMode.Locked;
    }

    // LateUpdate runs at the end of a frame, it is thus called after regular Update methods.
    private void LateUpdate()
    {
        // Get the Mouse inputs.
        currentXRot += Input.GetAxis("Mouse X") * xSens;
        currentYRot += Input.GetAxis("Mouse Y") * ySens;

        // Clamp the verticle rotation. This prevents the controls from flipping incase a Player looks directly in the air.
        currentYRot = Mathf.Clamp(currentYRot, yMin, yMax);

        // Check for Spectator status.
        if (isSpectator)
        {
            // Camera controls if the Player is a Spectator.
            // Rotate the Camera
            transform.rotation = Quaternion.Euler(-currentYRot, currentXRot, 0); // currentYRot is negative so that it is not flipped.
                                                                                 // Incase a Player want inverted controls simply change currentYRot to positive.

            // Get the Inputs for the axis. Options can be changed in project settings in the Unity Editor
            float xMove = Input.GetAxis("Horizontal");  // Default keys: "a" key to strafe left (negative value) & "d" key to strafe right (positive value)
            float zMove = Input.GetAxis("Vertical");    // Default keys: "s" key to move foward (negative value) & "w" key to move backwards (positive value)
            float yMove = 0;
            
            // Use a keyboard to move the Camera up or down along the y-axis.
            if(Input.GetKey(KeyCode.Q))
            {
                yMove = 1;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                yMove = -1;
            }

            // Cast a Vector3 that holds the movement direction.
            Vector3 moveDirection = transform.right * xMove + transform.up * yMove + transform.forward * zMove;
            // Apply movespeed to the direction to move the Camera in Spectator Mode.
            transform.position += moveDirection * moveSpeedSpectator * Time.deltaTime; // NOTE; Time.deltaTime is independant of the current framerate of the game,
                                                                                       // and thus PLayers move more consistently-
        }
        else
        {
            // Camera controls if the Player is NOT a Spectator.

            // Rotate the Camera vertically.
            transform.localRotation = Quaternion.Euler(-currentYRot, 0, 0);

            // Rotate the Player Object Horizontally.
            transform.parent.rotation = Quaternion.Euler(0, currentXRot, 0);
        }
    }
}