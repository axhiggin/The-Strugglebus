using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Handles enemy spawning, and group enemy behavior.
// 
public class EnemyManager : MonoBehaviour
{
    public GameObject[] enemyList; // Make this an object pool(?)

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
