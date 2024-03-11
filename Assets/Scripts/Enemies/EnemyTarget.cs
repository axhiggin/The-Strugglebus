using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to the GameManager's events
        GameManager.StartEnemyPhaseEvent += OnEnemyPhaseStart;
        GameManager.EndEnemyPhaseEvent += OnEnemyPhaseEnd;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the GameManager's events
        GameManager.StartEnemyPhaseEvent -= OnEnemyPhaseStart;
        GameManager.EndEnemyPhaseEvent -= OnEnemyPhaseEnd;
    }
   // GameObject

    private void OnEnemyPhaseStart()
    {
        // When the enemy phase starts, update the target and generate a flow field
        UpdateTargetAndGenerateFlowField();
    }

    private void OnEnemyPhaseEnd()
    {
        float randomX = Random.Range(-7f, 7f);
        float randomY = Random.Range(-3f, 3f);
        target.transform.position = new Vector3(randomX, randomY, 0);
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
}
