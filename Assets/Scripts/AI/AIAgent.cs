using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIAgent : MonoBehaviour
{
    public enum AgentType { Melee, Ranged }
    public enum BrainState { wandering, pursuing, attacking, retreating }
    public enum MovementState { idle, walking, running, attacking }

    [Tooltip("The type of agent that this AI will adopt.")]
    public AgentType agentType = AgentType.Melee;

    [Space]

    [Tooltip("The state of the brain of the agent.")]
    public BrainState brainState = BrainState.wandering;
    [Tooltip("the state of the agents movement (idle, walking, running, attacking)")]
    public MovementState movementState { get; private set; }

    [Space]

    [Tooltip("The tag(s) of gameobjects that which this agent will be hostile towards. Priority goes from top to bottom")]
    public List<string> hostileTags;


    public WanderingStateSettings wanderingState;
    public PursuingStateSettings pursuingState;
    public MeleeAttackingStateSettings meleeAttackingState;
    public RangeAttackingStateSettings rangeAttackingState;

    [Space]

    public GizmoSettings gizmoSettings;

    private NavMeshAgent agent;

    private void Start()
    {
        wanderingState.wanderRangePosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        UpdateState();

        switch (brainState)
        {
            case BrainState.wandering:
                FacilitateWander();
                break;
            case BrainState.pursuing:
                FacilitatePursue();
                break;
            case BrainState.attacking:
                FacilitateAttacks();
                break;
            case BrainState.retreating:
                FacilitateRetreat();
                break;
        }

        UpdateRotation();
    }

    public void UpdateState()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, pursuingState.visionRange);
        List<GameObject> targets = new List<GameObject>();

        // Create the list of targets
        // The targets are objects with a specified tag in the range
        for (int i = 0; i < objectsInRange.Length; i++)
        {
            foreach (string tag in hostileTags)
            {
                if (objectsInRange[i].CompareTag(tag))
                {
                    targets.Add(objectsInRange[i].gameObject);
                }
            }
        }

        // Stop any attacking coroutine(s)
        if (brainState != BrainState.attacking)
        {
            if (rangeAttackingState.attackCoroutine != null)
            {
                StopCoroutine(rangeAttackingState.attackCoroutine);
                rangeAttackingState.attackCoroutine = null;
            }
        }

        if (targets.Count > 0)
        {
            // Sort the list of targets by distance
            targets = SortList(targets);


            // Default to pursuing state
            if (brainState != BrainState.retreating)
                brainState = BrainState.pursuing;
            pursuingState.target = targets[0].transform;


            switch (agentType)
            {
                // Melee behaviour
                case AgentType.Melee:

                    meleeAttackingState.target = targets[0].transform;

                    if (Vector3.Distance(transform.position, targets[0].transform.position) <= meleeAttackingState.attackRange)
                    {
                        // Attacking state
                        brainState = BrainState.attacking;
                    }

                    break;

                // Ranged behaviour
                case AgentType.Ranged:

                    rangeAttackingState.target = targets[0].transform;

                    if (Vector3.Distance(transform.position, targets[0].transform.position) <= rangeAttackingState.attackRange)
                    {
                        if (Vector3.Distance(transform.position, targets[0].transform.position) <= rangeAttackingState.retreatDistance)
                        {
                            // Agent is too close to enemy, so retreat
                            brainState = BrainState.retreating;
                        }
                        else
                        {
                            if (brainState == BrainState.retreating)
                            {
                                // Keep retreating until a safe distance away
                            }
                            else
                            {
                                // Switch to attacking state
                                brainState = BrainState.attacking;
                            }
                        }
                    }
                    else
                    {
                        // Stop retreating
                        if (brainState == BrainState.retreating)
                            brainState = BrainState.pursuing;
                    }

                    break;
            }
            return;
        }

        // Default to wandering state
        brainState = BrainState.wandering;
    }

    public List<GameObject> SortList(List<GameObject> list)
    {
        // Use LINQ to sort the list based on distance from referenceTransform
        List<GameObject> sortedList = list.OrderBy(go => Vector3.Distance(go.transform.position, transform.position)).ToList();

        return sortedList;
    }

    public void FacilitateWander()
    {
        // Make the stopping distance minimal
        // Not zero because then the agent will spin
        agent.stoppingDistance = 1f;
        // Set the wandering speed
        agent.speed = wanderingState.speed;

        // Start the wandering coroutine (for delays between destination changes)
        if (wanderingState.wanderCoroutine == null)
            wanderingState.wanderCoroutine = StartCoroutine(UpdateWanderPosition());


        IEnumerator UpdateWanderPosition()
        {
            // Choose a 2d point in the range
            Vector2 point = Random.insideUnitCircle * wanderingState.wanderRange;
            // Sample that point on the nav mesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(new Vector3(point.x, 0, point.y), out hit, Mathf.Infinity, NavMesh.AllAreas))
                wanderingState.wanderPosition = hit.position;

            // Add the range offset
            wanderingState.wanderPosition += wanderingState.wanderRangePosition;

            // Set the agents destination to the new point
            agent.SetDestination(wanderingState.wanderPosition);

            // Add a delay before resetting the cooldown
            float time = Random.Range(wanderingState.wanderChangeInterval.x, wanderingState.wanderChangeInterval.y);
            yield return new WaitForSeconds(time);

            wanderingState.wanderCoroutine = null;
        }
    }

    public void FacilitatePursue()
    {
        // Set the pursuing speed
        agent.speed = pursuingState.speed;
        
        // Set the path
        agent.SetDestination(pursuingState.target.position);
    }

    public void FacilitateAttacks()
    {
        switch (agentType)
        {
            // Melee attacks
            case AgentType.Melee:
                agent.speed = meleeAttackingState.speed;
                agent.stoppingDistance = meleeAttackingState.attackRange;
                break;
            // Ranged attacks
            case AgentType.Ranged:
                agent.speed = rangeAttackingState.speed;
                agent.stoppingDistance = rangeAttackingState.attackRange;

                if (rangeAttackingState.attackCoroutine == null)
                {
                    // Projectiles coroutine (for cooldowns)
                    rangeAttackingState.attackCoroutine = StartCoroutine(RangeAttack());
                }
                break;
        }


        IEnumerator RangeAttack()
        {
            yield return new WaitForSeconds(rangeAttackingState.attackRate);
            RangedProjectile projectile = Instantiate(rangeAttackingState.projectilePrefab.gameObject, transform.position + rangeAttackingState.projectileOrigin, Quaternion.identity).GetComponent<RangedProjectile>();
            projectile.direction = (rangeAttackingState.target.position - projectile.transform.position).normalized;

            rangeAttackingState.attackCoroutine = StartCoroutine(RangeAttack());
        }
    }

    public void FacilitateRetreat()
    {
        // Ensure agent has no stopping distance
        agent.stoppingDistance = 0f;

        // Set the agents speed to the retreating speed
        agent.speed = rangeAttackingState.retreatSpeed;

        // Get the target position to retreat from
        Vector3 targetPosition = rangeAttackingState.target.position;

        // Calculate the direction to retreat (away from the target)
        Vector3 retreatDirection = agent.transform.position - targetPosition;
        retreatDirection.Normalize();

        // Calculate the retreat position
        Vector3 retreatPosition = agent.transform.position + retreatDirection * rangeAttackingState.attackRange;

        // Move the nav mesh agent to the retreat position
        agent.SetDestination(retreatPosition);
    }


    /// <summary>
    /// Rotates the agent to face in the direction of the path or target
    /// </summary>
    public void UpdateRotation()
    {
        Vector3 targetPosition = agent.steeringTarget;
        if (brainState == BrainState.attacking)
        {
            if (agentType == AgentType.Melee)
                targetPosition = meleeAttackingState.target.position;
            else
                targetPosition = rangeAttackingState.target.position;
        }

        // The difference in position between the agent and the next point in its path
        Vector3 deltaPosition = targetPosition - transform.position;

        

        if (deltaPosition != Vector3.zero)
        {
            // Calculate the rotation
            Quaternion toRotation = Quaternion.LookRotation(deltaPosition);
            // Interpolate and apply the rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 6 * Time.deltaTime);
        }
    }



    public void OnDrawGizmosSelected()
    {
        // Wander state gizmos
        if (gizmoSettings.wanderingGizmos)
        {
            // Assign the gizmo color
            Gizmos.color = gizmoSettings.wanderingGizmosColor;

            if (Application.isPlaying)
            {
                // Draw wander range
                Gizmos.DrawWireSphere(wanderingState.wanderRangePosition, wanderingState.wanderRange);

                // Only draw path/destination if the agent is wandering
                if (brainState == BrainState.wandering)
                {
                    DrawGizmoPath();
                }
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, wanderingState.wanderRange);
            }
        }
        if (gizmoSettings.pursuingGizmos)
        {
            Gizmos.color = gizmoSettings.pursuingGizmosColor;

            Gizmos.DrawWireSphere(transform.position, pursuingState.visionRange);
            DrawGizmoPath();
        }
        if (gizmoSettings.meleeGizmos && agentType == AgentType.Melee)
        {
            Gizmos.color = gizmoSettings.meleeGizmosColor;
            Gizmos.DrawWireSphere(transform.position, meleeAttackingState.attackRange);
            DrawGizmoPath();
        }
        if (gizmoSettings.rangeGizmos && agentType == AgentType.Ranged)
        {
            Gizmos.color = gizmoSettings.rangeGizmosColor;
            Gizmos.DrawWireSphere(transform.position, rangeAttackingState.attackRange);
            Gizmos.DrawWireSphere(transform.position, rangeAttackingState.retreatDistance);
            Gizmos.DrawSphere(transform.position + rangeAttackingState.projectileOrigin, 0.2f);
            DrawGizmoPath();
        }
    }

    private void DrawGizmoPath()
    {
        if (agent == null)
            return;

        // Draw path to wander destination
        for (int i = 0; i < agent.path.corners.Length; i++)
        {
            Vector3 point = agent.path.corners[i];

            if (i == 0)
                Gizmos.DrawLine(transform.position, point);
            else if (i < agent.path.corners.Length)
                Gizmos.DrawLine(agent.path.corners[i - 1], point);

        }

        // Draw wander destination point
        Gizmos.DrawSphere(agent.destination, 0.25f);
    }


    public static void GenerateFriendlyAgent()
    {
        // Create the agent gameobject
        GameObject agentObject = new GameObject("Friendly Agent");
        // Set it to be the 'Friendly' layer
        agentObject.layer = LayerMask.NameToLayer("Friendly");
        // Set the friendly tag
        agentObject.tag = "Friendly";

        // Add the nav mesh component
        NavMeshAgent agent = agentObject.AddComponent<NavMeshAgent>();
        // Configure the settings
        agent.angularSpeed = 0;

        // Add the agent component
        AIAgent AIAgent = agentObject.AddComponent<AIAgent>();
        // Set the hostile tags
        AIAgent.hostileTags = new List<string>
        {
            "Enemy"
        };

        // Add a health component
        AgentHealth health = agentObject.AddComponent<AgentHealth>();

        // Create temporary capsule graphics
        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule.transform.SetParent(agentObject.transform);
        capsule.transform.localPosition = new Vector3(0, 1, 0);

        // Apply the 'Friendly' layer to all children of the agent
        for (int i = 0; i < agentObject.transform.childCount; i++)
        {
            agentObject.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Friendly");
            agentObject.transform.GetChild(i).gameObject.tag = "Friendly";
        }
    }
    
    public static void GenerateEnemyAgent()
    {
        // Create the agent gameobject
        GameObject agentObject = new GameObject("Enemy Agent");
        // Set it to be the 'Friendly' layer
        agentObject.layer = LayerMask.NameToLayer("Enemy");
        // Set the friendly tag
        agentObject.tag = "Enemy";

        // Add the nav mesh component
        NavMeshAgent agent = agentObject.AddComponent<NavMeshAgent>();
        // Configure the settings
        agent.angularSpeed = 0;

        // Add the agent component
        AIAgent AIAgent = agentObject.AddComponent<AIAgent>();
        // Set the hostile tags
        AIAgent.hostileTags = new List<string>
        {
            "Friendly"
        };

        // Create temporary capsule graphics
        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule.transform.SetParent(agentObject.transform);
        capsule.transform.localPosition = new Vector3(0, 1, 0);

        // Apply the 'Friendly' layer to all children of the agent
        for (int i = 0; i < agentObject.transform.childCount; i++)
        {
            agentObject.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Enemy");
            agentObject.transform.GetChild(i).gameObject.tag = "Enemy";
        }
    }


    #region Classes

    [System.Serializable]
    public class WanderingStateSettings
    {
        [Tooltip("The movement speed of the agent in this state.")]
        public float speed = 3.5f;

        [Space]

        [Tooltip("The radius of the circle in which the agent will select a point to wander within.")]
        public float wanderRange = 10f;
        [Tooltip("The time (in seconds) between the change in target wander destination. \n \nChooses a random value within the range (x, y).")]
        public Vector2 wanderChangeInterval = new Vector2(3f, 6f);


        // The target position for the range/circle in which the agent will pick a point within
        public Vector3 wanderRangePosition { get; set; }
        // The target position that the agent will wonder towards
        public Vector3 wanderPosition { get; set; }
        // The coroutine that handles the delay between choosing points
        public Coroutine wanderCoroutine { get; set; }
    }

    [System.Serializable]
    public class PursuingStateSettings
    {
        [Tooltip("The movement speed of the agent in this state.")]
        public float speed = 5f;
        [Tooltip("The distance in which the agent can 'see' enemies.")]
        public float visionRange = 25f;

        // The target that the agent will be chasing
        public Transform target { get; set; }
    }

    [System.Serializable]
    public class MeleeAttackingStateSettings
    {
        [Tooltip("The movement speed of the agent in this state.")]
        public float speed = 5f;
        [Tooltip("The maximum distance the agent must be within to attack the target.")]
        public float attackRange = 2f;


        // The target that the agent will be chasing
        public Transform target { get; set; }
    }

    [System.Serializable]
    public class RangeAttackingStateSettings
    {
        [Tooltip("The projectile that the ranged agent will fire.")]
        public RangedProjectile projectilePrefab;
        [Tooltip("The time between projectils being fired.")]
        public float attackRate = 1f;
        [Tooltip("The local position in which projectiles will be fired from.")]
        public Vector3 projectileOrigin;

        [Space]

        [Tooltip("The movement speed of the agent in this state.")]
        public float speed = 5f;
        [Tooltip("The maximum distance the agent must be within to attack the target.")]
        public float attackRange = 20;

        [Space]

        [Tooltip("The distance from the target from which this agent will retreat.")]
        public float retreatDistance = 14f;
        [Tooltip("The movement speed of the agent whilst retreating")]
        public float retreatSpeed = 8f;

        // The target that the agent will be chasing
        public Transform target { get; set; }
        // The cooldown coroutine between attacks
        public Coroutine attackCoroutine { get; set; }
    }

    [System.Serializable]
    public class GizmoSettings
    {
        public bool wanderingGizmos = false;
        public bool pursuingGizmos = false;
        public bool meleeGizmos = false;
        public bool rangeGizmos = false;

        [Space]

        public Color wanderingGizmosColor = new Color(0f, 1f, 1f);
        public Color pursuingGizmosColor = new Color(1f, 0.3f, 0f);
        public Color meleeGizmosColor = new Color(1f, 0f, 0f);
        public Color rangeGizmosColor = new Color(0f, 0f, 1f);
    }

    #endregion
}
