using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    public GameObject target;

    public static EnemyTarget _instance;
    public static EnemyTarget Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<EnemyTarget>();
            }

            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        // Subscribe to the GameManager's events
        GameManager.StartBuildPhaseEvent += OnBuildPhaseStart;

        GameManager.StartEnemyPhaseEvent += OnEnemyPhaseStart;
        GameManager.EndEnemyPhaseEvent += OnEnemyPhaseEnd;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the GameManager's events
        GameManager.StartBuildPhaseEvent -= OnBuildPhaseStart;

        GameManager.StartEnemyPhaseEvent -= OnEnemyPhaseStart;
        GameManager.EndEnemyPhaseEvent -= OnEnemyPhaseEnd;
    }
   // GameObject


    private void OnBuildPhaseStart()
    {
        StartCoroutine(moveEnemyTarget());
    }

    private IEnumerator moveEnemyTarget()
    {
        yield return new WaitForSeconds(0.1f); 
        Vector3Int currentCell = PathingMap.Instance.tm.WorldToCell(target.transform.position);
        PathingMap.Instance.tm.SetTile(currentCell, null);
        Debug.Log("EnemyTarget: Build phase started, moving to random location.");
        float randomX = Random.Range(PathingMap.Instance.x_lower_bound, PathingMap.Instance.x_upper_bound);
        float randomY = Random.Range(PathingMap.Instance.y_lower_bound, PathingMap.Instance.y_lower_bound + 2);
        Vector3Int randCell = PathingMap.Instance.tm.WorldToCell(new Vector3(randomX, randomY, 0));
        int max_rerolls = 50;
        int curr_rerolls = 0;
        while (true)
        {
            if (curr_rerolls >= max_rerolls)
            {
                break;
            }
            // If valid path exists to spot picked
            if (PathingMap.Instance.generateFlowField(randCell) == true)
            {
                // Spot picked is a null tile OR a barricade, then we'll accept replacing it.
                // But not unpathable_invis (tower) or pathable_invis (spawner)
                if (PathingMap.Instance.tm.GetTile(randCell) == null ||
                    PathingMap.Instance.tm.GetTile(randCell).name == "Dungeon_Tileset_v2_78")
                {
                    break;
                }
            }
            randomX = Random.Range(PathingMap.Instance.x_lower_bound, PathingMap.Instance.x_upper_bound);
            randomY = Random.Range(PathingMap.Instance.y_lower_bound, PathingMap.Instance.y_lower_bound + 2);
            randCell = PathingMap.Instance.tm.WorldToCell(new Vector3(randomX, randomY, 0));
            curr_rerolls++;
        }

        PathingMap.Instance.tm.SetTile(randCell, PathingMap.Instance.pathable_invis_tile);
        randomX = Mathf.Round(randomX) + 0.5f;  // Round and add 0.5 to get center of tile.
        randomY = Mathf.Round(randomY) + 0.5f;
        target.transform.position = new Vector3(randomX, randomY, 0);
    }

    private void OnEnemyPhaseStart()
    {
        // When the enemy phase starts, update the target and generate a flow field
        UpdateTargetAndGenerateFlowField();
    }

    private void OnEnemyPhaseEnd()
    {

    }

    private void UpdateTargetAndGenerateFlowField()
    {
        if(target != null)
        {
            Vector3Int targetCell = PathingMap.Instance.tm.WorldToCell(target.transform.position);
            PathingMap.Instance.generateFlowField(targetCell);
            Debug.Log("Target cell updated and flow field generated.");
        }
        else
        {
            Debug.LogWarning("Target is null. Cannot update target cell or generate flow field.");
        }
    }

    // why did i make this function
    // just call directly in enemybasic
    public void decrementLives(int howManyLives)
    {
        GameManager.Instance.decrementLives(howManyLives);
        //screen shake by setting trigger in animator
        FindFirstObjectByType(typeof(Camera)).GetComponent<Animator>().SetTrigger("shakeTrigger");
    }
}
