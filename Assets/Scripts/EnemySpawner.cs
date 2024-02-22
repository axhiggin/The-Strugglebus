using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Last edited - Norman Zhu 3:32PM 2/21/24 - InstantiatePrefabs() GetObjectFromPool() spawnEnemy()
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;          // Just use this for one enemy type for nwo.
    // public GameObject specialEnemyPrefab;

    public int maxEnemies = 5;              // Max number of enemies to be spawned by this spawner at one time.
                                            // Also used during instantiation of object pool.
                                            // Default = 5.

    public List<GameObject> localEnemyPool; // List of the enemies spawned by self.


    public GameObject waypointPrefab;       // Use an empty gameObject prefab.

    public List<GameObject> waypointList;      // Contains a list of waypoints for enemies to travel through.
                                            // Inherited by enemies spawned. 
                                            // Use empty gameObject prefabs with colliders.


    // Instantiates object pool with provided enemyPrefab.
    //         Extend: if multiple enemy types, use weighted random to fill object pool with desired distribution of special to normal enemy types.
    void InstantiatePrefabs()
    {
        localEnemyPool = new List<GameObject>();

        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject newPrefab = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
            newPrefab.SetActive(false); // Set the instantiated prefab inactive
            localEnemyPool.Add(newPrefab);
        }
    }

    public GameObject GetObjectFromPool()
    {
        foreach (GameObject obj in localEnemyPool)
        {
            if (!obj.activeInHierarchy) // Check if the object is inactive
            {
                return obj; // Return the object
            }
        }
        // If no inactive object found, return null
        return null;
    }

    // "Spawns" one enemy using an inactive object from object pool.
    //      If null was returned, then can't spawn so returns false.
    //      If an object was successfully grabbed and spawned, returns true.
    public bool spawnEnemy()
    {
        GameObject enemyToSpawn = GetObjectFromPool();
        if (enemyToSpawn != null)
        {
            enemyToSpawn.SetActive(true);
            enemyToSpawn.transform.position = this.transform.position;
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        InstantiatePrefabs();
    }

    // Update is called once per frame
    void Update()
    {
        // Temp code for testing.
        // Call spawnEnemy after appropriate spawn logic later.
        bool successfullySpawned = spawnEnemy();
        if (successfullySpawned)
        {
            Debug.Log("Spawned one enemy. Number of enemies in local pool: " + localEnemyPool.Count);
        }
    }
}
