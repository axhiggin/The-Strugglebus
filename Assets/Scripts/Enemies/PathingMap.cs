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
    [SerializeField] 
    public Tilemap tm;
    [SerializeField] 
    public Tile tile;

    public Dictionary<Vector3Int, int> flowMap;
    // List of flow fields (Dictionary<Vector3Int, int>) for each waypoint.
    //         Each dictionary indexed using Vector3Int of a cell, returns flow field value.
    //         Uses Vector3Int to pull return value directly from PlayerBuild.cs / Tilemap.WorldToCell
    public Dictionary<Vector3Int, Vector3> flowField;
    // Mirror of flowMaps, but instead of int, returns Vector3 of direction to move in.
    //         Recalculated using flowMaps each time flowMaps is recalculated.
    //         Reduce lookup time.


    // Use breadth first search from start to generate a flow field for the index of flowMaps
    public void generateFlowField(Vector3Int start)
    {
        Dictionary<Vector3Int, int> this_map = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(start);

        // Create a dictionary for visited cells and their distance from start
        Dictionary<Vector3Int, int> visited = new Dictionary<Vector3Int, int>();
        visited[start] = 0;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            int distance = visited[current];

            // Write a loop to visit all neighbors of the current cell
            foreach (Vector3Int neighbor in new Vector3Int[] {
                current + Vector3Int.up,
                current + Vector3Int.down,
                current + Vector3Int.left,
                current + Vector3Int.right,
                current + Vector3Int.up + Vector3Int.left,
                current + Vector3Int.up + Vector3Int.right,
                current + Vector3Int.down + Vector3Int.left,
                current + Vector3Int.down + Vector3Int.right})
            {
                // Check if the neighbor is not visited and the tile in Tilemap for that cell is null
                if (!visited.ContainsKey(neighbor) && tm.GetTile(neighbor) == null)
                {
                    // If it is not, add it to the queue and the visited dictionary
                    queue.Enqueue(neighbor);
                    visited[neighbor] = distance + 1;
                    this_map[neighbor] = distance + 1;
                }

            }

        }
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
