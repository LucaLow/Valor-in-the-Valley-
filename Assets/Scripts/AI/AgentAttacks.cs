using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAttacks : MonoBehaviour
{
    public AIAgent agent;
    public Animator animator;

    public string attackTrigger;

    private void Update()
    {
        if (agent.brainState == AIAgent.BrainState.attacking)
        {
            animator.SetBool(attackTrigger, true);
        }
        else
        {
            animator.SetBool(attackTrigger, false);
        }
    }
}
