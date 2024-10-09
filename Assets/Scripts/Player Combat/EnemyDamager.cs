using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamager : MonoBehaviour
{
    public float damage = 50f;
    private const string enemyTag = "Enemy";

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(enemyTag))
        {
            AgentHealth health = other.transform.GetComponentInParent<AgentHealth>();

            Debug.Log(health.health);

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}
