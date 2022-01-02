using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("BulletStats")]
    private int bulletDamage;   // The damage the bullet does.
    private int attackerId;     // The Unique id of the Player attacking.
    private bool isMyBullet;    // Does the bullet belong to the local Player.

    [Header("Components")]
    public Rigidbody rigidbody;

    /// <summary>
    /// Pass the bullet information onto the bullet when it has been created.
    /// </summary>
    public void Initialize(int bulletdamage, int attackerId, bool isMyBullet)
    {
        this.bulletDamage = bulletdamage;
        this.attackerId = attackerId;
        this.isMyBullet = isMyBullet;

        // Add a lifetime to the Bullet so it is destroyed after a while weather it hits anything or not.
        Destroy(gameObject, 7.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the bullet has hit a Player and if the bullet belongs to the Local Player.
        if (other.CompareTag("Player") && isMyBullet)
        {
            // Get a reference to the Player hit by the Bullet.
            PlayerController playerHit = GameManager.gameInstance.GetPlayer(other.gameObject);

            // Make sure that a bullet fired by the local Player cannot damage the local Player.
            if (playerHit.playerId != attackerId)
            {
                // Call the TakeDamage method on the Player taking damage and ONLY that Player.
                playerHit.photonView.RPC("TakeDamage", playerHit.photonPlayer, attackerId, bulletDamage);
            }

            // After damage calculations destroy the Bullet Object.
            Destroy(gameObject);
        }
    }
}
