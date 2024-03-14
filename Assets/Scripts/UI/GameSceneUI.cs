using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUI : MonoBehaviour
{
    private Canvas canvas;
    [SerializeField] GameObject player;
    private TextMeshProUGUI roundCount, timeRemaining, materialCount;
    private Transform playerLives;
    private Slider healthSlider, easeSlider;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        roundCount = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        timeRemaining = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        materialCount = transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        //HEALTH BAR https://www.youtube.com/watch?v=3JjBJfoWDCM
        playerLives = transform.GetChild(2);
        healthSlider = playerLives.Find("HealthBar").GetComponent<Slider>();
        easeSlider = playerLives.Find("EaseHealth").GetComponent<Slider>();
        healthSlider.maxValue = GameManager.DEFAULT_LIVES;
        easeSlider.maxValue = GameManager.DEFAULT_LIVES;
    }

    private void Update()
    {
        materialCount.text = "Materials:       x" + player.GetComponent<PlayerBuild>().materialCount;
        //playerLives.value = GameManager.Instance.getLivesRemaining();
        roundCount.text = "Round: " + GameManager.Instance.getLevelCount();
        timeRemaining.text = "Time: " + GameManager.Instance.getCurrentTimeRemaining();

        //HEALTH BAR https://www.youtube.com/watch?v=3JjBJfoWDCM
        healthSlider.value = GameManager.Instance.getLivesRemaining();
        if(healthSlider.value != easeSlider.value)
        {
            easeSlider.value = Mathf.Lerp(easeSlider.value, healthSlider.value, 0.03f);

        }
    }
}
