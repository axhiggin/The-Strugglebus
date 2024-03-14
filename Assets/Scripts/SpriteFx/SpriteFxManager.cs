using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Norman Zhu 3/12/2024 10:55PM
public class SpriteFxManager : MonoBehaviour
{
    public static SpriteFxManager Instance;

    // a pool for each type of sprite effect
    public int typesOfExplosions;
    public GameObject[] explosionPrefab;            
    public List<GameObject>[] explosionPool;          // Explosions.
    public float[] explosionDurations;
    public int explosionsToPool = 10;

    public GameObject smokePrefab;
    public List<GameObject> smokePool;
    public float smokeDuration;
    public int smokeToPool = 10;


    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        explosionPool = new List<GameObject>[typesOfExplosions];
        for (int i = 0; i < typesOfExplosions; i++)
        {
            CreateExplosionPool(i);
        }

        smokePool = new List<GameObject>();
        for (int i = 0; i < smokeToPool; i++)
        {
            GameObject obj = Instantiate(smokePrefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            smokePool.Add(obj);
        }
    }

    public void CreateExplosionPool(int index)
    {
        explosionPool[index] = new List<GameObject>();
        for (int i = 0; i < explosionsToPool; i++)
        {
            GameObject obj = Instantiate(explosionPrefab[index]);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            explosionPool[index].Add(obj);
        }
    }
    public GameObject GetPooledExplosion(int explosionType)
    {
        for (int i = 0; i < explosionPool[explosionType].Count; i++)
        {
            if (!explosionPool[explosionType][i].activeInHierarchy)
            {
                return explosionPool[explosionType][i];
            }
        }
        return null;
    }   

    public GameObject GetPooledSmoke()
    {
        for (int i = 0; i < smokePool.Count; i++)
        {
            if (!smokePool[i].activeInHierarchy)
            {
                return smokePool[i];
            }
        }
        return null;
    }

    public void deactivateSpriteAfterDelay(float duration, GameObject sprite)
    {
        StartCoroutine(delayedDeactivation(duration, sprite));
    }

    IEnumerator delayedDeactivation(float duration, GameObject sprite) {
        yield return new WaitForSeconds(duration);
        sprite.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
