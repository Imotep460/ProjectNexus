using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


/// <summary>
/// List of available PickUpTypes.
/// Medpack, Ammunition.
/// </summary>
public enum PickUpType
{
    MedPack,
    Ammunition
}

public class Pickup : MonoBehaviour
{
    [Header("PickUpInfo")]
    public PickUpType pickUpType;
    public int pickUpValue;

    /// <summary>
    /// Controls what happens when a Player enters the Trigger of a pickup item.
    /// </summary>
    /// <param name="other">References the object that collided with the sphere collider.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Make sure only the Master Client controls this stuff.
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            // Reference the Player object.
            PlayerController player = GameManager.gameInstance.GetPlayer(other.gameObject);

            //switch (pickUpType)
            //{
            //    case PickUpType.MedPack:
            //        player.photonView.RPC("HealPlayer", player.photonPlayer, pickUpValue);      // Just send/Run the method on the Client of the Player being healed.
            //        break;
            //    case PickUpType.Ammunition:
            //        player.photonView.RPC("GiveAmmunition", player.photonPlayer, pickUpValue);  // Just send/run the method on the Client of the Player picking up ammo.
            //        break;
            //    default:
            //        break;
            //}
            
            if (pickUpType == PickUpType.MedPack)
            {
                player.photonView.RPC("HealPlayer", player.photonPlayer, pickUpValue);      // Just send/Run the method on the Client of the Player being healed.
            }
            else if (pickUpType == PickUpType.Ammunition)
            {
                player.photonView.RPC("GiveAmmunition", player.photonPlayer, pickUpValue);  // Just send/run the method on the Client of the Player picking up ammo.
            }

            // Destroy the PickUp Object on all the Players client
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
