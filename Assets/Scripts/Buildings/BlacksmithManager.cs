using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithManager : MonoBehaviour
{
    public int level = 0;
    public int maxLevel = 3;

    [Space]

    public int[] woodCostPerLevel;
    public int[] stoneCostPerLevel;
    public int[] foodCostPerLevel;

    [Space]

    public int[] healthPerLevel;

    [Space]

    public Transform troopParent;

    public void UpdateTroopStats() {
        
        foreach (Transform child in troopParent) {
            Debug.Log(child);

            Health healthComponent = child.GetComponent<Health>();

            // Health should be updated proportional to the units current health percent
            //(e.g. if at half health before upgrading, upgrade to have half of the new limit)

            float healthPercent = healthComponent.health / healthComponent.maxHealth;

            healthComponent.maxHealth = healthPerLevel[level];
            healthComponent.health = healthPerLevel[level] * healthPercent;
        }

    }

}