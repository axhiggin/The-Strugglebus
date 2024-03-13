using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreenUI : MonoBehaviour
{
    private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(toGame);
        //ADD THE REST HERE
    }

    void toGame()
    {
        SceneManager.LoadScene("GameScene1");
    }
}
