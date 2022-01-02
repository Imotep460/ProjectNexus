using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    [Header("ForcefieldStats")]
    public float shrinkWaitTime;    // Time to wait before shrinking the Forcefield.
    public float shrinkAmount;      // How much does the Forcefield shrink every time it shrinks.
    public float shrinkDuration;    // How long does it tak to shrink from one sixe to the next.
    public float minimumDiameter;   // The minimum possible diameter of the Forcefield.

    public int forcefieldDamage;    // How much damage does the Forcefield do to a Player.

    private float lastShrinkEndTime;    // When did the Forcefield last stop shrinking.
    private bool isShrinking;           // Is the Forcefield currently shrinking.
    private float targetDiameter;       // The diameter the Forcefield is shrinking to
    private float lastPlayerCheckTime;  // When did the Player last check the Players.


    private void Start()
    {
        lastShrinkEndTime = Time.time;              // Set a lastShrinkEndTime as this is the earliest anyone can have checked for a lastShrinkEndTime.
        targetDiameter = transform.localScale.x;    // On start set a targetDiameter so The forcefield does not prematurely start shrinking.
    }

    private void Update()
    {
        // Check if the Forcefield is currently shrinking.
        if (isShrinking)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * targetDiameter, (shrinkAmount / shrinkDuration) * Time.deltaTime);

            // Check if the Forcefield has reached the targetDiameter.
            if (transform.localScale.x == targetDiameter)
            {
                isShrinking = false;
            }
        }
        else
        {
            // Check if enough time has passed so the Forcefield can Shrink again, also Check if the Forcefield has reached the minimumDiameter.
            if (Time.time - lastShrinkEndTime >= shrinkWaitTime && transform.localScale.x > minimumDiameter)
            {
                Shrink();
            }
        }

        CheckPlayers();
    }

    /// <summary>
    /// Shrink the Forcefield.
    /// </summary>
    private void Shrink()
    {
        isShrinking = true;

        // Make sure that the Forcefield does not shrink below the minimumDiameter.
        if (transform.localScale.x - shrinkAmount > minimumDiameter)
        {
            targetDiameter -= shrinkAmount;
        }
        else
        {
            targetDiameter = minimumDiameter;
        }

        // Calculate a new lastShrinkEndTime.
        lastShrinkEndTime = Time.time + shrinkDuration;
    }

    private void CheckPlayers()
    {
        // Check if enough time has passed since the last Check.
        if (Time.time - lastPlayerCheckTime > 1.0f)
        {
            lastPlayerCheckTime = Time.time;

            // Loop through the Players.
            foreach (PlayerController player in GameManager.gameInstance.players)
            {
                // Check and skip the Player if the Player is dead.
                if (player.isDead || !player)
                {
                    continue;
                }

                // Check if the Player is outside the forcefield.
                if (Vector3.Distance(Vector3.zero, player.transform.position) >= transform.localScale.x)
                {
                    player.photonView.RPC("TakeDamage", player.photonPlayer, 0, forcefieldDamage);
                }
            }
        }
    }
}