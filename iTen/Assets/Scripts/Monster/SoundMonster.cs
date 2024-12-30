using NUnit.Framework.Internal.Commands;
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
    private MonsterState currentState;

    private NavMeshAgent agent;
    private Vector3? currentTarget;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isDetect = false;

    private Transform detectedPlayer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        SetWalkingState(0); // Idle 초기 상태
        StartCoroutine(WanderRandomly());
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
                    StopChasing();
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void StopChasing()
    {
        isChasing = false;
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
        animator.SetInteger("isWalking", 0);   // 잠시 Idle 상태 유지
        yield return new WaitForSeconds(0.2f); // 잠시 멈춘 후
        animator.SetInteger("isWalking", 1);   // Run 상태로 전환
        currentState = MonsterState.Chase;
    }

    private IEnumerator WanderRandomly()
    {
        while (!isChasing)
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

            yield return null;
        }
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

        agent.isStopped = true;
        animator.SetTrigger("isWatching");
        currentState = MonsterState.Idle;
        StartCoroutine (HandlePostWatch());
    }

    private IEnumerator HandlePostWatch()
    {
        yield return new WaitForSeconds(3.1f);

        agent.isStopped = false;

        if (!isChasing && !isAttacking)
        {
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
    }
}
