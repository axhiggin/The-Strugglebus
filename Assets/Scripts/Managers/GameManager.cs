using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Added code to run countdown logic - Norman Zhu 3/12/2024 11:18AM

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
    // ====================================================== CONSTANTS ====================================================================
    public const bool DEBUG_MODE = true; // Set to false to disable various debug prints and other dependents.
    
    private static GameManager _instance;

    public GameObject playerReference;
    public GameObject enemyTargetReference;
    public delegate void MyEventHandler();

    public static event MyEventHandler StartBuildPhaseEvent;
    public static event MyEventHandler EndBuildPhaseEvent;
    public static event MyEventHandler StartEnemyPhaseEvent;
    public static event MyEventHandler EndEnemyPhaseEvent;
    public static event MyEventHandler GameOverEvent;

    // THESE TWO VARIABLES WILL BE USED IN updateCurrentScaling().
    [SerializeField]
    bool WE_WANT_TO_CLAMP_TIME_SCALING = true;      // Do we want to clamp time scaling?
    [SerializeField]
    float MAX_TIME_SCALE = 20f;                     // max amount of seconds after which we clamp the difficulty scaling.
                                                    // Ex. If player finishes build/enemy phase before this time,
                                                    //     they benefit from having less of a difficulty spike.

    [SerializeField]
    public int difficultyScaling = 1; // This should be a constant in the context of one game
                                      // of the game that defines the scaling curve,
                                      // "one game" being from start to until the player wins/loses
                                      // But not constant.
                                      // Maybe make a difficulty setting in the start menu.

    // Dirty flag for the countdown logic in Update()
    private bool isCountingDown = false;

    // ==================================================== SCALING WITH TIME ==============================================================
    private float currentScaling = 1f;// DO NOT ACCESS THIS VARIABLE DIRECTLY OR CHANGE TO PUBLIC
                                      // USE getCurrentScaling() FUNCTION
                                      // This variable will be a function of the difficultyScaling curve and time elapsed or rounds elapsed.
                                      // It will be incremented as enemyPhaseTime goes on in gameManager.
                                      // Just using a linear function for now
                                      // currentScaling += difficultyScaling * Time.deltaTime
                                      // If you implement functions in other classes that you want to scale as a game goes on,
                                      // write them as functions depending on this float.
    private float startOfRoundCurrentScaling = 1f;

    private float currentPhaseTimeElapsed = 0f; // reset at start of each phase.
                                               // actively incremented in update()
    public bool isBuildPhase = false;          // updated when phase changes.
    public bool isEnemyPhase = false;          // updated when phase changes.
    public bool gameIsOver = false;            // NOT UPDATED YET.

    private int levelCount = 1;                // Use GameManager.Instance.getLevelCount();   // returns levelCount
                                               //     GameManager.Instance.resetLevelCount(); // sets to 1
                                               // This gets reset at the start of the game to 1.
                                               // And incremented every time enemy phase ends.

    private int currentScore = 0;              // Use GameManager.Instance.getScore();           // returns the score
                                               //     GameManager.Instance.incrementScore(int x) // increments the score by x
    public const int DEFAULT_LIVES = 10;
    private int livesRemaining = 0;

    public int totalZombiesKilled = 0;

    //VOLUME CONTROL
    public float musicVolume = 1;
    public float effectsVolume = 1;
    // Default values set here. 
    // SerializeField means you can change in inspector.
    // Inspector values will override values here.
    [SerializeField]
    private float buildPhaseLengthSeconds = 20f;
    [SerializeField]
    private float enemyPhaseLengthSeconds = 20f;
    //-----------------------------------------------------VariablesForOtherClasses----------------------------------------------------------------//
    



    private bool tutorialEnabler = true;
    public TextMeshProUGUI timerLabel;

    public static GameManager Instance { get { return _instance; } }


    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }



    // Start is called before the first frame update
    void Start()
    {
        if (DEBUG_MODE)
            Debug.Log("Hello world - GameManager.\n");

        // Subscribe to the sceneManager Events
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        // Check if the current scene is named "StartScreen"
       //f (SceneManager.GetActiveScene().name == "StartScreen")
       //
       //   // Change scene to "GameScene1"
       //   SceneManager.LoadScene("GameScene1");
       //
    }



    // Update is called once per frame
    void Update()
    {
        //Gets Canvas when Scene is GameScene and then updates the label 
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            GameObject timerLabelObject = GameObject.Find("TimerLabel");
            if(timerLabelObject != null) 
            {
                timerLabel = timerLabelObject.GetComponent<TextMeshProUGUI>(); 
            }

        }
        // If game's not over:
        if (!gameIsOver)
        {
            // Increment timer and scaling.
            currentPhaseTimeElapsed += Time.deltaTime;  // Timer.
            // Debug.Log("currentPhaseTimeElapsed incremented to: " + currentPhaseTimeElapsed);
            updateCurrentScaling();                     // Scaling.
            // Debug.Log("Current scaling is: " + currentScaling);
            //Timer updates UI Label
            UpdateTimerLabel();
            // If build phase timed out, then end phase and start enemy phase.
            if (isBuildPhase && currentPhaseTimeElapsed > buildPhaseLengthSeconds)
            {
                currentPhaseTimeElapsed = 0f;           // Reset timer.
                endBuildPhase();                        // Switch phase.
                startEnemyPhase();
            }
            // If enemy phase timed out, then end phase and start build phase.
            if (isEnemyPhase && currentPhaseTimeElapsed > enemyPhaseLengthSeconds)
            {
                currentPhaseTimeElapsed = 0f;           // Reset timer.
                endEnemyPhase();                        // Switch phase.
                startBuildPhase();
            }
        }

        if (getCurrentTimeRemaining() <= 3.0f && !isCountingDown)
        {
            if (isBuildPhase)
            {
                isCountingDown = true;
                UIManager.Instance.threeSecondUICountdown(4); // 0 indicates STart building image.
                                                                  // to be displayed after the 3 second countdown.
                                                                  // 4 indicates "Times Up" image to be displayed after 3s

            }
            else if (isEnemyPhase)
            {
                isCountingDown = true;
                //Debug.Log("enemy phase detected, calling 3 second UI countdown");
                UIManager.Instance.threeSecondUICountdown(0); // 0 indicates STart building image.
                                                              // to be displayed after the 3 second countdown.
                                                              // 4 indicates "Times Up" image to be displayed after 3s
            }
        }
    }




    // Use some function to return how much to increment the currentScaling
    // Based on given amount of time and the difficultyScaling curve.
    private float difficultyFunction(float time)
    {
        return time * difficultyScaling; // Linear.
    }


    public float getCurrentTimeRemaining()
    {
        if (isBuildPhase)
        {
            return buildPhaseLengthSeconds - currentPhaseTimeElapsed;
        }
        else if (isEnemyPhase)
        {
            return enemyPhaseLengthSeconds - currentPhaseTimeElapsed;
        }
        else
        {
            // Debug.Log("Error: No phase is active. - GameManager getCurrentTimeRemaining()");
            return 0f;
        }
    }

    // Empties currentPhaseTimeElapsed, and increments currentScaling by a function of currentPhaseTimeElapsed and the difficultyScaling.
    private void updateCurrentScaling()
    {
        if (WE_WANT_TO_CLAMP_TIME_SCALING)
        {
            if (currentScaling > startOfRoundCurrentScaling + difficultyFunction(MAX_TIME_SCALE))
            {
                // Debug.Log("Clamping currentScaling");
                currentScaling = startOfRoundCurrentScaling + difficultyFunction(MAX_TIME_SCALE);
            }
        }
        // Some function to set currentScaling
        currentScaling += difficultyFunction(Time.deltaTime);
    }

    public int getScore()
    {
        return currentScore;
    }

    public void incrementScore(int x)
    {
        currentScore += x;
    }

    public void decrementLives(int x) { 
        Debug.Log("Lives before: " + livesRemaining + ". Lives after decrease: " + (livesRemaining - x));
        livesRemaining -= x;
        if (livesRemaining <= 0)
        {
            gameIsOver = true;
            GameOverEvent?.Invoke();
            SceneManager.LoadScene("GameOverScene");
            // Call game over function.
            // Maybe call a game over event here.
            // Maybe call a game over event in UIManager.
            // Maybe call a game over event in EnemySpawn.
            // Maybe call a game over event in Player.
            // Maybe call a game over event in EnemyTarget.
            // Maybe call a game over event in PathingMap.
            // Maybe call a game over event in EnemyBasic.
            // Maybe call a game over event in PlayerBuild
        }
    }

    // GETTER FUNCTION.
    public int getLevelCount()
    {
        return levelCount;
    }
    public void resetLevelCount()
    {
        levelCount = 1;
    }
    // GETTER FUNCTION. Use this when you need things to scale.
    //          *Don't change the variable to public or access directly from outside.
    public float getCurrentScaling()
    {
        return currentScaling;
    }
    private void initializeStartOfGame()
    {
        currentScaling = 1f;
        currentScore = 0;
        totalZombiesKilled = 0;
        livesRemaining = DEFAULT_LIVES;
        playerReference.SetActive(false); // Start player as inactive during first countdown start of game.
        resetLevelCount();
    }

    private IEnumerator startAfterSeconds(float seconds)
    {
        Debug.Log("start after seconds invoked");
        yield return new WaitForSeconds(seconds);
        startBuildPhase();
    }

    private void startBuildPhase()
    {
        //DESPAWN ENEMIES HERE


        if (DEBUG_MODE)
            Debug.Log("Starting build phase");
        // Reset time.
        currentPhaseTimeElapsed = 0f;
        // Resets currentScaling slightly per round.
        currentScaling = currentScaling / 2.0f;
        startOfRoundCurrentScaling = currentScaling; // Track current scaling at start, to clamp later.
        isBuildPhase = true;
        isCountingDown = false;

        playerReference.SetActive(true);
        enemyTargetReference.SetActive(true);
        StartBuildPhaseEvent?.Invoke();

        PathingMap.Instance.generateFlowFieldToEndpoint();

    }
    private void endBuildPhase()
    {
        if (DEBUG_MODE)
            Debug.Log("Ending build phase");
        isBuildPhase = false;

        playerReference.SetActive(false);
        //enemyTargetReference.targetCell();
        EndBuildPhaseEvent?.Invoke();
    }
    private void startEnemyPhase()
    {
        if (DEBUG_MODE)
        Debug.Log("Starting enemy phase");
        isEnemyPhase = true;
        isCountingDown = false;
        // Reset time.
        currentPhaseTimeElapsed = 0f;

        StartEnemyPhaseEvent?.Invoke();
    }
    private void endEnemyPhase()
    {
        if (DEBUG_MODE)
        Debug.Log("Ending enemy phase");
        isEnemyPhase = false;

        levelCount++;

        EndEnemyPhaseEvent?.Invoke();
    }

    // SCENE LOAD TRIGGERS. Functions are subscribed to the sceneManager.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This function will be called whenever a new scene is loaded
        Debug.Log("Scene loaded: " + scene.name);
        HighScore();

        // if scene.name is GameScene1, set the playerReference
        if (scene.name == "GameScene1")
        {
            gameIsOver = false;
            playerReference = GameObject.Find("Player");
            enemyTargetReference = GameObject.Find("EnemyTarget");
            // START THE GAME!
            initializeStartOfGame();
            StartCoroutine(startAfterSeconds(3.0f)); //startBuildPhase();
            UIManager.Instance.threeSecondUICountdown(0); // 0 indicates STart building image.
                                                          // to be displayed after the 3 second countdown.
                                                          // 4 indicates "Times Up" image to be displayed after 3s
        }
    }
    void OnSceneUnloaded(Scene scene)
    {// This function will be called whenever a scene is unloaded
        if (DEBUG_MODE)
            Debug.Log("Scene unloaded: " + scene.name);
    }

    public bool GetTutorialEnabler()
    {
        return tutorialEnabler;
    }

    private void UpdateTimerLabel()
    {
        if (timerLabel != null)
        {
            timerLabel.text = "Time: " + Mathf.Round(currentPhaseTimeElapsed);
        }
    }


    public int getLivesRemaining()
    {
        return livesRemaining;
    }

    
    private void HighScore(){
         if(totalZombiesKilled > PlayerPrefs.GetInt("HighScore")){
            PlayerPrefs.SetInt("HighScore", totalZombiesKilled);
        }
        if(levelCount > PlayerPrefs.GetInt("LevelCount")){
            PlayerPrefs.SetInt("LevelCount", levelCount);
        }
        Debug.Log("Player Prefs high score:" + PlayerPrefs.GetInt("HighScore"));
        Debug.Log("Player Prefs level count:" + PlayerPrefs.GetInt("Level count"));
    }
}
