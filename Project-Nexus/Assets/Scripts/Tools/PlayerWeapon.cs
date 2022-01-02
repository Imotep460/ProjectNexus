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


    [PunRPC]
    private void SpawnBullet(Vector3 spawnPosition, Vector3 bulletDirection)
    {
        // Spawn the bullet.
        GameObject bulletObject = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        // Orient the bullet.
        bulletObject.transform.forward = bulletDirection;

        // Get a reference to the Bullet Script.
        Bullet bulletScript = bulletObject.GetComponent<Bullet>();

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