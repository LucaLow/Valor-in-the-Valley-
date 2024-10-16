using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEnemy : MonoBehaviour
{
    public AIAgent agent;

    private void Start()
    {
        agent.wanderingState.wanderRangePosition = TownHallManager.Instance.transform.position;
    }
}
