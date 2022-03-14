using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ObjectPoolController : MonoBehaviour
{
    //public static ObjectPoolController objectPoolInstance;

    [Header("Objects")]
    public GameObject objectPrefab;

    [Header("Variables")]
    public int createOnStart;
    private List<GameObject> objectPool = new List<GameObject>();

    // Start is called before the first frame update
    public void Start()
    {
        // Instantiate and add all the objects to the objectpool that the objectpool needs on start of a battle.
        for (int i = 0; i < createOnStart; i++)
        {
            CreateNewGameObject();
        }
    }

    /// <summary>
    /// Instantiate a new gameobject, add the new gameobject to the objectpool.
    /// Gameobjects are by default disabled when they are added to the objectpool.
    /// </summary>
    /// <returns>Returns a reference to the newly created gameobject.</returns>
    public GameObject CreateNewGameObject()
    {
        // Instantiate and maintain a reference to a new gameobject.
        GameObject gameobject = PhotonNetwork.Instantiate("Bullet", new Vector3(0f, 0f, 0f), Quaternion.identity);
        gameObject.SetActive(false);    // Disable the gameobject when it is first instantiated.

        // Add the newly created object to the objectpool.
        objectPool.Add(gameobject);

        // Return the newly created gameobject.
        return gameobject;
    }

    /// <summary>
    /// Find a gameobject inside the objectpool.
    /// Also check if the object is inactive.
    /// </summary>
    /// <returns>Returns a reference to a gameobject in the objectpool.</returns>
    public GameObject GetObjectFromPool()
    {
        // Get a reference to an inactive gameobject in the objectpool.
        GameObject tempGameObject = objectPool.Find(g => g.activeInHierarchy == false);

        if (tempGameObject == null)
        {
            // If there is no inactive gameobjects in the objectpool then we create a new gameobject and add it to the objectpool.
            tempGameObject = CreateNewGameObject();
        }

        //activate the gameobject.
        tempGameObject.SetActive(true);

        // Return a reference to the gameobject.
        return tempGameObject;
    }

    public void Destroy(GameObject bulletObject)
    {

    }
}