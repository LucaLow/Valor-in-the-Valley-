using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public Health hp;
    public int health;
    public int maxHealth = 100;
    void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        hp.SetHealth(health);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if(health <= 0)
        {
            SceneManager.LoadScene("Main Menu");
        }
            
    }
}
