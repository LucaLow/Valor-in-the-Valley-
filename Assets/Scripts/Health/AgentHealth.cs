using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentHealth : Health
{
    public Image healthBar;
    public Gradient healthBarColors;

    private void Update()
    {
        float healthPercentage = HealthPercentage();
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, healthPercentage, 10 * Time.deltaTime);
        healthBar.color = Color.Lerp(healthBar.color, healthBarColors.Evaluate(healthPercentage), 10 * Time.deltaTime);

        if (IsDead())
            Destroy(gameObject);
    }
}
