using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarracksManager : MonoBehaviour
{

    // Placement cost
    public int[] cost;

    public int maxTroops;
    public int currentTroops = 0;

    public int troopsQueued = 0;

    float timeTraining = 0f;
    float trainTime = 3f;

    private void SpawnTroop()
    {
        Debug.Log("Troop Spawned");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (troopsQueued > 0)
        {
            timeTraining += Time.deltaTime;

            if (timeTraining > trainTime)
            {
                timeTraining -= trainTime;
                troopsQueued -= 1;
                SpawnTroop();
            }
        } else
        {
            timeTraining = 0f;
        }

    }
}
