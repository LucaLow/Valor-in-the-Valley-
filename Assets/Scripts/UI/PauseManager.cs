using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused = false;

    public GameObject pauseUI;
    public GameObject settingsUI;

    [Space]

    public CameraController cameraController;
    public CameraEffects cameraEffects;

    [Space]

    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public Slider FOVSlider;

    private void Start()
    {
        IsPaused = false;
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;

            if (IsPaused)
            {
                Time.timeScale = 0;
                pauseUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;

                volumeSlider.value = StaticVariables.Volume;
                sensitivitySlider.value = StaticVariables.MouseSensitivity;
                FOVSlider.value = StaticVariables.CameraFOV;
            }
            else
            {
                Time.timeScale = 1;
                pauseUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void Resume()
    {
        IsPaused = false;

        Time.timeScale = 1;
        pauseUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Settings()
    {
        settingsUI.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsUI.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void ChangeVolume()
    {
        StaticVariables.Volume = volumeSlider.value;
    }
    public void ChangeSensitivity()
    {
        StaticVariables.MouseSensitivity = sensitivitySlider.value;
        cameraController.horizontalSensitivity = StaticVariables.MouseSensitivity / 1.5f;
        cameraController.verticalSensitivity = StaticVariables.MouseSensitivity / 1.5f;
    }
    public void ChangeFOV()
    {
        float fov = FOVSlider.value;
        StaticVariables.CameraFOV = fov;

        cameraEffects.movementFOVAdjustmentsSettings.sprintingFOV = 70f * StaticVariables.CameraFOV / 85;
        cameraEffects.movementFOVAdjustmentsSettings.movingForwardFOV = 63f * StaticVariables.CameraFOV / 85;
        cameraEffects.movementFOVAdjustmentsSettings.movingBackwardFOV = 57f * StaticVariables.CameraFOV / 85;
        cameraEffects.movementFOVAdjustmentsSettings.idleFOV = 60f * StaticVariables.CameraFOV / 85;
    }
}
