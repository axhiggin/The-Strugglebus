using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;
// using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Last edited - Alex Abadajos 3/3/24-3/4/24

// Example use:
// Abstract button pressed into appropriate UI relevant logic.
// InputManager just calls UIManager.escPressed(). Doesn't have to know what escPressed does, or what scene they're in.
//                          UIManager.escPressed() performs differently depending on scene.
public class UIManager : MonoBehaviour
{
    public bool startScene = false;
    public bool gameScene = false;
    [SerializeField] private Canvas gameSceneCanvas;
    [SerializeField] private Canvas mainMenuSceneCanvas;
    [SerializeField] private GameObject mainMenuSceneCanvasGameObject;
    [SerializeField] private TextMeshProUGUI timerLabel;
    [SerializeField] private Button[] mainMenuSceneButtons;
    [SerializeField] private Button[] gameSceneButtons;
    private bool buttonGrabnFlag;
    private bool oneTimeMainMenuButtonGrabberFlag;
    private bool oneTimeGameSceneButtonGrabberFlag;
    private bool paused = false;

    // Images for 3 2 1 countdown
    [SerializeField] private Image[] countdown;

    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<UIManager>();
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
        //intializes and marks the buttonGrabnFlag true so it doesn't add actionlisteners for everyframe in update.
        buttonGrabnFlag= true;
        oneTimeMainMenuButtonGrabberFlag= true;

    }
    void Update()
    {
        //detects the current scene either startscreen or gamescene
        if (SceneManager.GetActiveScene().name == "StartScreen" && oneTimeMainMenuButtonGrabberFlag ==true)
        {
            GrabAllMainMenuButtons();
            //sets a flag for scene specific interactions such as pressing esc and getting a pause menu, or quitting
            startScene = true;
            gameScene = false;
            //ends the button grabber if statement after one iteration
            oneTimeMainMenuButtonGrabberFlag = false;
        }
        else if(SceneManager.GetActiveScene().name == "GameScene" && oneTimeGameSceneButtonGrabberFlag ==true)
        {
            //sets a flag for scene specific interactions such as pressing esc and getting a pause menu, or quitting
            startScene = false;
            gameScene = true;
        }

        


    }
    //VV These flat out don't work, its been a hot minute since I've done get and sets so I'm probably fucking this up
    public bool GetStartSceneBool { get { return gameScene; }}
    public bool GetGameSceneBool { get { return gameScene; }}


    void GrabAllMainMenuButtons()
    {
            //grabs the mainmenucanvas and pools all the buttons into an array for mouse over interactions such as adjusting transparency on hover.
            mainMenuSceneCanvasGameObject = GameObject.Find("MainMenuCanvas");
            mainMenuSceneCanvas = mainMenuSceneCanvasGameObject.GetComponent<Canvas>();
            mainMenuSceneButtons = mainMenuSceneCanvas.GetComponentsInChildren<Button>();
            // links all button objects with their respective action listeners
            if (buttonGrabnFlag == true)
            {
                for (int i = 0; i < mainMenuSceneButtons.Length; i++)
                {
                    foreach (Button button in mainMenuSceneButtons)
                    {
                        if (button.name == "StartButton")
                        {
                            button.onClick.AddListener(ClickedStartButton);
                            i++;
                        }
                        else if (button.name == "OptionsButton")
                        {
                            button.onClick.AddListener(ClickedOptionsButton);
                            i++;
                        }
                        else if (button.name == "CreditsButton")
                        {
                            button.onClick.AddListener(ClickedCreditsButton);
                            i++;
                        }
                        else if (button.name == "ExitButton")
                        {
                            button.onClick.AddListener(ClickedExitOrESCOnMainMenu);
                            i++;
                        }
                    }
                }
                //stops adding action listeners after the loop iterates through the array of buttons completely once(hopefully)
                buttonGrabnFlag = false;
            }
            
    }

    //Starts game on clicking start button
    //To test gamescene more quickly currently start automatically launches game scene
    public void ClickedStartButton()
    {
        SceneManager.LoadScene("GameScene1");
    }
    //Displays Options sub-menu

    public void ClickedOptionsButton()
    {
        //options sub menu ask the team what options we want 
        Debug.Log("Options Button Clicked");
    }
    //Displays credits
    public void ClickedCreditsButton()
    {

        //either bring up scrolling scene of credits or bring up a ui labels of credits and a back button
        Debug.Log("Credits Button Clicked");
    }

    //Exits
    public void ClickedExitOrESCOnMainMenu()
    {
        Application.Quit();
        Debug.Log("Exit Button Clicked");
    }
    // escPressed() will check scene.
    //          if startscreen, performs back button logic in menu.
    //          if in-game, performs pause button logic.
    void escPressed()
    {
        
        if (gameScene == true) 
        {
            if (!paused) 
            {
                PauseGame();
                //bring up pause menu ui
      
            }
            else if (paused)
            {
                ResumeGame();
                //unbring up pause menu ui
            }
            

        }
        else if (startScene == true)
        {
            ClickedExitOrESCOnMainMenu(); 
        }


    }
    void PauseGame()
    {
        Time.timeScale = 0f;
        paused= true;  
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        paused= false;
    }

    // Countdown for phase start
    public void threeSecondUICountdown(int flag)
    {
        // Disable normal timer.

        // Use large image timer.

        // 3, 2, 1, GO!
        StartCoroutine(setExclusiveActiveAfterSeconds(0, GameSceneUIObjectHolder.Instance.countdown, 3)); // 3... 
        StartCoroutine(setExclusiveActiveAfterSeconds(1, GameSceneUIObjectHolder.Instance.countdown, 2)); // 2...
        StartCoroutine(setExclusiveActiveAfterSeconds(2, GameSceneUIObjectHolder.Instance.countdown, 1)); // 1...
        StartCoroutine(setExclusiveActiveAfterSeconds(3, GameSceneUIObjectHolder.Instance.countdown, flag)); // Go!
        StartCoroutine(setExclusiveActiveAfterSeconds(5, GameSceneUIObjectHolder.Instance.countdown, -1)); // -1 flag to set no index active
                                                                                                           // aka blank
    }

    // Sets everything in objects inactive.
    // Sets the object at the index active.
    private IEnumerator setExclusiveActiveAfterSeconds(float seconds, GameObject[] objects, int index)
    {
        yield return new WaitForSeconds(seconds);
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
        if (index != -1)
        {
            objects[index].SetActive(true);
        }
    }

}
