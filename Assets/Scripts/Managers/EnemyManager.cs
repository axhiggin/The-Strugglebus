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
    private int spawnersPerWave = 1;     // can increment this based on currentScaling in GameManager.
    public GameObject[] enemySpritePrefab;

    // Instantiated, not object pooled.
    //         Only need one or a few at level start.

    /*private static EnemyManager _instance;

    public static EnemyManager Instance { get { return _instance; } }


    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("INSTANCE OF ENEMYMANAGER DESTROYED");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        //DontDestroyOnLoad(gameObject);
    }*/
    // Start is called before the first frame update
    void Start()
    {
        // TEMP CODE. REPLACE WITH SUBSCRIBE TO GAMEMANAGER EVENT INSTEAD FOR PREP / ENEMY PHASES.
        // Subscribe to the startbuildphaseevent in gamemanager
        GameManager.StartBuildPhaseEvent += spawnSpawner;
        
        spawnerList = new List<GameObject>();
    }


    private void spawnSpawner()
    {
        //if (scene.name == "GameScene1")
        //{
        if (GameManager.DEBUG_MODE)
            Debug.Log("EnemyManager: Spawning spawner.");
        for (int i = 0; i < spawnersPerWave; ++i)
        {
            // Pick a random tile.
            float xCoord = Random.Range(PathingMap.Instance.x_lower_bound, PathingMap.Instance.x_upper_bound) + 0.5f;
            float yCoord = Random.Range(PathingMap.Instance.y_upper_bound - 1, PathingMap.Instance.y_upper_bound + 1) + 0.5f;

            Vector3 randomTilePosition = new Vector3(xCoord, yCoord, 0);
            Vector3Int tilePosInt = PathingMap.Instance.tm.WorldToCell(randomTilePosition);

            int max_rerolls = 50;
            int current_reroll = 0;
            while (true)
            {
                // check if tile is empty, if not, reroll.
                if (isValidSpawnerTile(tilePosInt))
                    break;
                if (current_reroll >= max_rerolls)
                    break;
                xCoord = Random.Range(PathingMap.Instance.x_lower_bound, PathingMap.Instance.x_upper_bound) + 0.5f;
                yCoord = Random.Range(PathingMap.Instance.y_upper_bound - 1, PathingMap.Instance.y_upper_bound + 1) + 0.5f;
                randomTilePosition = new Vector3(xCoord, yCoord, 0);
                tilePosInt = PathingMap.Instance.tm.WorldToCell(randomTilePosition);
                current_reroll++;
            }

            if (isValidSpawnerTile(tilePosInt))
            {

                Vector3 spawnLoc = new Vector3(xCoord, yCoord, 0);
                Vector3Int spawnerCell = PathingMap.Instance.tm.WorldToCell(spawnLoc);
                PathingMap.Instance.tm.SetTile(spawnerCell, PathingMap.Instance.pathable_invis_tile);
                GameObject newSpawner = Instantiate(spawnerPrefab, spawnLoc, Quaternion.identity);
                newSpawner.SetActive(true);
                newSpawner.transform.SetParent(this.transform);

                spawnerList.Add(newSpawner);
            } else
            {
                Debug.Log("Could not find a valid spawner tile. Did not spawn a spawner.");
            }

        }
    }
    private bool isValidSpawnerTile(Vector3Int tilePosInt)
    {
        if (PathingMap.Instance.generateFlowField(tilePosInt) == false)
        {
            return false;
        }
        if (PathingMap.Instance.tm.GetTile(tilePosInt) == null ||
            PathingMap.Instance.tm.GetTile(tilePosInt).name == "Dungeon_Tileset_v2_78")
        {
            return true;
        }
        return false;
    }
    private void despawnSpawners()
    {

    }

    public int getSpawnerCount()
    {
        return spawnerList.Count;
    }

    public bool vector3_matches_one_spawner(Vector3Int coordinate)
    {
        bool matches = false;
        foreach (GameObject spawner in spawnerList)
        {
            Vector3 spawnerLoc = spawner.transform.position;
            if (PathingMap.Instance.tm.WorldToCell(spawnerLoc) == coordinate)
            {
                Debug.Log("provided coordinate matched with a spawner - EnemyManager()");
                matches = true;
                break;
            }
        }
        return matches;
    }



    private void Update()
    {

    }

    private void OnDestroy()
    {
        GameManager.StartBuildPhaseEvent -= spawnSpawner;
    }
}
