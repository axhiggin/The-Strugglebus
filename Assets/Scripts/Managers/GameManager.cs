using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Last edited - Norman Zhu 1:04PM 2/21/24

// GameManager singleton handles win/lose conditions
//                       as well as managing the other manager classes.
//                       i.e. gamemanager detects scene change
//                            gamemanager tells other managers to start/stop their logic cycles through bool flags or function calls.

// Handle scene management here too, as it shouldn't be too much code, maybe.

// Also, use gameManager to implement observer pattern.
//      Run events in gameManager based on game logic.
//      Other classes subscribe to these events and run their own logic based on the gameManager's events.

//      Ex. Enemy phase starts, gameManager runs event. EnemySpawn is subscribed to this event and triggers StartSpawning().
//          Preparation phase starts, gameManager runs event. EnemySpawn is subscribed to this event and triggers StopSpawning().

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public GameObject playerReference;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
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
        Debug.Log("Hello world - GameManager.\n");

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Subscribe to the sceneUnloaded event
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        // Check if the current scene is named "StartScreen"
        if (SceneManager.GetActiveScene().name == "StartScreen")
        {
            // Change scene to "GameScene1"
            SceneManager.LoadScene("GameScene1");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This function will be called whenever a new scene is loaded
        Debug.Log("Scene loaded: " + scene.name);

        // if scene.name is GameScene1, set the playerReference
        if (scene.name == "GameScene1")
        {
            playerReference = GameObject.Find("Player");
        }
    }

    void OnSceneUnloaded(Scene scene)
    {
        // This function will be called whenever a scene is unloaded
        Debug.Log("Scene unloaded: " + scene.name);
    }
}
