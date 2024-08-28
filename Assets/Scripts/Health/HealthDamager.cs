using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDamager : MonoBehaviour
{
    public float damage = 25f;
    public string filterTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(filterTag) == false)
        {
            print(transform.parent.name + "    " + other.transform.parent.name);

            Health healthComponent = other.GetComponent<Health>();
            healthComponent = healthComponent == null ? other.GetComponentInParent<Health>() : null;
            if (healthComponent != null)
            {
                healthComponent.TakeDamage(damage);
            }
        }
    }
}
