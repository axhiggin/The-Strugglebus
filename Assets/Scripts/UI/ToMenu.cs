using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToMenu : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(goToMenu);
    }

    void goToMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }
}
