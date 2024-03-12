using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager _instance;

    private GameObject[] bloodSplatPool;
    [SerializeField] private GameObject bloodSplatPrefab;
    [SerializeField] private int bloodSplatPoolSize = 10;

    public static ParticleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ParticleManager>();
            }

            return _instance;
        }
    }


    

    // Start is called before the first frame update
    void Start()
    {
        // Set up object pooling for blood splats
        bloodSplatPool = new GameObject[bloodSplatPoolSize];
        for (int i = 0; i < bloodSplatPoolSize; i++)
        {
            bloodSplatPool[i] = Instantiate(bloodSplatPrefab, Vector3.zero, Quaternion.identity);
            bloodSplatPool[i].SetActive(false);
            bloodSplatPool[i].transform.SetParent(this.transform);
        }
    }

    public void spawnBloodSplat(Vector3 spawnPosition, Vector3 lookingPosition, float duration)
    {
        if (getBloodSplatFromPool() != null)
        {
            GameObject bloodSplat = getBloodSplatFromPool();
            bloodSplat.transform.position = spawnPosition;
            bloodSplat.transform.LookAt(lookingPosition);
            bloodSplat.SetActive(true);
            StartCoroutine(destroyParticle(bloodSplat, duration));
        }
    }

    private IEnumerator destroyParticle(GameObject particle, float duration)
    {
        yield return new WaitForSeconds(duration);
        // set inactive for object pool
        particle.SetActive(false);
    }

    private GameObject getBloodSplatFromPool()
    {
        foreach (GameObject bloodSplat in bloodSplatPool)
        {
            if (!bloodSplat.activeInHierarchy)
            {
                return bloodSplat;
            }
        }
        return null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }


}
