using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
// using UnityEditor.SearchService;
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
        canvas.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(toOptions);
        canvas.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(toCredits);
    }

    void toGame()
    {
        SceneManager.LoadScene("GameScene1");
    }

    void toOptions()
    {
        SceneManager.LoadScene("OptionsScene");
    }

    void toCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }
}
