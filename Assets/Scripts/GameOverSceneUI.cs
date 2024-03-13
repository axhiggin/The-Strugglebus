using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverSceneUI : MonoBehaviour
{
    private Canvas canvas;
    private TextMeshProUGUI roundCount, zombiesKilled;
    private Button btn;

    void Start()
    {
        canvas = GetComponent<Canvas>();

        roundCount = canvas.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        roundCount.text = "You died after " + GameManager.Instance.getLevelCount() + " rounds";

        zombiesKilled = canvas.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        zombiesKilled.text = "You killed " + GameManager.Instance.totalZombiesKilled + " zombies";

        canvas.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(goToMenuScene);
        
        canvas.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(goToGameScene);
    }
    
    public void goToMenuScene()
    {
        SceneManager.LoadScene("StartScreen");
    }
    public void goToGameScene()
    {
        SceneManager.LoadScene("GameScene1");
    }
}
