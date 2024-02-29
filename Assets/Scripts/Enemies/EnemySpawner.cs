using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Last edited - Norman Zhu 3:32PM 2/21/24 - InstantiatePrefabs() GetObjectFromPool() spawnEnemy()
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;          // More prefabs for more enemy types.
    public List<GameObject> localEnemyPool; // List of the enemies spawned by self.
    public float spawnTimer = 0.0f;         // Actual timer, gets incremented by time.deltaTime in update()
    public float spawnInterval = 1.0f;      // Spawn interval in seconds.
    public int maxEnemies = 5;              // Max number of enemies to be spawned by this spawner at one time.
                                            // Also used during instantiation of object pool.
                                            // Default = 5.
    private bool shouldSpawn = false;

    public GameObject waypointPrefab;       
    public List<GameObject> waypointList;   // Contains a list of waypoints for enemies to travel through.
                                            //      Inherited by enemies spawned. 
                                            //      Use empty gameObject prefabs with colliders.


    // Instantiates object pool with provided enemyPrefab.
    //         Extend: if multiple enemy types, use weighted random to fill object pool with desired distribution of special to normal enemy types.
    void InstantiatePrefabs()
    {
        localEnemyPool = new List<GameObject>();

        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject newPrefab = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
            newPrefab.SetActive(false); // Set the instantiated prefab inactive
            newPrefab.transform.SetParent(this.transform);
            localEnemyPool.Add(newPrefab);
        }
    }

    // Use this function to initialize the waypoint list with ordered waypoints.
    // First waypoint will be the first traversed by enemies.
    public void addWaypoint(Vector3 destination)
    {
        Instantiate(waypointPrefab, destination, Quaternion.identity);
        waypointList.Add(waypointPrefab);
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
            enemyToSpawn.GetComponent<EnemyBasic>().waypointList = new List<GameObject>(waypointList);
                                                                    // Make a copy of waypoint list for the enemy.
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        InstantiatePrefabs();
        waypointList = new List<GameObject>();

        GameManager.StartEnemyPhaseEvent += startSpawning;
        GameManager.EndEnemyPhaseEvent += stopSpawning;
    }

    private void OnDestroy()
    {
        GameManager.StartEnemyPhaseEvent -= startSpawning;
        GameManager.EndEnemyPhaseEvent -= stopSpawning;
    }

    private void startSpawning()
    {
        Debug.Log("Spawner.cs/startSpawning() was successfully invoked through event system.");
        shouldSpawn = true;
    }

    private void stopSpawning()
    {
        Debug.Log("Spawner.cs/stopSpawning() was successfully invoked through event system.");
        shouldSpawn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldSpawn)
        {
            // Temp code for testing.
            // Call spawnEnemy after appropriate spawn logic later.
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0.0f;
                bool successfullySpawned = spawnEnemy();
                if (successfullySpawned)
                {
                    // Debug.Log("Spawned one enemy. Number of enemies in local pool: " + localEnemyPool.Count);
                }
            }
        }
    }
}
