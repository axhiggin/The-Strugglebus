using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Last edited - Norman Zhu 3:32PM 2/21/24 

// Handles enemy spawning, and group enemy behavior.
// 
public class EnemyManager : MonoBehaviour
{
    public GameObject[] spawnerList; // Each spawner handles its own local list of enemies.
                                     // And unique list of NavMesh destinations.
                                     

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
