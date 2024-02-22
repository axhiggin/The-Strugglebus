using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class EnemyBasic : MonoBehaviour
{
    public List<GameObject> waypointList;      // Contains an ordered list of waypoints for enemies to travel through.
                                               // Inherited from parent enemySpawner when 'spawned'.
                                               // Remove vector3s from list as traversed.
    public float speed = 10.0f;

    // Enemy state flag. isAttacking represents something in range to attack.
    // if isAttacking == true, ignore pathfinding/waypoints. 
    public bool isAttacking;
    
    void Start()
    {
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {

        // Grab rigidbody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        // Only follow Navigation if nothing in range to attack.
        if (isAttacking == false)
        {
            if (waypointList.Count == 0)
            {
                // TEMP CODE: REPLACE WITH NavMesh.SetDestination.
                // Add force to rigidbody in direction of player
                rb.AddForce((GameManager.Instance.playerReference.transform.position - transform.position).normalized * speed);
            }
            else
            {
                // TEMP CODE: REPLACE WITH NavMesh.SetDestination.
                // Add force to rigidbody in direction of next waypoint
                rb.AddForce((waypointList[0].transform.position - transform.position).normalized * speed);
            }
        }
        // If player in range.
        //         Override waypoints and target player.

    }
}
