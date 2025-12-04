using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform[] playertransforms;
    public GameObject[] players;

    public LayerMask obstacleMask, groundMask;

    public float viewDistance = 10f;
    public float maxDistance = 15f;
    public float viewAngle = 90f;

    //Roam
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public bool chasing;

    public float aggroLost = 3f;
    public float outOfSight = 0f;

    public Transform currPlayer;

    public float facingThreshold = 0.9f;
    public bool stunned = false;

    private void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        playertransforms = new Transform[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playertransforms.SetValue(players[i].transform, i);
        }
        agent = GetComponent<NavMeshAgent>();
        walkPointSet = false;
        chasing = false;
        walkPointRange = 10f;
    }

    private void Update()
    {
        if(players.Length == 0)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            playertransforms = new Transform[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                playertransforms.SetValue(players[i].transform, i);
            }
        }
        if (playerInView())
        {
            outOfSight = 0f;
            chasing = true;
            if (!stunned)
            {
                agent.SetDestination(currPlayer.position);
            }
            else
            {
                agent.SetDestination(transform.position);
            }
        }
        else if (chasing)
        {
            outOfSight += Time.deltaTime;
            if (outOfSight < aggroLost)
            {
                if (!stunned)
                {
                    agent.SetDestination(currPlayer.position);
                }
                else
                {
                    agent.SetDestination(transform.position);
                }
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
        Vector3[] dirToPlayers = new Vector3[playertransforms.Length];
        float[] distanceToPlayers = new float[playertransforms.Length];
        float[] anglesBetweenPlayers = new float[playertransforms.Length];

        for (int i = 0; i < playertransforms.Length; i++)
        {
            dirToPlayers.SetValue((playertransforms[i].position - transform.position).normalized, i);
            distanceToPlayers.SetValue(dirToPlayers[i].magnitude, i);
            anglesBetweenPlayers.SetValue(Vector3.Angle(transform.forward, dirToPlayers[i]), i);
        }

        float shortestDist = int.MaxValue;

        for (int i = 0; i < playertransforms.Length; i++)
        {
            if (distanceToPlayers[i] <= viewDistance && anglesBetweenPlayers[i] <= viewAngle / 2f)
            {
                if (Physics.Raycast(transform.position + Vector3.up, dirToPlayers[i], out RaycastHit hit, viewDistance, ~obstacleMask))
                {
                    if (hit.transform == playertransforms[i])
                    {
                        Vector3 playerDir = (playertransforms[i].position - transform.position).normalized;
                        Vector3 enemyDir = (transform.position - playertransforms[i].position).normalized;
                        float enemyDot = Vector3.Dot(transform.forward, playerDir);
                        float playerDot = Vector3.Dot(playertransforms[i].forward, enemyDir);
                        if(enemyDot > facingThreshold && playerDot > facingThreshold && players[i].GetComponent<Flashlight>().maxLightToggle)
                        {
                            stunned = true;
                        }
                        else
                        {
                            stunned = false;
                        }
                        if (distanceToPlayers[i] < shortestDist)
                        {
                            currPlayer = playertransforms[i];
                            shortestDist = distanceToPlayers[i];
                        }
                    }
                }
            }
            else if (distanceToPlayers[i] <= maxDistance && anglesBetweenPlayers[i] <= viewAngle / 2f)
            {
                if (Physics.Raycast(transform.position + Vector3.up, dirToPlayers[i], out RaycastHit hit, maxDistance, ~obstacleMask))
                {
                    if (hit.transform == playertransforms[i])
                    {
                        Vector3 playerDir = (playertransforms[i].position - transform.position).normalized;
                        Vector3 enemyDir = (transform.position - playertransforms[i].position).normalized;
                        float enemyDot = Vector3.Dot(transform.forward, playerDir);
                        float playerDot = Vector3.Dot(playertransforms[i].forward, enemyDir);
                        if (enemyDot > facingThreshold && playerDot > facingThreshold && players[i].GetComponent<Flashlight>().maxLightToggle)
                        {
                            stunned = true;
                        }
                        else
                        {
                            stunned = false;
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