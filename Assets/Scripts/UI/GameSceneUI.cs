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
    private Slider playerLives;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        roundCount = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        timeRemaining = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        playerLives = transform.GetChild(2).GetComponent<Slider>();
        playerLives.maxValue = GameManager.DEFAULT_LIVES;
        materialCount = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        materialCount.text = "Materials:       x" + player.GetComponent<PlayerBuild>().materialCount;
        playerLives.value = GameManager.Instance.getLivesRemaining();
        roundCount.text = "Round: " + GameManager.Instance.getLevelCount();
        timeRemaining.text = "Time: " + GameManager.Instance.getCurrentTimeRemaining();
    }
}
