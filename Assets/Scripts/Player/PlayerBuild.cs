//Norman Zhu 7:00PM 3/12/24. enemyTarget public field. Check pathing before build.
//Norman Zhu 9:36PM 3/7/24. Moved material count to player build.


//Worked on by: Aidan
//Norman Zhu 8:43PM 2/27/24. Moving Tilemap, tile references to a singleton.
    //                       Adding if statement conditionals to place/delete barrier.
    //                             Calls functions in PathingMap to recalculate flow field.
    //                       Added PlayerCell to send to PathingMap.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerBuild : MonoBehaviour
{
    // [SerializeField] Tilemap tm;
    // [SerializeField] Tile tile;
    [SerializeField] PlayerMovement pm;

    public int STARTING_MATERIAL_COUNT = 15;
    //maybe change accessability, just temporary
    public int materialCount;

    public GameObject enemyTarget;

    private void Start()
    {
        materialCount = STARTING_MATERIAL_COUNT;
        if (enemyTarget == null)
        {
            Debug.Log("ERROR: PlayerBuild.cs: enemyTarget is null. Please assign in inspector.");
        }
    }
    void LateUpdate()
    {
        //from https://forum.unity.com/threads/how-can-i-place-a-tile-in-a-tilemap-by-script.508338/
        Vector3Int playerCell = PathingMap.Instance.tm.WorldToCell(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z));
        Vector3Int currentCell = playerCell;

        //change direction of tile placement based off of movement script direction
        if (pm.currDir == PlayerMovement.direction.left)
        {
            currentCell.x -= 1;
        }
        else if (pm.currDir == PlayerMovement.direction.right)
        {
            currentCell.x += 1;
        }
        else if (pm.currDir == PlayerMovement.direction.up)
        {
            currentCell.y += 1;
        }
        else if (pm.currDir == PlayerMovement.direction.down)
        {
            currentCell.y -= 1;
        }

        //places a barrier                     ADD && GameManager.Instance.materialCount > 0 when actually testing
        if (Input.GetKeyDown(KeyCode.E) && materialCount>0)
        {
            // If the tile at currentCell in Tilemap tm is null, then place a tile and recalc flow field.
            if (PathingMap.Instance.tm.GetTile(currentCell) == null)
            {
                PathingMap.Instance.tm.SetTile(currentCell, PathingMap.Instance.tile);
                
                //change material count
                materialCount--;

                Vector3Int targetCell = PathingMap.Instance.tm.WorldToCell(enemyTarget.transform.position);
                if (PathingMap.Instance.generateFlowField(targetCell) == false)
                {
                    Debug.LogWarning(PathingMap.Instance.tm.GetTile(currentCell));
                    Debug.LogWarning("PlayerBuild: Failed to path to all enemySpawners, removing last created barricade.");
                    PathingMap.Instance.tm.SetTile(currentCell, null);
                    materialCount++;
                }
            }
            else
            {
                // Otherwise, don't recalculate flow field, because nothing was placed.

                // Play sound effect / particles here maybe.
                if (GameManager.DEBUG_MODE)
                    Debug.Log("Tile already exists at " + currentCell);

            }
        }

        //deletes a barrier
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (PathingMap.Instance.tm.GetTile(currentCell) == null)
            {
                return;
            }
            // If the tile at currentCell in Tilemap tm is not null, then delete the tile and recalc flow field.
            if (PathingMap.Instance.tm.GetTile(currentCell).name == "Dungeon_Tileset_v2_78")
            {
                Debug.Log("Name matched Dungeon_Tileset_v2_78");
                PathingMap.Instance.tm.SetTile(currentCell, null);
                //change material count
                materialCount++;
            }
        }
    }


}
