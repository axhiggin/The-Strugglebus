using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioClip gameSceneMusic;
    public AudioClip startScreenMusic;
    public AudioClip gameOverMusic;
    private AudioSource audioSource;

    private static AudioManager _instance;

    public static AudioManager Instance { get { return _instance; } }


    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayAudioForScene();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayAudioForScene();
    }
void PlayAudioForScene()
{
    Scene scene = SceneManager.GetActiveScene();
    Debug.Log(scene.name);
    switch (scene.name)
    {
        case "StartScreen":
            Debug.Log("Start Scene Music should play now: " + startScreenMusic.name);
            PlayAudioClip(startScreenMusic);
            break;
        case "GameScene1":
            Debug.Log("Game Scene 1 Music should play now: " + gameSceneMusic.name);
            PlayAudioClip(gameSceneMusic);
            break;
        case "GameOverScene":
            Debug.Log("Game Scene 1 Music should play now: " + gameOverMusic.name);
            PlayAudioClip(gameOverMusic);
            break;
        default:
            Debug.Log("No matching scene found for audio playback.");
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
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Renamed from start() to Start() to follow C# naming conventions
    void Start()
    {
        //PlayAudioForScene();
    }
}
