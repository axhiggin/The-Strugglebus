using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Norman Zhu 3/13/2024 3:11PM

public class AudioFxManager : MonoBehaviour
{
    public static AudioFxManager Instance;

    public GameObject audioFxObjectPrefab;
    private List<GameObject> objectPool;
    public int audioObjectsToPool = 10;

    public AudioClip bigExplosion;
    public float bigExplosionDuration;
    public AudioClip[] explosionSounds;
    public float[] explosionDuration;
    public AudioClip[] laserSounds;
    public float[] laserDuration;
    public AudioClip[] buildingSounds;
    public float[] buildingDuration;

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
        objectPool = new List<GameObject>();
    }

    public GameObject GetAudioObject()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].activeInHierarchy)
            {
                return objectPool[i];
            }
        }
        return null;
    }

    public void deactivateObjectAfterDelay(float duration, GameObject obj)
    {
        StartCoroutine(delayedDeactivation(duration, obj));
    }

    IEnumerator delayedDeactivation(float duration, GameObject obj)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
