using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using StarterAssets;
using Photon.Pun; // 포톤 네트워킹 using 추가

public class Spider : MonoBehaviour // Photon.Pun.MonoBehaviourPunCallbacks 상속
{

    // 빛의 몬스터 상태
    public enum MonsterState
    {
        Idle,           // 대기 상태
        Patrol,         // 정찰 상태
        Chase,          // 추적 상태
        Attack,         // 공격 상태
        Run,            // 도망 상태
        COUNT,
    }

    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;                      // NavMesh 사용

    [Header("Settings")]
    [SerializeField] private float attackRange = 2.0f;                // 공격 범위
    [SerializeField] private float detectionRange = 10.0f;              // 감지 범위
    [SerializeField] private float patrolSpeed = 2.0f;                // 정찰 속도
    [SerializeField] private float chaseSpeed = 5.0f;                 // 추적 속도
    [SerializeField] private float RandAnRunRadius = 5.0f;             // 도망가거나 랜덤으로 잡을 때 반경
    [SerializeField] private float fieldOfViewAngle = 360.0f;          // 시야각

    private MonsterState currentState;                              // 플레이어 상태

    private GameSystem gameSystem;                                  // 게임시스템
    public Animator spiderAnimator;                                     // 스파이더 애니메이션
    private FirstPersonController playerController;

