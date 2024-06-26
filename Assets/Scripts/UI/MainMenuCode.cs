using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCode : MonoBehaviour
{
    public void StartGameBtn()
    {
        print("Start game");
        SceneManager.LoadScene("Unity Mini Game Project");
    }

    public void ExitGameBtn()
    {
        Application.Quit();
    }
}
