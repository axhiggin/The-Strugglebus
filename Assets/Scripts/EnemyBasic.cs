using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyBasic : MonoBehaviour
{
    public List<GameObject> waypointList;      // Contains an ordered list of waypoints for enemies to travel through.
                                            // Inherited from parent enemySpawner when 'spawned'.
                                            // Remove vector3s from list as traversed.
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
