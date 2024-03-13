using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Norman - 3/6/24 3:27PM - added debug mode.
//                          spawns a square prefab at center of each tile traversed by the pathing algorithm.
//                              color brightness will be set to distance according to pathing algorithm.
//                          set DEBUG_MODE to true to activate.

// Norman - 2/27/24 8:53PM - Script created.
//                           Functions:
//                               - generateFlowField(Vector3Int start)
//                               - get_valid_diagonals(Vector3Int current)
//                           Variables:
//                               - x_lower_bound, x_upper_bound, y_lower_bound, y_upper_bound
//                                     Negative values OK. Define the edges of the grid using these variables.
//                               - MAX_DISTANCE
//                                 Constant. Unpathable cells are set to this value to avoid null pointer errors.x
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
    public GameObject endpoint;             // The endpoint for the enemies to reach.
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
    // INCLUSIVE BOUNDS. SET IN THE INSPECTOR. 
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
    public Tile tile;       // barricade tile
    [SerializeField]
    public Tile unpathable_invis_tile;  // transparent tile
    [SerializeField]
    public Tile pathable_invis_tile;  // transparent tile
    [SerializeField]
    public const bool DEBUG_MODE = true;   // PathingMap debug mode prints out squares on each tile
    [SerializeField]
    public GameObject debugSquare;
    private List<GameObject> debugPool;

    private EnemyManager enemMan;

    // List of flow fields (Dictionary<Vector3Int, int>) for each waypoint.
    //         Each dictionary indexed using Vector3Int of a cell, returns flow field value.
    //         Uses Vector3Int to pull return value directly from PlayerBuild.cs / Tilemap.WorldToCell
    public Dictionary<Vector3Int, float> flowMap; // DEPRECATED.
    public float[,] flowMapArray;

    public bool v3int_in_bounds(Vector3Int v3)
    {
        if (v3.x < x_lower_bound ||
            v3.x > x_upper_bound ||
            v3.y < y_lower_bound ||
            v3.y > y_upper_bound)
        {
            return false;
        } else
        {
            return true;
        }

    }

    // Making sure we're only iterating over the diagonals aren't blocked off on both cardinal sides,
    // By making a list of the valid_diagonals. 
    public List<Vector3Int> get_valid_diagonals(Vector3Int current)
    {
        List<Vector3Int> valid_diagonals = new List<Vector3Int>();

        // UP LEFT
        if (tileIsPathable(current + Vector3Int.up) || tileIsPathable(current + Vector3Int.left))
        {
            Vector3Int upleft = new Vector3Int(current.x - 1, current.y + 1, 0);
            if (v3int_in_bounds(upleft))
            {
                valid_diagonals.Add(upleft);
            }
        }
        // UP RIGHT
        if (tileIsPathable(current + Vector3Int.up) || tileIsPathable(current + Vector3Int.right))
        {
            Vector3Int upright = new Vector3Int(current.x + 1, current.y + 1, 0);
            if (v3int_in_bounds(upright))
            {
                valid_diagonals.Add(upright);
            }
        }
        // DOWN RIGHT
        if (tileIsPathable(current + Vector3Int.down) || tileIsPathable(current + Vector3Int.right))
        {
            Vector3Int downright = new Vector3Int(current.x + 1, current.y - 1, 0);
            if (v3int_in_bounds(downright))
            {
                valid_diagonals.Add(current + Vector3Int.down + Vector3Int.right);
            }
        }
        // DOWN LEFT
        if (tileIsPathable(current + Vector3Int.down) || tileIsPathable(current + Vector3Int.left))
        {
            Vector3Int downleft = new Vector3Int(current.x - 1, current.y - 1, 0);
            if (v3int_in_bounds(downleft)){
                valid_diagonals.Add(current + Vector3Int.down + Vector3Int.left);
            }
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

    private void spawn_debug_square (Vector3Int v3, float distance)
    {
        Vector3 tileCenter = new Vector3(v3.x + 0.5f, v3.y + 0.5f, 0f);
        GameObject square = Instantiate(debugSquare, tileCenter, Quaternion.identity);
        debugPool.Add(square);
        SpriteRenderer renderer = square.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            // Calculate color based on distance
            float brightness = Mathf.Clamp01(1f - distance / 10); // Assuming MAX_DISTANCE is the maximum possible distance
            Color color = new Color(brightness, brightness, brightness); // Set color with the same brightness for all channels

            // Set the color of the material
            renderer.material.color = color;
        }
    }

    public void generateFlowFieldToEndpoint()
    {
        if (endpoint != null)
        {
            Vector3 endpointPosition = endpoint.transform.position;
            Vector3Int endpointCell = tm.WorldToCell(endpointPosition);
            generateFlowField(endpointCell);
        }
        else
        {
            Debug.Log("attempted to generate flow field to null 'endpoint' gameobject in PathingMap.cs");
            Debug.Log("no flow field was generated.");
        }
    }

    // Use breadth first search from start to generate a flow field for the index of flowMaps
    public bool generateFlowField(Vector3Int start)
    {
        if (DEBUG_MODE)
        {
            foreach (GameObject square in debugPool)
            {
                Destroy(square);
            }
        }
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(start);

        // Create a dictionary for visited cells and their distance from start
        // Dictionary<Vector3Int, float> visited = new Dictionary<Vector3Int, float>(); ====================DEPRECATED ======================
        Debug.Log("attempting to create flow field for dimensions:");
        Debug.Log("rows: " + rows);
        Debug.Log("cols: " + cols);
        float[,] visitedArray = new float[rows, cols];

        // Initialize all values to signify unvisited.
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

        bool visitedAllSpawners = false;
        int visitedSpawners = 0;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            if (enemMan.vector3_matches_one_spawner(current))
            {
                visitedSpawners++;
            }

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
                // Ensure the neighbor's coordinates are within the inclusive bounds.
                if (v3int_in_bounds(neighbor))
                {
                    // Ensure the neighbor hasn't been visited by the algorithm yet.
                    if (visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] == -1)   // using -1 as a signifier for unvisited tiles.
                    {
                        // Ensure the neighbor is empty of other tiles.
                        if (tileIsPathable(neighbor))// if (tm.GetTile(neighbor) == null)
                        {   
                            // add it to the queue and the visited dictionary
                            queue.Enqueue(neighbor);
                            //visited[neighbor] = distance + 1;
                            visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] = distance + 1;

                            if (DEBUG_MODE)
                            {
                                spawn_debug_square(neighbor, distance + 1);
                            }
                        }
                        // Tile is not null, set it's value to MAX_DISTANCE + distance.
                        else
                        {
                            //visited[neighbor] = MAX_DISTANCE;
                            visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] = distance + MAX_DISTANCE;
                        }
                    }
                }
            }

            // Making sure we're only iterating over the diagonals aren't blocked off on both cardinal sides,
            // By making a list of the valid_diagonals. 
            List<Vector3Int> valid_diagonals = get_valid_diagonals(current);

            // Visit all diagonal neighbors of the current cell (distance increments by diagonal (sqrt2))
            foreach (Vector3Int neighbor in valid_diagonals)
            {
                // Ensure the neighbor's coordinates are within the inclusive bounds.
                if (v3int_in_bounds(neighbor))
                {
                    // Ensure the neighbor hasn't been visited by the algorithm yet.
                    if (visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] == -1)   // using -1 as a signifier for unvisited tiles.
                    {
                        // Ensure the neighbor is empty of other tiles.
                        if (tileIsPathable(neighbor)) // if (tm.GetTile(neighbor) == null)
                        {
                            // add it to the queue and the visited dictionary
                            queue.Enqueue(neighbor);
                            //visited[neighbor] = distance + sqrt2
                            visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] = distance + Mathf.Sqrt(2);
                            if (DEBUG_MODE)
                            {
                                spawn_debug_square(neighbor, distance + Mathf.Sqrt(2));
                            }
                        }
                        // Tile is not null, set it's value to MAX_DISTANCE + distance.
                        else
                        {
                            //visited[neighbor] = MAX_DISTANCE;
                            visitedArray[v3_to_array_y(neighbor), v3_to_array_x(neighbor)] = distance + MAX_DISTANCE;
                        }
                    }
                }
            }
        }

        if (visitedSpawners == enemMan.getSpawnerCount())
        {
            visitedAllSpawners = true;
        }

        if (visitedAllSpawners == true)
        {
            Debug.Log("Visited all spawners with this pass of generateFlowFIeld!");
            // Set the flow field for the start cell
            //flowMap = visited;
            flowMapArray = visitedArray;

        } else
        {
            Debug.Log("COULD NOT VISIT ALL SPAWNERS WITH THIS PASS OF GENERATEFLOWFIELD!");
            Debug.Log("SPAWNERS THAT EXIST: " + enemMan.getSpawnerCount());
            Debug.Log("SPAWNERS VISITED SUCCESSFULLY: " + visitedSpawners);
        }
        return visitedAllSpawners;
    }

    private bool tileIsPathable(Vector3Int tileCell)
    {
        if (tm.GetTile(tileCell) == null)
        {
            return true;
        }
        if (tm.GetTile(tileCell).name == "pathable_invis"){
            return true;
        }
        //if (tm.GetTile(tileCell).name != "unpathable_invis" &&
        //    tm.GetTile(tileCell).name != "Unpathable_invis" &&
        //    tm.GetTile(tileCell).name != "Dungeon_Tileset_v2_99" && 
        //    tm.GetTile(tileCell).name != "Dungeon_Tileset_v2_78")
        //{
        //    return true;
        //}
        //else
        //{
        return false;
        //}
    } 

    // Start is called before the first frame update
    void Start()
    {
        enemMan = FindObjectOfType<EnemyManager>();

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
        if (DEBUG_MODE)
        {
            debugPool = new List<GameObject>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}


//void tileBox()
//{
//    for (int i = y_lower_bound; i <= y_upper_bound + 1; ++i)
//    {
//        Vector3Int leftWall = new Vector3Int(x_lower_bound - 1, i, 0);
//        Vector3Int rightWall = new Vector3Int(x_upper_bound + 1, i, 0);
//        tm.SetTile(leftWall, rightfacingWall);
//        tm.SetTile(rightWall, leftfacingWall);
//    }
//    Vector3Int swCornerCoord = new Vector3Int(x_lower_bound - 1, y_lower_bound - 1, 0);
//    tm.SetTile(swCornerCoord, swCorner);
//    Vector3Int seCornerCoord = new Vector3Int(x_upper_bound + 1, y_lower_bound - 1, 0);
//    tm.SetTile(seCornerCoord, seCorner);

//    for (int i = x_lower_bound; i <= x_upper_bound; ++i)
//    {
//        Vector3Int topWall = new Vector3Int(i, y_upper_bound + 1, 0);
//        Vector3Int botWall = new Vector3Int(i, y_lower_bound - 1, 0);
//        tm.SetTile(topWall, downfacingWall);
//        tm.SetTile(botWall, upfacingWall);
//    }

//    Debug.Log("finished setting tilebox for walls");
//}