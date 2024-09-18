using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDamager : MonoBehaviour
{
    public float damage = 25f;
    public string filterTag;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);

        if (other.transform.CompareTag(filterTag) == false)
        {
            Health healthComponent = other.GetComponent<Health>();
            healthComponent = healthComponent == null ? other.GetComponentInParent<Health>() : healthComponent;
            if (healthComponent != null)
            {
                Debug.Log(healthComponent.gameObject.name);
                healthComponent.TakeDamage(damage);
            }
        }
    }
}
