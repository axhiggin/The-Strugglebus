using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public AudioClip gameSceneMusic;
    public AudioClip startSceneMusic;
    private AudioSource audioSource;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playAudioForScene();
    }

    void playAudioForScene(){
        Scene scene = SceneManager.GetActiveScene();
         switch (scene.name)
        {
            case "StartScene":
                PlayAudioClip(startSceneMusic);
                break;
            case "GameScene1":
                PlayAudioClip(gameSceneMusic);
                break;
            default:
                // Optional: stop playing audio or handle other scenes
                break;
        }
    }

    void PlayAudioClip(AudioClip clip)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.clip = clip;
        audioSource.Play();
    }

    void OnDestroy()
    {
        // Unsubscribe to ensure no memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void start(){
        playAudioForScene();
    }
}
