using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPooler : MonoBehaviour
{
    public static BulletPooler Instance;
    private List<GameObject> objects;
    [SerializeField] GameObject bullet;
    [SerializeField] int numInPool;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //FROM UNITY DOCUMENTATION https://learn.unity.com/tutorial/introduction-to-object-pooling#5ff8d015edbc2a002063971d
        objects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < numInPool; i++)
        {
            tmp = Instantiate(bullet);
            tmp.SetActive(false);
            objects.Add(tmp);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < numInPool; i++)
        {
            if (!objects[i].activeInHierarchy)
            {
                return objects[i];
            }
        }
        return null;
    }
}