    private Transform FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {

            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                closestPlayer = player.transform;
                minDistance = distance;
            }
        }
        return closestPlayer;
    }

    private FirstPersonController FindClosestPlayerTransform()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        FirstPersonController closestPlayerController = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {

            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                closestPlayerController = player.GetComponent<FirstPersonController>();
                minDistance = distance;
            }
        }
        return closestPlayerController;
    }

    private void Update()
    {

        // if (photonView.IsMine)
        // {
        //     // ... (기존 코드)

        //     player = FindClosestPlayer(); // 가장 가까운 플레이어 찾기
        //     playerController = FindClosestPlayerTransform();
        //     // ... (기존 코드)
        // }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // mesh = GetComponent<MeshRenderer>();
        currentState = MonsterState.COUNT;
    }

    private void Start()
    {
        gameSystem = GameSystem.Instance;
        ChangeState(MonsterState.Idle);
    }

    private void FixedUpdate()
    {
        playerController = FindClosestPlayerTransform();

        if (playerController != null)
            CheckPlayerDistance();
        else
            ChangeState(MonsterState.Idle);

        // 상태 머신
        switch (currentState)
        {
            case MonsterState.Idle:
                IdleState();
                break;
            case MonsterState.Patrol:
                PatrolState();
                break;
            case MonsterState.Chase:
                ChaseState();
                break;
            case MonsterState.Attack:
                AttackState();
                break;
            case MonsterState.Run:
                RunState();
                break;
        }
        // 호스트에서만 몬스터 AI 실행
    }

    private void CheckPlayerDistance()
    {
        // 거리 계산
        float distanceToPlayer = (playerController.transform.position - transform.position).sqrMagnitude;
        float sqrDetectionRange = detectionRange * detectionRange;

        // 상태별 거리 체크
        switch (currentState)
        {
            case MonsterState.Chase:
                if (distanceToPlayer <= attackRange * attackRange)
                {
                    ChangeState(MonsterState.Attack);
                }
                else if (gameSystem.IsSafeZone || playerController._input.flash) // 도망 조건 추가
                {
                    ChangeState(MonsterState.Run);
                }
                break;
            case MonsterState.Run:
                if (distanceToPlayer > detectionRange * detectionRange * 4.0f) // 도망 상태에서는 더 먼 거리에서 Idle로 전환
                {
                    ChangeState(MonsterState.Idle);
                }
                break;
            default:
                if (distanceToPlayer <= sqrDetectionRange && CanSeePlayer())
                {
                    ChangeState(MonsterState.Idle);
                }
                break;
        }
    }

    private void IdleState()
    {
        // 대기 상태
        agent.speed = patrolSpeed;

        // remaingDistance는 목적지까지의 거리가 얼마나 남았는지에 대한 여부이다.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            RPC_SetRandomPosition(); // RPC 호출
        }

        // 어두운 곳에서 플레이어 감지 시 추적 상태로 전환
        if (!gameSystem.IsSafeZone && !playerController._input.flash && CanSeePlayer())
        {
            ChangeState(MonsterState.Chase);
        }
    }

    private void PatrolState()
    {
        // 순찰 상태
        // 무언가 대기상태랑 비슷하다.

        IdleState();
        SoundManager.Instance.PlayerFootstep(0.4f, "Spider_Walk", transform);
        // 어두운 곳에서 플레이어 감지 시 추적 상태로 전환
        if (!gameSystem.IsSafeZone && !playerController._input.flash && CanSeePlayer())
        {
            ChangeState(MonsterState.Chase);
        }
    }

    private void ChaseState()
    {
        // 추적 상태 로직
        SoundManager.Instance.PlayGrowlingSound("Find_Spider");

        AudioClip growlingClip = SoundManager.Instance.GetClip("Find_Spider");
        float growlingTime = growlingClip.length;
        Invoke(nameof(PlayFootstepAfterGrowling), growlingTime);

        agent.speed = chaseSpeed;
        RPC_SetDestination(playerController.transform.position); // RPC 호출
    }

    private void PlayFootstepAfterGrowling()
    {
        SoundManager.Instance.PlayerFootstep(0.2f, "Spider_Walk", transform);
    }

    private float attackDelay = 0.0f;
    private float attackInterval = 20.0f;                // 20초마다 때리기
    private void AttackState()
    {
        attackDelay += Time.deltaTime;
        if (attackDelay >= attackInterval)
        {
            // 공격 상태 로직
            agent.ResetPath();

            // 플레이어로 바라보기
            RPC_LookAtPlayer(); // RPC 호출

            // 그리고 어택딜레이는 여기서 추가해도 됨
            //            Debug.Log("플레이어 공격!");
            // 일단 Dotween으로 그냥 애니메이션 간단하게 재생
            //this.transform.DOPunchScale(Vector3.one, 0.5f);
            ChangeState(MonsterState.Chase);
            attackDelay = 0.0f;
        }
    }

    private void RunState()
    {
        agent.speed = chaseSpeed;

        // 도망갈 때 런 포지션을 따로 잡아놓는 것으로 함
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            RPC_SetRunPosition(); // RPC 호출
        }
    }

    private bool CanSeePlayer()
    {
        if (playerController.transform == null) return false;

        // 몬스터에서 플레이어까지의 방향 벡터와 거리 계산
        Vector3 directionToPlayer = playerController.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // 플레이어가 감지 범위 내에 있는지 확인
        if (distanceToPlayer > detectionRange)
        {
            return false;
        }

        // 시야각 내에 있는지 확인
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer.normalized);
        if (dotProduct < Mathf.Cos(fieldOfViewAngle * 0.5f * Mathf.Deg2Rad))
        {
            return false;
        }

        // 이 조건을 뚫고 오면 플레이어를 찾은거
        //        Debug.Log("플레이어 찾음");
        return true;
    }

    private void RPC_SetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * RandAnRunRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, RandAnRunRadius, NavMesh.AllAreas))
        {
            SoundManager.Instance.PlayGrowlingSound("Spider_Growl");
            agent.SetDestination(hit.position);
        }
    }

    private void RPC_SetRunPosition()
    {
        Vector3 fleeDirection = transform.position - playerController.transform.position;
        Vector3 fleePosition = transform.position + fleeDirection.normalized * RandAnRunRadius;

        NavMeshHit hit;
        // SamplePosition은 내가 가려고 하는 위치가 NavMesh에 포함이 되는지 확인하고 있으면
        // 유효한 위치를 반환하게 됨
        if (NavMesh.SamplePosition(fleePosition, out hit, RandAnRunRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void RPC_SetDestination(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
    }

    private void RPC_LookAtPlayer()
    {
        transform.LookAt(playerController.transform);
    }

    private string GetStateString()
    {
        switch (currentState)
        {
            case MonsterState.Idle:           // 대기상태
            case MonsterState.Patrol:         // 정찰상태
            case MonsterState.Chase:          // 추적상태
            case MonsterState.Run:            // 도망상태
                return "running";
            case MonsterState.Attack:         // 공격상태
                return "attack2";
        }

        return "Idle";
    }


    private void ChangeState(MonsterState newState)
    {
        RPC_ChangeState((int)newState);
    }


    private void RPC_ChangeState(int newState)
    {
        if (currentState != (MonsterState)newState)
        {
            spiderAnimator.SetBool(GetStateString(), false);

            // 상태 변경
            currentState = (MonsterState)newState;
            spiderAnimator.SetBool(GetStateString(), true);
        }
    }





#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        float halfFOV = fieldOfViewAngle * 0.5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * transform.forward * detectionRange;
        Vector3 rightRayDirection = rightRayRotation * transform.forward * detectionRange;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftRayDirection);
        Gizmos.DrawRay(transform.position, rightRayDirection);

        if (playerController.transform != null)
        {
            float sqrDistanceToPlayer = (playerController.transform.position - transform.position).sqrMagnitude;
            if (sqrDistanceToPlayer <= detectionRange * detectionRange)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, playerController.transform.position);
            }
        }
    }
#endif
}