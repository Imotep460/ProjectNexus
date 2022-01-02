using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class BattleUI : MonoBehaviour
{
    [Header("UIElements")]
    public Slider healthBar;                // References the Player health bar.
    public TextMeshProUGUI battleInfoText;  // References the Battle information text.
    public TextMeshProUGUI ammoText;        // References the Players ammo text.
    public TextMeshProUGUI winText;         // References the winner text.
    public Image winBackground;             // References the winner text background.

    private PlayerController player;        // References the local Player whose information is being shown in the UI.

    // Setup a instance of the BattleUI.
    public static BattleUI uIInstance;

    private void Awake()
    {
        uIInstance = this;
    }

    /// <summary>
    /// Carry over a PlayerController for the localPlayer, and set the HealthBar values.
    /// </summary>
    public void InitializeUI(PlayerController localPlayer)
    {
        player = localPlayer;

        // Set the healthBar values.
        healthBar.maxValue = player.maxHealth;  // Max value
        healthBar.value = player.currentHealth; // Current health value.

        // Update the ammotext and the battleinfo.
        UpdateBattleInfoText();
        UpdateAmmoText();

        // Make sure the winbackgorund it disabled when the Battle is started.
        winBackground.gameObject.SetActive(false);
    }

    /// <summary>
    /// Update the HealthBar.
    /// </summary>
    public void UpdateHealthBar()
    {
        healthBar.value = player.currentHealth;
    }

    /// <summary>
    /// Update the BattleInfoText.
    /// </summary>
    public void UpdateBattleInfoText()
    {
        // Show current alive players and max Players in the Battle also show the amount of kills the Player has.
        battleInfoText.text = "Players Alive: " + GameManager.gameInstance.alivePlayers + " / " + NetworkController.instance.maxPlayers + "\nPlayers Killed: " + player.playerKills;
    }

    /// <summary>
    /// Update the Players ammo text.
    /// </summary>
    public void UpdateAmmoText()
    {
        // Shows current and Maimum ammonition.
        ammoText.text = "Ammo:\n" + player.playerWeapon.currentAmmo + " / " + player.playerWeapon.maxAmmo;
    }

    /// <summary>
    /// Enable and Update the winning Game text.
    /// </summary>
    /// <param name="winnerName">The nickname of the winning Player.</param>
    public void SetWinText(string winnerName)
    {
        winBackground.gameObject.SetActive(true);   // Enable the win text background.
        winText.text = winnerName + "Wins!!!";      // Update the win text to show the winner.
    }
}