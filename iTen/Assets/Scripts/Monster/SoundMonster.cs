using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun; // 포톤 네트워킹 using 추가

public enum MonsterState
{
    Idle,
    Patrol,
    Chase,
    Attack
}

public class SoundMonster : MonoBehaviourPunCallbacks // Photon.Pun.MonoBehaviourPunCallbacks 상속
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
        SetWalkingState(0); // Idle 상태 설정
        TriggerWatch();
    }

    public void OnSoundHeard(Vector3 soundPos)
    {
        photonView.RPC("RPC_OnSoundHeard", RpcTarget.All, soundPos); // RPC 호출
    }

    [PunRPC]
    private void RPC_OnSoundHeard(Vector3 soundPos)
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
            photonView.RPC("RPC_StartChasing", RpcTarget.All, currentTarget.Value); // RPC 호출
        }
    }

    [PunRPC]
    private void RPC_StartChasing(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
        agent.speed = chaseSpeed;
        isChasing = true;
        currentState = MonsterState.Chase;
        StopCoroutine(WanderRandomly());
        StartCoroutine(UpdateChasingTarget());
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
        photonView.RPC("RPC_StopChasing", RpcTarget.All); // RPC 호출
    }

    [PunRPC]
    private void RPC_StopChasing()
    {
        isChasing = false;
        agent.speed = normalSpeed;
        currentTarget = null;
        SetWalkingState(0);
        StartCoroutine(WanderRandomly());
    }

    public void SetWalkingState(int walkingState)
    {
        photonView.RPC("RPC_SetWalkingState", RpcTarget.All, walkingState); // RPC 호출
    }

    [PunRPC]
    public void RPC_SetWalkingState(int walkingState)
    {
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
        animator.SetTrigger("isWatching");
        yield return new WaitForSeconds(0.2f); // 잠시 대기
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
                float distanceToTarget = Vector3.Distance(transform.position, curTarget.position);

                if (distanceToTarget <= 50f)
                {
                    if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    {
                        Vector3 randomDirection = Random.insideUnitSphere * 50f;
                        randomDirection += transform.position;

                        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                        {
                            photonView.RPC("RPC_MoveToTarget", RpcTarget.All, hit.position); // RPC 호출
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
                    Vector3 newPosition = curTarget.position;

                    if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                    {
                        //Debug.Log($"Monster is far away from player. Monster is going to {hit.position}");
                        photonView.RPC("RPC_MoveToTarget", RpcTarget.All, hit.position); // RPC 호출
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

    [PunRPC]
    private void RPC_MoveToTarget(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
    }

    private Transform FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closestPlayer = null;
        float minDistance = Mathf.Infinity;

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
        Gizmos.DrawWireSphere(transform.position, 50f);
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
        photonView.RPC("RPC_TriggerAttack", RpcTarget.All); // RPC 호출
    }

    [PunRPC]
    public void RPC_TriggerAttack()
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
        photonView.RPC("RPC_TriggerWatch", RpcTarget.All); // RPC 호출
    }

    [PunRPC]
    public void RPC_TriggerWatch()
    {
        if (isAttacking || currentState == MonsterState.Idle)
        {
            return;
        }

        agent.isStopped = true;
        currentState = MonsterState.Idle;
        animator.SetTrigger("isWatching");
        StartCoroutine(HandlePostWatch());
    }

    private IEnumerator HandlePostWatch()
    {
        yield return new WaitForSecondsRealtime(3.11f);

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
        // 플레이어 감지 로직은 각 클라이언트에서 실행되지만, 
        // TriggerAttack() 함수는 RPC를 통해 모든 클라이언트에서 동기화됩니다.
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
            float distanceToPlayer = Vector3.Distance(transform.position, potentialPlayer.position);

            int wallCount = DetectWallsBetween(transform.position, potentialPlayer.position);

            float adjustRad = detectionRadius - wallCount * 5f;
            if (distanceToPlayer <= adjustRad)
            {
                detectedPlayer = potentialPlayer;
                Debug.Log($"Player detected. Wall count: {wallCount}, Adjusted radius: {adjustRad}");

                if (!isAttacking)
                {
                    TriggerAttack(); // RPC 호출
                }
                return;
            }
        }
        detectedPlayer = null;
        if (!isChasing)
        {
            TriggerWatch(); // RPC 호출
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
        // DetectPlayer() 함수는 각 클라이언트에서 실행되지만, 
        // TriggerAttack() 함수는 RPC를 통해 모든 클라이언트에서 동기화됩니다.
        DetectPlayer();

        if (isChasing && currentTarget.HasValue)
        {
            agent.SetDestination(currentTarget.Value);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                StopChasing(); // RPC 호출
            }
        }

        switch (currentState)
        {
            case MonsterState.Patrol:
                animator.SetInteger("isWalking", 0);
                break;
            case MonsterState.Chase:
                animator.SetInteger("isWalking", 1);
                break;
            case MonsterState.Idle:
                Debug.Log("Monster state is idle");
                break;
            case MonsterState.Attack:
                Debug.Log("Monster state is attack");
                break;
            default:
                break;
        }
    }
}