using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDamager : MonoBehaviour
{
    public float damage = 25f;

    private void OnTriggerEnter(Collider other)
    {
        Health healthComponent = other.GetComponent<Health>();

        if (healthComponent != null)
        {
            healthComponent.TakeDamage(damage);
        }
    }
}
