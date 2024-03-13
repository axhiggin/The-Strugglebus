using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectsVolume : MonoBehaviour
{
    private Slider volumeSlider;
    void Start()
    {
        volumeSlider = GetComponent<Slider>();
    }

    void Update()
    {
        GameManager.Instance.effectsVolume = volumeSlider.value;
    }
}
