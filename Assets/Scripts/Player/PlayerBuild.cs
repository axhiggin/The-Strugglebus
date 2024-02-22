//Worked on by: Aidan
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerBuild : MonoBehaviour
{
    [SerializeField] Tilemap tm;
    [SerializeField] Tile tile;
    [SerializeField] PlayerMovement pm;
    void LateUpdate()
    {
        //from https://forum.unity.com/threads/how-can-i-place-a-tile-in-a-tilemap-by-script.508338/
        Vector3Int currentcell = tm.WorldToCell(transform.position);

        //change direction of tile placement based off of movement script direction
        if (pm.currDir == PlayerMovement.direction.left)
        {
            currentcell.x -= 1;
        }
        else if (pm.currDir == PlayerMovement.direction.right)
        {
            currentcell.x += 1;
        }
        else if (pm.currDir == PlayerMovement.direction.up)
        {
            currentcell.y += 1;
        }
        else if (pm.currDir == PlayerMovement.direction.down)
        {
            currentcell.y -= 1;
        }

        //places a barrier
        if (Input.GetKeyDown(KeyCode.E))
        {
            tm.SetTile(currentcell, tile);
        }

        //deletes a barrier
        if (Input.GetKeyDown(KeyCode.Q))
        {
            tm.SetTile(currentcell, null);
        }
    }
}
