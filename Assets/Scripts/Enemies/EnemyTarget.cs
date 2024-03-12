using System.Collections;
using System.Collections.Generic;
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
        Debug.Log("EnemyTarget: Build phase started, moving to random location.");
        float randomX = Random.Range(PathingMap.Instance.x_lower_bound, PathingMap.Instance.x_upper_bound);
        float randomY = Random.Range(PathingMap.Instance.y_lower_bound, PathingMap.Instance.y_lower_bound + 2);

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
    }
}
