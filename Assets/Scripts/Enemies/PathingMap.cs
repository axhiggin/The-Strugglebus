using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Norman - 2/27/24 8:53PM - Script created.
public class PathingMap : MonoBehaviour
{
    private static PathingMap _instance;
    public static PathingMap Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PathingMap>();
            }

            return _instance;
        }
    }
    public int x_lower_bound = -10;
    public int x_upper_bound = 10;
    public int y_lower_bound = -10;
    public int y_upper_bound = 10;
    public float MAX_DISTANCE = 10000.0f;
    [SerializeField] 
    public Tilemap tm;
    [SerializeField] 
    public Tile tile;

    public Dictionary<Vector3Int, float> flowMap;
    // List of flow fields (Dictionary<Vector3Int, int>) for each waypoint.
    //         Each dictionary indexed using Vector3Int of a cell, returns flow field value.
    //         Uses Vector3Int to pull return value directly from PlayerBuild.cs / Tilemap.WorldToCell

    // Use breadth first search from start to generate a flow field for the index of flowMaps
    public void generateFlowField(Vector3Int start)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(start);

        // Create a dictionary for visited cells and their distance from start
        Dictionary<Vector3Int, float> visited = new Dictionary<Vector3Int, float>();

        visited[start] = 0;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            float distance = visited[current];

            // Visit all adjacent neighbors of the current cell (distance increments by 1)
            foreach (Vector3Int neighbor in new Vector3Int[] {
                current + Vector3Int.up,
                current + Vector3Int.down,
                current + Vector3Int.left,
                current + Vector3Int.right})
            {
                // Check if the neighbor is not visited and the tile in Tilemap for that cell is null
                if (!visited.ContainsKey(neighbor) && tm.GetTile(neighbor) == null)
                {
                    // PLACEHOLDER.
                    // Check that it is within the defined bounds of the map.
                    if (neighbor.x < x_upper_bound && neighbor.x > x_lower_bound && neighbor.y < y_upper_bound && neighbor.y > y_lower_bound)
                    {
                        // add it to the queue and the visited dictionary
                        queue.Enqueue(neighbor);
                        visited[neighbor] = distance + 1;
                    }
                }
                // If we're not enqueueing it, and it's not already visited, then set it to MAX_DISTANCE.
                else if (!visited.ContainsKey(neighbor))
                {
                    visited[neighbor] = MAX_DISTANCE;
                }
            }

            // Visit all diagonal neighbors of the current cell (distance increments by diagonal (sqrt2))
            foreach (Vector3Int neighbor in new Vector3Int[] {
                current + Vector3Int.up + Vector3Int.left,
                current + Vector3Int.up + Vector3Int.right,
                current + Vector3Int.down + Vector3Int.left,
                current + Vector3Int.down + Vector3Int.right})
            {
                // Check if the neighbor is not visited and the tile in Tilemap for that cell is null
                if (!visited.ContainsKey(neighbor) && tm.GetTile(neighbor) == null)
                {
                    // PLACEHOLDER.
                    // Check that it is within the defined bounds of the map.
                    if (neighbor.x < x_upper_bound && neighbor.x > x_lower_bound && neighbor.y < y_upper_bound && neighbor.y > y_lower_bound)
                    {
                        // add it to the queue and the visited dictionary
                        queue.Enqueue(neighbor);
                        visited[neighbor] = distance + Mathf.Sqrt(2);
                    }
                }
                // If we're not enqueueing it, and it's not already visited, then set it to MAX_DISTANCE.
                else if (!visited.ContainsKey(neighbor))
                {
                    visited[neighbor] = MAX_DISTANCE;
                }
            }
        }

        // Set the flow field for the start cell
        flowMap = visited;
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
