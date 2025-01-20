using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterState
{
    Idle,
    Patrol,
    Chase,
    Attack
}

public class SoundMonster : MonoBehaviour
{
    private float detectionRadius = 20f;
    public LayerMask playerLayer;

    private Animator animator;
    private MonsterState currentState = MonsterState.Patrol;

    private NavMeshAgent agent;
    private Vector3? currentTarget;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isDetect = false;

    private Transform detectedPlayer;

    private float normalSpeed;
    private float chaseSpeed = 6;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        normalSpeed = agent.speed;
        agent.isStopped = true;
        TriggerWatch();
    }

    public void OnSoundHeard(Vector3 soundPos)
    {
        currentTarget = soundPos;

        if (!isChasing)
        {
            StartChasing();
        }
    }

    private void StartChasing()
    {
        if (currentTarget.HasValue)
        {
            agent.SetDestination(currentTarget.Value);
            agent.speed = chaseSpeed;
            isChasing = true;
            currentState = MonsterState.Chase;
            animator.SetInteger("isWalking", 1);
            StopCoroutine(WanderRandomly());
            StartCoroutine(UpdateChasingTarget());
        }
    }

    private IEnumerator UpdateChasingTarget()
    {
        while (isChasing)
        {
            if (currentTarget.HasValue)
            {
                agent.SetDestination(currentTarget.Value);

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentTarget = null;
                    isChasing = false;

                    Transform nearbyPlayer = FindClosestPlayer();

                    if (nearbyPlayer != null)
                    {
                        detectedPlayer = nearbyPlayer;
                        DetectPlayer();
                        yield break;
                    }
                    else
                    {
                        StopChasing();
                    }
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void StopChasing()
    {
        isChasing = false;
        agent.speed = normalSpeed;
        currentTarget = null;
        SetWalkingState(0);
        StartCoroutine(WanderRandomly());
    }

    public void SetWalkingState(int walkingState)
    {
        animator.SetInteger("isWalking", walkingState);

        if (walkingState == 1 && currentState != MonsterState.Chase)
        {
            StartCoroutine(SwitchToRunAfterIdle());
        }
        else if (walkingState == 0)
        {
            currentState = MonsterState.Patrol;
        }
    }

    private IEnumerator SwitchToRunAfterIdle()
    {
        currentState = MonsterState.Idle;
        animator.SetTrigger("isWatching");     // Idle ���·� ��ȯ
        yield return new WaitForSeconds(0.2f); // ��� ���� ��
        animator.SetInteger("isWalking", 1);   // Run ���·� ��ȯ
        agent.speed = chaseSpeed;
        currentState = MonsterState.Chase;
    }

    private IEnumerator WanderRandomly()
    {
        while (!isChasing)
        {
            Transform curTarget = FindClosestPlayer();

            if (curTarget != null)
            {
                currentState = MonsterState.Patrol;
                float distanceToTarget = Vector3.Distance(transform.position, curTarget.position);

                if (distanceToTarget <= 30f)
                {
                    if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    {
                        Vector3 randomDirection = Random.insideUnitSphere * 50f;
                        randomDirection += transform.position;

                        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                        {
                            agent.SetDestination(hit.position);
                            SetWalkingState(0);
                        }
                        else
                        {
                            TriggerWatch();
                        }
                    }
                }
                else
                {
                    //Vector3 directionToTarget = (curTarget.position - transform.position).normalized;
                    //float maxDistance = Mathf.Min(Vector3.Distance(transform.position, curTarget.position));
                    Vector3 newPosition = curTarget.position;

                    if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 20f, NavMesh.AllAreas))
                    {
                        Debug.Log($"Monster is far away from player. Monster is going to {hit.position}");
                        agent.SetDestination(hit.position);
                        SetWalkingState(0);
                    }
                    else
                    {
                        Debug.Log("NavMesh.SamplePosition failed to find a valid position.");
                    }
                }
            }

            yield return null;
        }
    }

    private Transform FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closestPlayer = null;
        float minDistance=Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float dis = Vector3.Distance(transform.position, player.transform.position);
            if (dis < minDistance)
            {
                closestPlayer = player.transform;
                minDistance = dis;
            }
        }
        return closestPlayer;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 30f);
    }

    private IEnumerator HandlePostAttack()
    {
        Debug.Log("HandlePostAttack");
        yield return new WaitForSeconds(2.1f);

        detectedPlayer = null;
        isAttacking = false;
        agent.isStopped = false;

        TriggerWatch();
        StartCoroutine(DetectionCoolTime());

        if (!isChasing)
        {
            StartCoroutine(WanderRandomly());
        }
    }

    public void TriggerAttack()
    {
        if (isAttacking || detectedPlayer == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, detectedPlayer.position);
        if (distanceToPlayer > agent.stoppingDistance)
        {
            return;
        }

        isAttacking = true;
        Debug.Log("TriggerAttack");

        Vector3 directionToPlayer = (detectedPlayer.position - transform.position).normalized;
        directionToPlayer.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        agent.isStopped = true;

        animator.SetTrigger("isAttack");
        currentState = MonsterState.Attack;

        StartCoroutine(HandlePostAttack());
    }


    public void TriggerWatch()
    {
        if (isAttacking || currentState == MonsterState.Idle)
        {
            return;
        }

        //Debug.Log("TriggerWatch");
        currentState = MonsterState.Idle;
        agent.isStopped = true;
        animator.SetTrigger("isWatching");
        StartCoroutine(HandlePostWatch());
    }

    private IEnumerator HandlePostWatch()
    {
        //Debug.Log("HandlePostWatch");
        yield return new WaitForSeconds(0.2f);

        if (!isChasing && !isAttacking)
        {
            agent.isStopped = false;
            SetWalkingState(0);
            StartCoroutine(WanderRandomly());
        }
    }

    private IEnumerator DetectionCoolTime()
    {
        isDetect = true;
        yield return new WaitForSeconds(10f);
        isDetect = false;
    }

    private void DetectPlayer()
    {
        if (isDetect || isAttacking)
        {
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        foreach (Collider collider in hitColliders)
        {
            if (((1 << collider.gameObject.layer) & playerLayer) == 0)
            {
                continue;
            }

            Transform potentialPlayer = collider.transform;
            Vector3 directionToPlayer = (potentialPlayer.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, potentialPlayer.position);

            int wallCount = DetectWallsBetween(transform.position, potentialPlayer.position);

            float adjustRad = detectionRadius - wallCount * 5f;
            if (distanceToPlayer <= adjustRad)
            {
                detectedPlayer = potentialPlayer;
                Debug.Log($"Player detected. Wall count: {wallCount}, Adjusted radius: {adjustRad}");

                if (!isAttacking)
                {
                    TriggerAttack();
                }
                return;
            }
        }

        detectedPlayer = null;
        if (!isChasing)
        {
            TriggerWatch();
        }
    }

    private int DetectWallsBetween(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        RaycastHit[] hits = Physics.RaycastAll(start, direction, distance);

        int wallCount = 0;
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                wallCount++;
            }
        }
        return wallCount;
    }

    private void Update()
    {
        DetectPlayer();

        if (detectedPlayer != null && !isAttacking)
        {
            agent.SetDestination(detectedPlayer.position);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                TriggerAttack();
            }
        }

        if (isChasing && currentTarget.HasValue)
        {
            agent.SetDestination(currentTarget.Value);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                StopChasing();
            }
        }

        //Debug.Log($"Monster current state is {currentState}");
    }
}
