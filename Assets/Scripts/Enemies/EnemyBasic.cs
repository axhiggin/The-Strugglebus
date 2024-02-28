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

    // Making sure we're only iterating over the diagonals aren't blocked off on both cardinal sides,
    // By making a list of the valid_diagonals. 
    private List<Vector3Int> get_valid_diagonals(Vector3Int current)
    {
        List<Vector3Int> valid_diagonals = new List<Vector3Int>();
        if (PathingMap.Instance.tm.GetTile(current + Vector3Int.up) == null || PathingMap.Instance.tm.GetTile(current + Vector3Int.left) == null)
        {
            // Check if current up left is within the bounds of the PathingMap grid.
            if (current.x + 1 < PathingMap.Instance.x_upper_bound && current.y + 1 < PathingMap.Instance.y_upper_bound)
            {
                valid_diagonals.Add(current + Vector3Int.up + Vector3Int.left);
            }
        }
        if (PathingMap.Instance.tm.GetTile(current + Vector3Int.up) == null || PathingMap.Instance.tm.GetTile(current + Vector3Int.right) == null)
        {
            // Check if current up right is within the bounds of the PathingMap grid.
            if (current.x + 1 < PathingMap.Instance.x_upper_bound && current.y + 1 < PathingMap.Instance.y_upper_bound)
            {
                valid_diagonals.Add(current + Vector3Int.up + Vector3Int.right);
            }
        }
        if (PathingMap.Instance.tm.GetTile(current + Vector3Int.down) == null || PathingMap.Instance.tm.GetTile(current + Vector3Int.right) == null)
        {
            // Check if current down right is within the bounds of the PathingMap grid.
            if (current.x + 1 < PathingMap.Instance.x_upper_bound && current.y + 1 < PathingMap.Instance.y_upper_bound)
            {
                valid_diagonals.Add(current + Vector3Int.down + Vector3Int.right);
            }
        }
        if (PathingMap.Instance.tm.GetTile(current + Vector3Int.down) == null || PathingMap.Instance.tm.GetTile(current + Vector3Int.left) == null)
        {
            // Check if current down left is within the bounds of the PathingMap grid.
            if (current.x + 1 < PathingMap.Instance.x_upper_bound && current.y + 1 < PathingMap.Instance.y_upper_bound)
            {
                valid_diagonals.Add(current + Vector3Int.down + Vector3Int.left);
            }
        }
        return valid_diagonals;
    }

    // Uses the PathingMap to check adjacent cells in grid and find one with shortest distance to destination.
    // Returns a vector3 representing the direction to move in.
    Vector3 grabVectorFromPathingMap()
    {
        Vector3Int enemyCell = PathingMap.Instance.tm.WorldToCell(transform.position);
        Vector3Int targetCell = new Vector3Int(0, 0, 0);
        float lowest_flow_value = -1.0f; // Initialized to -1.0f to indicate NULL.

        List<Vector3Int> valid_neighbors = get_valid_diagonals(enemyCell);
        valid_neighbors.Add(enemyCell + Vector3Int.up);
        valid_neighbors.Add(enemyCell + Vector3Int.down);
        valid_neighbors.Add(enemyCell + Vector3Int.left);
        valid_neighbors.Add(enemyCell + Vector3Int.right);

        // Loop over valid neighboring cells and find the lowest flow field value.
        foreach (Vector3Int neighbor in valid_neighbors)
        {
            if (PathingMap.Instance.flowMap[neighbor] != null)
            {
                float neighbor_flow_value = PathingMap.Instance.flowMap[neighbor];
                if (lowest_flow_value == -1.0f || neighbor_flow_value < lowest_flow_value)
                {
                    targetCell = neighbor;
                    lowest_flow_value = PathingMap.Instance.flowMap[neighbor];
                }
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
