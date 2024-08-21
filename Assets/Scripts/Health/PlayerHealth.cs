using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    public Slider healthBarSlider;
    public Gradient healthBarColors;

    private void Update()
    {
        float healthPercentage = HealthPercentage();
        healthBarSlider.value = Mathf.Lerp(healthBarSlider.value, healthPercentage, 10 * Time.deltaTime);
        healthBarSlider.targetGraphic.color = Color.Lerp(healthBarSlider.targetGraphic.color, healthBarColors.Evaluate(healthPercentage), 10 * Time.deltaTime);


        if (IsDead())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("Main Menu");
        }
    }
}
