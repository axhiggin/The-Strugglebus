using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// Last edited - Norman Zhu 3:32PM 2/21/24 

// Handles enemy spawning, and group enemy behavior.
// 
public class EnemyManager : MonoBehaviour
{
    public GameObject spawnerPrefab;
    public List<GameObject> spawnerList; // Each spawner handles its own local list of enemies.
                                     //         And its own unique list of Waypoint references to map a route.

                                     // Instantiated, not object pooled.
                                     //         Only need one or a few at level start.
                                     

    private static EnemyManager _instance;

    public static EnemyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<EnemyManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        // TEMP CODE. REPLACE WITH SUBSCRIBE TO GAMEMANAGER EVENT INSTEAD FOR PREP / ENEMY PHASES.
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += spawnSpawner;
        spawnerList = new List<GameObject>();
    }

    private void spawnSpawner(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene1")
        {
            Debug.Log("EnemyManager: Spawning spawner.");
            // TEMP CODE. HARD CODED SPAWNER LOCATION.
            Vector3 spawnLoc = new Vector3(2, 2, 0);
            GameObject newSpawner = Instantiate(spawnerPrefab, spawnLoc, Quaternion.identity);
            newSpawner.SetActive(true);
            newSpawner.transform.SetParent(this.transform);
            spawnerList.Add(newSpawner); 

            spawnLoc = new Vector3(-2, 2, 0);
            newSpawner = Instantiate(spawnerPrefab, spawnLoc, Quaternion.identity);
            newSpawner.SetActive(true);
            newSpawner.transform.SetParent(this.transform);
            spawnerList.Add(newSpawner);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
