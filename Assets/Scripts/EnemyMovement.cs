using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask obstacleMask, groundMask;

    public float viewDistance = 10f;
    public float viewAngle = 90f;

    //Roam
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange = 10f;

    public bool chasing;

    public float aggroLost = 3f;
    public float outOfSight = 0f;



    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;  //Will change to array in the future, need to decide if we want to give players an ID to differentiate them
        agent = GetComponent<NavMeshAgent>();
        walkPointSet = false;
        chasing = false;
    }

    private void Update()
    {
        if (playerInView())
        {
            outOfSight = 0f;
            chasing = true;
            agent.SetDestination(player.position);
        }
        else if (chasing)
        {
            outOfSight += Time.deltaTime;
            if (outOfSight < aggroLost)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                chasing = false;
                outOfSight = 0f;
                SetWalkPoint();
            }
        }
        else
        {
            Roam();
        }
    }
    private bool playerInView()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = dirToPlayer.magnitude;

        if (distanceToPlayer > viewDistance)
        {
            return false;
        }

        float angleBetween = Vector3.Angle(transform.forward, dirToPlayer);
        if (angleBetween > viewAngle / 2f)
        {
            return false;
        }

        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, out RaycastHit hit, viewDistance, ~obstacleMask))
        {
            if (hit.transform == player)
            {
                return true;
            }
        }

        return false;
    }

    private void Roam()
    {
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }

        if (!walkPointSet)
        {
            SetWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
    }

    private void SetWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundMask))
        {
            walkPointSet = true;
        }

    }
}

