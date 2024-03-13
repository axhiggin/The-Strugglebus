using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Last edited - Norman Zhu 1:29PM 2/21/24

// InputManager will hold hotkeys (if changed in the settings)
//              detect input from the controls,
//              make calls to abstracted functionality in managers when appropriate.
//
public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    public static InputManager Instance { get { return _instance; } }


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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
