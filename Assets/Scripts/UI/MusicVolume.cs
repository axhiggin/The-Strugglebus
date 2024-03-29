using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolume : MonoBehaviour
{
    private Slider volumeSlider;
    void Start()
    {
        volumeSlider = GetComponent<Slider>();
        volumeSlider.maxValue = AudioManager.Instance.maxVolume;
    }

    void Update()
    {
        AudioManager.Instance.setVolume(volumeSlider.value);
    }
}
