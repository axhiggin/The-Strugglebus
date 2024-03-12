using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// PLACE ON THE MAIN GAMESCENE1 CANVAS
// NO CODE IN HERE.
// JUST STATIC INSTANCE TO REFERENCE UI ELEMENTS IN THE GAME SCENE
// 
// Added Norman Zhu 3/12/2024 10:39AM
public class GameSceneUIObjectHolder : MonoBehaviour
{
    private static GameSceneUIObjectHolder _instance;
    public GameObject[] countdown;

    public static GameSceneUIObjectHolder Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameSceneUIObjectHolder>();
            }

            return _instance;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
