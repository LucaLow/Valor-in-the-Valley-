using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCode : MonoBehaviour
{
    public GameObject settingsPage;
    public GameObject mainPage;

    public Slider FOVSlider;
    public Slider SensitvitiySlider;
    public Slider VolumeSlider;


    public void StartGameBtn()
    {
        print("Start game");
        SceneManager.LoadScene("CurrentMainScene");
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

    public void ChangeVolume()
    {
        StaticVariables.Volume = VolumeSlider.value;
    }
    public void ChangeSensitivity()
    {
        StaticVariables.MouseSensitivity = SensitvitiySlider.value;
    }
    public void ChangeFOV()
    {
        float fov = FOVSlider.value;
        StaticVariables.CameraFOV = fov;
    }
}
