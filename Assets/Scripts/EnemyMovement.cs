using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour {
    public NavMeshAgent agent;
    public Transform[] playertransforms;
    public GameObject[] players;
    public LayerMask obstacleMask, groundMask;

    public float viewDistance = 10f;
    public float maxDistance = 15f;
    public float viewAngle = 90f;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public bool chasing;
    public float aggroLost = 3f;
    public float outOfSight = 0f;
    public Transform currPlayer;
    public float facingThreshold = 0.9f;
    public bool stunned = false;

    private void Start() {
        InitializePlayers();
        agent = GetComponent<NavMeshAgent>();
        walkPointSet = false;
        chasing = false;
        walkPointRange = 10f;
    }

    // Extract for testing
    public void InitializePlayers() {
        players = GameObject.FindGameObjectsWithTag("Player");
        playertransforms = new Transform[players.Length];
        for (int i = 0; i < players.Length; i++) {
            playertransforms[i] = players[i].transform;
        }
    }

    private void Update() {
        UpdateChaseState();
    }

    // Extract for testing
    public void UpdateChaseState() {
        if (playerInView()) {
            outOfSight = 0f;
            chasing = true;
            SetDestinationToPlayer();
        } else if (chasing) {
            outOfSight += Time.deltaTime;
            if (outOfSight < aggroLost) {
                SetDestinationToPlayer();
            } else {
                StopChasing();
            }
        } else {
            Roam();
        }
    }

    private void SetDestinationToPlayer() {
        if (agent != null && currPlayer != null) {
            if (!stunned) {
                agent.SetDestination(currPlayer.position);
            } else {
                agent.SetDestination(transform.position);
            }
        }
    }

    private void StopChasing() {
        chasing = false;
        outOfSight = 0f;
        SetWalkPoint();
    }

    // Extract distance/angle calculation for testing
    public bool IsPlayerInViewRange(Vector3 playerPos, out float distance, out float angle) {
        Vector3 dirToPlayer = (playerPos - transform.position).normalized;
        distance = Vector3.Distance(transform.position, playerPos);
        angle = Vector3.Angle(transform.forward, dirToPlayer);

        return distance <= viewDistance && angle <= viewAngle / 2f;
    }

    // Extract facing check for testing
    public bool AreFacingEachOther(Transform player, out float enemyDot, out float playerDot) {
        Vector3 playerDir = (player.position - transform.position).normalized;
        Vector3 enemyDir = (transform.position - player.position).normalized;

        enemyDot = Vector3.Dot(transform.forward, playerDir);
        playerDot = Vector3.Dot(player.forward, enemyDir);

        return enemyDot > facingThreshold && playerDot > facingThreshold;
    }

    private bool playerInView() {
        if (playertransforms == null || playertransforms.Length == 0) return false;

        Vector3[] dirToPlayers = new Vector3[playertransforms.Length];
        float[] distanceToPlayers = new float[playertransforms.Length];
        float[] anglesBetweenPlayers = new float[playertransforms.Length];

        for (int i = 0; i < playertransforms.Length; i++) {
            dirToPlayers[i] = (playertransforms[i].position - transform.position).normalized;
            distanceToPlayers[i] = Vector3.Distance(transform.position, playertransforms[i].position);
            anglesBetweenPlayers[i] = Vector3.Angle(transform.forward, dirToPlayers[i]);
        }

        float shortestDist = float.MaxValue;

        for (int i = 0; i < playertransforms.Length; i++) {
            if (distanceToPlayers[i] <= viewDistance && anglesBetweenPlayers[i] <= viewAngle / 2f) {
                if (Physics.Raycast(transform.position + Vector3.up, dirToPlayers[i], out RaycastHit hit, viewDistance, ~obstacleMask)) {
                    if (hit.transform == playertransforms[i]) {
                        CheckStunCondition(i);
                        if (distanceToPlayers[i] < shortestDist) {
                            currPlayer = playertransforms[i];
                            shortestDist = distanceToPlayers[i];
                        }
                    }
                }
            } else if (distanceToPlayers[i] <= maxDistance && anglesBetweenPlayers[i] <= viewAngle / 2f) {
                if (Physics.Raycast(transform.position + Vector3.up, dirToPlayers[i], out RaycastHit hit, maxDistance, ~obstacleMask)) {
                    if (hit.transform == playertransforms[i]) {
                        CheckStunCondition(i);
                    }
                }
            }
        }

        return shortestDist != float.MaxValue;
    }

    private void CheckStunCondition(int playerIndex) {
        Vector3 playerDir = (playertransforms[playerIndex].position - transform.position).normalized;
        Vector3 enemyDir = (transform.position - playertransforms[playerIndex].position).normalized;
        float enemyDot = Vector3.Dot(transform.forward, playerDir);
        float playerDot = Vector3.Dot(playertransforms[playerIndex].forward, enemyDir);

        var flashlight = players[playerIndex].GetComponent<Flashlight>();
        if (enemyDot > facingThreshold && playerDot > facingThreshold && flashlight != null && flashlight.maxLightToggle) {
            stunned = true;
        } else {
            stunned = false;
        }
    }

    private void Roam() {
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f) {
            walkPointSet = false;
        }

        if (!walkPointSet) {
            SetWalkPoint();
        }

        if (walkPointSet && agent != null) {
            agent.SetDestination(walkPoint);
        }
    }

    public void SetWalkPoint() {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundMask)) {
            walkPointSet = true;
        }
    }

#if UNITY_EDITOR
    // Test helpers
    public void TestSetChasing(bool value) { chasing = value; }
    public void TestSetOutOfSight(float value) { outOfSight = value; }
#endif
}