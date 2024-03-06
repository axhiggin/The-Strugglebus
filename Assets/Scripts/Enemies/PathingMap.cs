using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Norman - 2/27/24 8:53PM - Script created.
//                           Functions:
//                               - generateFlowField(Vector3Int start)
//                               - get_valid_diagonals(Vector3Int current)
//                           Variables:
//                               - x_lower_bound, x_upper_bound, y_lower_bound, y_upper_bound
//                                     Negative values OK. Define the edges of the grid using these variables.
//                               - MAX_DISTANCE
//                                 Constant. Unpathable cells are set to this value to avoid null pointer errors.
//                                 Make sure it's higher than the max possible distance.
//                               - tm, tile
//                                 Serialize field references to prefabs for the tileMap.
//                               - flowMap
//                                     Dictionary<Vector3Int, float>
//                                     Index using Vector3Int, returns flow field value (float).
//                                     EnemyBasic.cs will move in direction of lowest neighboring flow field value.

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
    // INCLUSIVE BOUNDS
    public int x_lower_bound = -8;
    public int x_upper_bound = 7;
    public int y_lower_bound = -4;
    public int y_upper_bound = 3;

    private int rows = 0;
    private int cols = 0;

    public float MAX_DISTANCE = 100000.0f;
    [SerializeField]
    public Tilemap tm;
    [SerializeField]
    public Tile tile;
    [SerializeField]
    public Tile seCorner;
    [SerializeField]
    public Tile swCorner;
    [SerializeField]
    public Tile upfacingWall;
    [SerializeField]
    public Tile downfacingWall;
    [SerializeField]
    public Tile rightfacingWall;
    [SerializeField]
    public Tile leftfacingWall;

    // List of flow fields (Dictionary<Vector3Int, int>) for each waypoint.
    //         Each dictionary indexed using Vector3Int of a cell, returns flow field value.
    //         Uses Vector3Int to pull return value directly from PlayerBuild.cs / Tilemap.WorldToCell
    public Dictionary<Vector3Int, float> flowMap; // DEPRECATED.
    public float[,] flowMapArray;

    // Making sure we're only iterating over the diagonals aren't blocked off on both cardinal sides,
    // By making a list of the valid_diagonals. 
    private List<Vector3Int> get_valid_diagonals(Vector3Int current)
    {
        List<Vector3Int> valid_diagonals = new List<Vector3Int>();
        if (tm.GetTile(current + Vector3Int.up) == null || tm.GetTile(current + Vector3Int.left) == null)
        {
            valid_diagonals.Add(current + Vector3Int.up + Vector3Int.left);
        }
        if (tm.GetTile(current + Vector3Int.up) == null || tm.GetTile(current + Vector3Int.right) == null)
        {
            valid_diagonals.Add(current + Vector3Int.up + Vector3Int.right);
        }
        if (tm.GetTile(current + Vector3Int.down) == null || tm.GetTile(current + Vector3Int.right) == null)
        {
            valid_diagonals.Add(current + Vector3Int.down + Vector3Int.right);
        }
        if (tm.GetTile(current + Vector3Int.down) == null || tm.GetTile(current + Vector3Int.left) == null)
        {
            valid_diagonals.Add(current + Vector3Int.down + Vector3Int.left);
        }
        return valid_diagonals;
    }

    private int v3_to_array_x(Vector3Int v3)
    {
        return v3.x - x_lower_bound;
    }

    private int v3_to_array_y(Vector3Int v3)
    {
        return v3.y - y_lower_bound;
    }

    // Use breadth first search from start to generate a flow field for the index of flowMaps
    public void generateFlowField(Vector3Int start)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(start);

        // Create a dictionary for visited cells and their distance from start
        // Dictionary<Vector3Int, float> visited = new Dictionary<Vector3Int, float>(); ====================DEPRECATED ======================
        Debug.Log("attempting to create flow field for dimensions:");
        Debug.Log("rows: " + rows);
        Debug.Log("cols: " + cols);
        float[,] visitedArray = new float[rows, cols];

        // Initialize all values 
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                visitedArray[i, j] = -1;
            }
        }

        //visited[start] = 0;
        visitedArray[v3_to_array_y(start), v3_to_array_x(start)] = 0;
        //visitedFlag[v3_to_array_y(start), v3_to_array_x(start)] = true;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            // Grab distance as current node distance.
            float distance = visitedArray[v3_to_array_y(current), v3_to_array_x(current)];

            // Visit all adjacent neighbors of the current cell (distance increments by 1)
            //       as defined by current cell + up/down/left/right
            foreach (Vector3Int neighbor in new Vector3Int[] {
                current + Vector3Int.up,
                current + Vector3Int.down,
                current + Vector3Int.left,
                current + Vector3Int.right})
            {
                // Check if the neighbor is not visited and the tile in Tilemap for that cell is null
                //if (!visited.ContainsKey(neighbor) && tm.GetTile(neighbor) == null) ==================DEPRECATED =============================
                if (visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] == -1 && tm.GetTile(neighbor) == null)   // using -1 as a signifier for unvisited tiles.
                {
                    // Check that it is within the defined bounds of the map.
                    if (neighbor.x < x_upper_bound && neighbor.x > x_lower_bound && neighbor.y < y_upper_bound && neighbor.y > y_lower_bound)
                    {
                        // add it to the queue and the visited dictionary
                        queue.Enqueue(neighbor);
                        //visited[neighbor] = distance + 1;
                        visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] = distance + 1;
                    }
                }
                // If we're not enqueueing it, and it's not already visited, then set it to MAX_DISTANCE.
                //else if (!visited.ContainsKey(neighbor)) deprecated.
                else if (visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] == -1)
                {
                    //visited[neighbor] = MAX_DISTANCE;
                    visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] = MAX_DISTANCE;
                }
            }

            // Making sure we're only iterating over the diagonals aren't blocked off on both cardinal sides,
            // By making a list of the valid_diagonals. 
            List<Vector3Int> valid_diagonals = get_valid_diagonals(current);

            // Visit all diagonal neighbors of the current cell (distance increments by diagonal (sqrt2))
            foreach (Vector3Int neighbor in valid_diagonals)
            {
                // Check if the neighbor is not visited and the tile in Tilemap for that cell is null
                //if (!visited.ContainsKey(neighbor) && tm.GetTile(neighbor) == null)
                if (visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] == -1 && tm.GetTile(neighbor) == null)
                {
                    // PLACEHOLDER.
                    // Check that it is within the defined bounds of the map.
                    if (neighbor.x < x_upper_bound && neighbor.x > x_lower_bound && neighbor.y < y_upper_bound && neighbor.y > y_lower_bound)
                    {
                        // add it to the queue and the visited dictionary
                        queue.Enqueue(neighbor);
                        //visited[neighbor] = distance + Mathf.Sqrt(2);
                        visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] = distance + Mathf.Sqrt(2);
                    }
                }
                // If we're not enqueueing it, and it's not already visited, then set it to MAX_DISTANCE.
                // else if (!visited.ContainsKey(neighbor))
                else if (visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] == -1)
                {
                    //visited[neighbor] = MAX_DISTANCE;
                    visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] = MAX_DISTANCE;
                }
            }
        }

        // Set the flow field for the start cell
        //flowMap = visited;
        flowMapArray = visitedArray;
    }

    void tileBox()
    {
        for (int i = y_lower_bound; i <= y_upper_bound + 1; ++i)
        {
            Vector3Int leftWall = new Vector3Int(x_lower_bound - 1, i, 0);
            Vector3Int rightWall = new Vector3Int(x_upper_bound + 1, i, 0);
            tm.SetTile(leftWall, rightfacingWall);
            tm.SetTile(rightWall, leftfacingWall);
        }
        Vector3Int swCornerCoord = new Vector3Int(x_lower_bound - 1, y_lower_bound - 1, 0);
        tm.SetTile(swCornerCoord, swCorner);
        Vector3Int seCornerCoord = new Vector3Int(x_upper_bound + 1, y_lower_bound - 1, 0);
        tm.SetTile(seCornerCoord, seCorner);

        for (int i = x_lower_bound; i <= x_upper_bound; ++i)
        {
            Vector3Int topWall = new Vector3Int(i, y_upper_bound + 1, 0);
            Vector3Int botWall = new Vector3Int(i, y_lower_bound - 1, 0);
            tm.SetTile(topWall, downfacingWall);
            tm.SetTile(botWall, upfacingWall);
        }

        Debug.Log("finished setting tilebox for walls");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the row and col variables to use for initializing the array.
        cols = (x_upper_bound - x_lower_bound) + 1;
        rows = (y_upper_bound - y_lower_bound) + 1;

        // initialize flow map
        // flowMap = new Dictionary<Vector3Int, float>();
        flowMapArray = new float[rows, cols];

        //for (int i = x_lower_bound; i < x_upper_bound; i++)
        //{
        //    for (int j = y_lower_bound; j < y_upper_bound; j++)
        //    {
        //        flowMap[new Vector3Int(i, j, 0)] = 0;
        //    }
        //}
        GameManager.StartBuildPhaseEvent += tileBox;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
