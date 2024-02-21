using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Last edited - Norman Zhu 1:04PM 2/21/24

// GameManager singleton handles win/lose conditions
//                       as well as managing the other manager classes.
//                       i.e. gamemanager detects scene change
//                            gamemanager tells other managers to start/stop their logic cycles through bool flags or function calls.

// Handle scene management here too, as it shouldn't be too much code, maybe.;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

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
    }

    void OnSceneUnloaded(Scene scene)
    {
        // This function will be called whenever a scene is unloaded
        Debug.Log("Scene unloaded: " + scene.name);
    }
}
