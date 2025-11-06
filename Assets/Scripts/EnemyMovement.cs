using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform[] players;

    public LayerMask obstacleMask, groundMask;

    public float viewDistance = 10f;
    public float viewAngle = 90f;

    //Roam
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public bool chasing;

    public float aggroLost = 3f;
    public float outOfSight = 0f;

    public Transform currPlayer;



    private void Start()
    {
        GameObject[] temp;
        temp = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[temp.Length];
        for (int i = 0; i < temp.Length; i++)
        {
            players.SetValue(temp[i].transform, i);
        }
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
            agent.SetDestination(currPlayer.position);
        }
        else if (chasing)
        {
            outOfSight += Time.deltaTime;
            if (outOfSight < aggroLost)
            {
                agent.SetDestination(currPlayer.position);
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
        Vector3[] dirToPlayers = new Vector3[players.Length];
        float[] distanceToPlayers = new float[players.Length];
        float[] anglesBetweenPlayers = new float[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            dirToPlayers.SetValue((players[i].position - transform.position).normalized, i);
            distanceToPlayers.SetValue(dirToPlayers[i].magnitude, i);
            anglesBetweenPlayers.SetValue(Vector3.Angle(transform.forward, dirToPlayers[i]), i);
        }

        float shortestDist = int.MaxValue;

        for (int i = 0; i < players.Length; i++)
        {
            if (distanceToPlayers[i] <= viewDistance && anglesBetweenPlayers[i] <= viewAngle / 2f)
            {
                if (Physics.Raycast(transform.position + Vector3.up, dirToPlayers[i], out RaycastHit hit, viewDistance, ~obstacleMask))
                {
                    if (hit.transform == players[i])
                    {
                        if (distanceToPlayers[i] < shortestDist)
                        {
                            currPlayer = players[i];
                            shortestDist = distanceToPlayers[i];
                        }
                    }
                }
            }
        }

        if (shortestDist != int.MaxValue)
        {
            return true;
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