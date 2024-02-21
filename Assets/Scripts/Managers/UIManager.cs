using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Last edited - Norman Zhu 1:26PM 2/21/24

// Example use:
// Abstract button pressed into appropriate UI relevant logic.
// InputManager just calls UIManager.escPressed(). Doesn't have to know what escPressed does, or what scene they're in.
//                          UIManager.escPressed() performs differently depending on scene.
public class UIManager : MonoBehaviour
{

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // escPressed() will check scene.
    //          if startscreen, performs back button logic in menu.
    //          if in-game, performs pause button logic.
    void escPressed()
    {


    }
}
