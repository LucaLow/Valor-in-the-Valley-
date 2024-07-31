using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentHealth : MonoBehaviour
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

    public bool IsDead()
    {
        if (health <= 0)
        {
            return true;
        }

        return false;
    }
}
