using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// Norman - 2/27/24 10:45PM - Added grabVectorFromPathingMap() function.
//                                  Uses PathingMap's generated flow map to return a vector3 direction.
//                                  Vector3 represents direction from EnemyObject's position to the center of next cell to move to.

public class EnemyBasic : MonoBehaviour
{
    public List<GameObject> waypointList;      // Contains an ordered list of waypoints for enemies to travel through.
                                               // Inherited from parent enemySpawner when 'spawned'.
                                               // Remove vector3s from list as traversed.
    public float speed = 10.0f;

    // Enemy state flag. isAttacking represents something in range to attack.
    // if isAttacking == true, ignore pathfinding/waypoints. 
    public bool isAttacking;

    public const bool USE_PATHING_MAP = true;
    
    void Start()
    {
        isAttacking = false;
    }

    // Uses the PathingMap to check adjacent cells in grid and find one with shortest distance to destination.
    // Returns a vector3 representing the direction to move in.
    Vector3 grabVectorFromPathingMap()
    {
        Vector3Int enemyCell = PathingMap.Instance.tm.WorldToCell(transform.position);
        Vector3Int targetCell = new Vector3Int(0, 0, 0);
        float lowest_flow_value = -1.0f; // Initialized to -1.0f to indicate NULL.

        // Loop over valid neighboring cells and find the lowest flow field value.
        foreach (Vector3Int neighbor in new Vector3Int[] {
                enemyCell + Vector3Int.up,
                enemyCell + Vector3Int.down,
                enemyCell + Vector3Int.left,
                enemyCell + Vector3Int.right,
                enemyCell + Vector3Int.up + Vector3Int.left,
                enemyCell + Vector3Int.up + Vector3Int.right,
                enemyCell + Vector3Int.down + Vector3Int.left,
                enemyCell + Vector3Int.down + Vector3Int.right})
        {
            float neighbor_flow_value = PathingMap.Instance.flowMap[neighbor];
            if (lowest_flow_value == -1.0f || neighbor_flow_value < lowest_flow_value)
            {
                targetCell = neighbor;
                lowest_flow_value = PathingMap.Instance.flowMap[neighbor];
            }
        }

        // Create a direction vector from enemyCell to targetCell.
        Vector3 thisVector = PathingMap.Instance.tm.GetCellCenterWorld(targetCell) - transform.position;
        return thisVector;
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
                if (USE_PATHING_MAP)
                {
                    // Add force to rigidbody in direction given by grabVectorFromPathingMap()
                    rb.AddForce(grabVectorFromPathingMap().normalized * speed);
                } else
                {
                    // Add force to rigidbody in direction of player
                    rb.AddForce((GameManager.Instance.playerReference.transform.position - transform.position).normalized * speed);
                }
            }
            else
            {
                if (USE_PATHING_MAP)
                {
                    // Add force to rigidbody in direction given by grabVectorFromPathingMap()
                    rb.AddForce(grabVectorFromPathingMap().normalized * speed);
                }
                else
                {
                    // Add force to rigidbody in direction of next waypoint
                    rb.AddForce((waypointList[0].transform.position - transform.position).normalized * speed);
                }
            }
        }
        // If player in range.
        //         Override waypoints and target player.

    }
}
