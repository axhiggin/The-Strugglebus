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
    void LateUpdate()
    {
        //from https://forum.unity.com/threads/how-can-i-place-a-tile-in-a-tilemap-by-script.508338/
        Vector3Int playerCell = PathingMap.Instance.tm.WorldToCell(transform.position);
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            // If the tile at currentCell in Tilemap tm is null, then place a tile and recalc flow field.
            if (PathingMap.Instance.tm.GetTile(currentCell) == null)
            {
                PathingMap.Instance.tm.SetTile(currentCell, PathingMap.Instance.tile);
                PathingMap.Instance.generateFlowField(playerCell);
                //change material count
                GameManager.Instance.materialCount--;
            }
            else
            {
                // Otherwise, don't recalculate flow field, because nothing was placed.
                Debug.Log("Tile already exists at " + currentCell);
                // Play sound effect / particles here maybe.

            }
        }

        //deletes a barrier
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // If the tile at currentCell in Tilemap tm is not null, then delete the tile and recalc flow field.
            if (PathingMap.Instance.tm.GetTile(currentCell) != null)
            {
                PathingMap.Instance.tm.SetTile(currentCell, null);
                PathingMap.Instance.generateFlowField(playerCell);
                //change material count
                GameManager.Instance.materialCount++;
            }
            else
            {
                // Otherwise, don't recalculate flow field, because nothing was deleted.
                Debug.Log("No tile to delete at " + currentCell);
                // Play sound effect / particles here maybe.
            }
            PathingMap.Instance.tm.SetTile(currentCell, null);
        }
    }
}
