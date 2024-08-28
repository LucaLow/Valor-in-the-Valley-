using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCode : MonoBehaviour
{
    public GameObject settingsPage;
    public GameObject mainPage;
    public void StartGameBtn()
    {
        print("Start game");
        SceneManager.LoadScene("Backup");
    }

    public void ExitGameBtn()
    {
        Application.Quit();
    }

    public void settingsPageOpenBtn()
    {
        settingsPage.SetActive(true);
        mainPage.SetActive(false);
    }

    public void settingsPageCloseBtn()
    {
        settingsPage.SetActive(false);
        mainPage.SetActive(true);
    }
}
