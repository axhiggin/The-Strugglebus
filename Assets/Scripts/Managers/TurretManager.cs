using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Norman Zhu 3/12/2024 4:03PM.     turretsList, destroyTurrets, spawnTurrets
//                                  spawn/destroy logic subscribed to GameManager events

//Last edited by Guy Haiby 3/10 1:00 Am
//Manage Turret spawning, group behavior
public class TurretManager : MonoBehaviour{

    [SerializeField] GameObject turretPrefab; //turret prefab to spawn
    [SerializeField] int numTurrets;
    [SerializeField] int MAX_TURRETS;

    private List<GameObject> turretsList;



    private void Start()
    {
        // SpawnTurrets();

        GameManager.StartBuildPhaseEvent += spawnTurrets;
        // GameManager.EndEnemyPhaseEvent += destroyTurrets;
        turretsList = new List<GameObject>();
        turretsList.Clear();
    }

    private void OnDestroy()
    {
        GameManager.StartBuildPhaseEvent -= spawnTurrets;
    }

    private void destroyTurrets()
    {
        Debug.Log("destroyTurrets called");
        foreach (GameObject turret in turretsList)
        {
            Vector3Int tilePosInt = PathingMap.Instance.tm.WorldToCell(turret.transform.position);
            PathingMap.Instance.tm.SetTile(tilePosInt, null);
            Destroy(turret);
        }

        turretsList.Clear();
    }

    private void spawnTurrets()
    {
        Debug.Log("spawnTurrets called");
        if (turretsList.Count >= MAX_TURRETS)
        {
            return;
        }
        for (int i = 0; i < numTurrets; ++i)
        {
            Vector3 randomTilePosition = getRandomTurretCoords();
            Vector3Int tilePosInt = PathingMap.Instance.tm.WorldToCell(randomTilePosition);

            int max_rerolls = 50;
            int current_reroll = 0;
            while (true)
            {
                // check if tile is empty, if not, reroll.
                if (isValidTowerTile(tilePosInt))
                    break;
                if (current_reroll >= max_rerolls)
                    break;
                randomTilePosition = getRandomTurretCoords();
                tilePosInt = PathingMap.Instance.tm.WorldToCell(randomTilePosition);
                current_reroll++;
            }

            if (isValidTowerTile(tilePosInt))
            {
                Debug.Log("Spawning turret");
                GameObject newTurret = Instantiate(turretPrefab, randomTilePosition, Quaternion.identity);
                PathingMap.Instance.tm.SetTile(tilePosInt, PathingMap.Instance.unpathable_invis_tile);
                turretsList.Add(newTurret);
            }
        }
    }

    private Vector3 getRandomTurretCoords()
    {
        float xCoord = Random.Range(PathingMap.Instance.x_lower_bound, PathingMap.Instance.x_upper_bound) + 0.5f;
        float yCoord = Random.Range(PathingMap.Instance.y_lower_bound + 2, PathingMap.Instance.y_upper_bound - 2) + 0.5f;
        return new Vector3(xCoord, yCoord, 0);
    }

    // Empty tiles and barricade tiles are valid.
    private bool isValidTowerTile(Vector3Int tilePosInt)
    {
        if (PathingMap.Instance.generateFlowField(tilePosInt) == false){
            return false;
        }
        if (PathingMap.Instance.tm.GetTile(tilePosInt) == null || 
            PathingMap.Instance.tm.GetTile(tilePosInt).name == "Dungeon_Tileset_v2_78")
        {
            return true;
        }
        return false;
    }

    //spawn n number of turrets inside the bounds restricted by the bounds of the tilemap 
    //private void SpawnTurrets()
    //{
    //    BoundsInt bounds = groundTilemap.cellBounds;
    //    TileBase[] allTiles = groundTilemap.GetTilesBlock(bounds);

    //    for (int i = 0; i < numTurrets; i++)
    //    {   
    //        //get random coordinates
    //        Vector3Int randomTilePosition = new Vector3Int(
    //            Random.Range(bounds.xMin, bounds.xMax),
    //            Random.Range(bounds.yMin, bounds.yMax),
    //            bounds.z);

    //        //get tile at those coord
    //        TileBase tile = allTiles[randomTilePosition.x - bounds.xMin + (randomTilePosition.y - bounds.yMin) * bounds.size.x];

    //        //spwan turret at position
    //        if (tile != null)
    //        {
    //            Vector3 spawnPosition = groundTilemap.GetCellCenterWorld(randomTilePosition);
    //            Instantiate(turretPrefab, spawnPosition, Quaternion.identity);
    //        }
    //    }
    //}
}