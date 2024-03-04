using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    // Alex Abadajos 
    //don't actually need to use this but keeping it for later use if neccesary
    [SerializeField] private Canvas gameSceneCanvas;
    [SerializeField] private Canvas pauseScreenCanvas;
    [SerializeField] private Canvas tutorialScreenCanvas;
    [SerializeField] private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        //waits two seconds on scene transition
        StartCoroutine(WaitTwoSeconds());

        //loads GameManager and checks if tutorials are enabled
       gameManager = FindAnyObjectByType<GameManager>();
       bool tutorialsCheckEnabled = gameManager.GetTutorialEnabler();

        //if tutorial enabled show tutorial
        if (!tutorialsCheckEnabled) 
        {
            tutorialScreenCanvas.gameObject.SetActive(true);

        }

        //Needs to enable a tutorial controls canvas
        //Needs to access GameManager and a tutorialenabledbool to disable tutorials in the future
        //Needs to start the buildphase after tutorial panel is read and a confirm button is clicked.
        //Needs to enable a timer when the buildphase starts and to start spawning enemies when the buildphase is finished after a brief delay
    }
    
    public void ClickedTutorialEnd()
    {
        tutorialScreenCanvas.gameObject.SetActive(false);
        StartCoroutine(WaitTwoSeconds());
    }

    //Couroutine for waiting between load and tutorial closing
    IEnumerator WaitTwoSeconds() 
    {
        yield return new WaitForSecondsRealtime(2);
    }
    


    // Update is called once per frame
    void Update()
    {
        
    }
}
