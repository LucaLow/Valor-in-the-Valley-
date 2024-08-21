using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= Mathf.Abs(amount);
    }

    public void Heal(float amount)
    {
        health += Mathf.Abs(amount);
    }

    public float HealthPercentage()
    {
        return Mathf.InverseLerp(0, maxHealth, health);
    }

    public bool IsDead()
    {
        if (health <= 0)
        {
            return true;
        }

        return false;
    }
}
