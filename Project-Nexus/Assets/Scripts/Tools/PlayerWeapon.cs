using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PlayerWeapon : MonoBehaviour
{
    [Header("WeaponStats")]
    public int weaponDamage;
    public int currentAmmo;
    public int maxAmmo;
    public float bulletSpeed;
    public float fireRate;

    private float lastShotTime;

    [Header("Components")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPosition;

    private PlayerController player;

    private void Awake()
    {
        // Set the reference to the Player component on Awake.
        player = GetComponent<PlayerController>();
    }

    /// <summary>
    /// Fors�g at affyr et skud.
    /// Metoden fejler og et skud bliver ikke affyret med mindre at der er g�et nok tid siden sidste skud blev affyret.
    /// </summary>
    public void TryShoot()
    {
        // Check if Player has enough ammunition to fire and the required time since last shot fired has passed.
        if (currentAmmo <= 0 || Time.time - lastShotTime < fireRate)
        {
            return; // Return if the requirements are NOT met.
        }

        currentAmmo--;
        lastShotTime = Time.time;

        // Update the Ammo indicator in the UI.
        BattleUI.uIInstance.UpdateAmmoText();

        // Spawn the Bullet. Bullet is spawned for all Players. Method is called on all the remote clients.
        player.photonView.RPC("SpawnBullet", RpcTarget.All, bulletSpawnPosition.position, Camera.main.transform.forward);
    }

    /// <summary>
    /// Instatiates a Bullet object
    /// </summary>
    /// <param name="spawnPosition">The Position where the bullet is spawned.</param>
    /// <param name="bulletDirection">The Direction the Bullet travels in.</param>
    [PunRPC]
    private void SpawnBullet(Vector3 spawnPosition, Vector3 bulletDirection)
    {
        // Spawn the bullet.
        GameObject bulletObject = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        // Orient the bullet.
        bulletObject.transform.forward = bulletDirection;

        // Get a reference to the Bullet Script.
        Bullet bulletScript = bulletObject.GetComponent<Bullet>();

        // Reference the SphereCollider so that it can be guarenteed to be a trigger.
        var bulletCollider = bulletObject.GetComponent<SphereCollider>();
        bulletCollider.isTrigger = true;    // Make sure the SphereCollider is a trigger.
        bulletCollider.radius = 0.5f;       // Because the SphereCollider's radius is calculated from the center of components relative space
                                            // the radius of the SphereCollider needs to be 0.5f as the center of the SphereCollider is
                                            // defaulting to the Center of the relative SphereCollider.

        // Initialize the Bullet and set it's velocity.
        bulletScript.Initialize(weaponDamage, player.playerId, player.photonView.IsMine);
        bulletScript.rigidbody.velocity = bulletDirection * bulletSpeed;
    }

    /// <summary>
    /// Give a Player more ammunition.
    /// </summary>
    /// <param name="ammunitionToGive">Amount of ammunition to give to the Player.</param>
    [PunRPC]
    public void GiveAmmunition(int ammunitionToGive)
    {
        currentAmmo = Mathf.Min(currentAmmo + ammunitionToGive, maxAmmo);

        // Update the Ammunition text in the UI
        BattleUI.uIInstance.UpdateAmmoText();
    }
}