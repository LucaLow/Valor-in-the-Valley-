using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
public class PlayerHealth : Health
{
    public Slider healthBarSlider;
    public Gradient healthBarColors;
    public WaveSystem waves;
    public string filePath = "Assets/Scripts/UI/leaderboard.txt";
    private void Update()
    {
        float healthPercentage = HealthPercentage();
        healthBarSlider.value = Mathf.Lerp(healthBarSlider.value, healthPercentage, 10 * Time.deltaTime);
        healthBarSlider.targetGraphic.color = Color.Lerp(healthBarSlider.targetGraphic.color, healthBarColors.Evaluate(healthPercentage), 10 * Time.deltaTime);

        if (IsDead())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            int wave = waves.waveNumber;
           // using(StreamWriter sw = File.AppendText("C:\\leaderboard.text"))
           // {
           //     sw.Write("anon, " + wave);
           // }

            SceneManager.LoadScene("Main Menu");
        }
    }
}
