using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Last edited by Guy Haiby 3/10 1:00 Am
//Manage Turret spawning, group behavior
public class TurretManager : MonoBehaviour{
    [SerializeField] GameObject turretPrefab; //turret prefab to spawn
    [SerializeField] Tilemap groundTilemap; //tilemap of scene
    [SerializeField] int numTurrets;



    private void Start()
    {
        SpawnTurrets();
    }

    //spawn n number of turrets inside the bounds restricted by the bounds of the tilemap 
    private void SpawnTurrets()
    {
        BoundsInt bounds = groundTilemap.cellBounds;
        TileBase[] allTiles = groundTilemap.GetTilesBlock(bounds);

        for (int i = 0; i < numTurrets; i++)
        {   
            //get random coordinates
            Vector3Int randomTilePosition = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                bounds.z);

            //get tile at those coord
            TileBase tile = allTiles[randomTilePosition.x - bounds.xMin + (randomTilePosition.y - bounds.yMin) * bounds.size.x];

            //spwan turret at position
            if (tile != null)
            {
                Vector3 spawnPosition = groundTilemap.GetCellCenterWorld(randomTilePosition);
                Instantiate(turretPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}