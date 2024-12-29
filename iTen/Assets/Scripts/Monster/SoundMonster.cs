using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterState
{
    Watch,
    Walk,
    Run,
    Attack
}

public class SoundMonster : MonoBehaviour
{
    private float detectionRadius = 5f;
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
            currentState = MonsterState.Run;
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

        if (walkingState == 1 && currentState != MonsterState.Run)
        {
            StartCoroutine(SwitchToRunAfterIdle());
        }
        else if (walkingState == 0)
        {
            currentState = MonsterState.Walk;
        }
    }

    private IEnumerator SwitchToRunAfterIdle()
    {
        currentState = MonsterState.Watch;
        animator.SetInteger("isWalking", 0); // 잠시 Idle 상태 유지
        yield return new WaitForSeconds(0.2f); // 잠시 멈춘 후
        animator.SetInteger("isWalking", 1);   // Run 상태로 전환
        currentState = MonsterState.Run;
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
        if (isAttacking || currentState == MonsterState.Watch)
        {
            return;
        }

        agent.isStopped = true;
        animator.SetTrigger("isWatching");
        currentState = MonsterState.Watch;
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
            Debug.Log("Pass Detecting");
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        if (hitColliders.Length > 0)
        {
            detectedPlayer = hitColliders[0].transform;
            if (!isAttacking)
            {
                TriggerAttack();
            }
        }
        else
        {
            detectedPlayer = null;
            if (!isChasing)
            {
                TriggerWatch();
            }
        }
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
            Debug.Log($"Monster is chasing target at {currentTarget.Value}");
        }
    }
}
