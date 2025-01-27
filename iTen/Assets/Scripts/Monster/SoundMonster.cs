using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public enum MonsterState
{
    Idle,
    Patrol,
    Chase,
    Attack
}

public class SoundMonster : MonoBehaviourPunCallbacks
{
    private MonsterState currentState = MonsterState.Patrol;

    private PhotonView photonView;

    private float detectionRadius = 30f;
    public LayerMask playerLayer;

    private Animator animator;
    //private MonsterState currentState = MonsterState.Patrol;

    private NavMeshAgent agent;
    private Vector3? currentTarget;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isDetect = false;
    //private PhotonView photonView;

    private Transform detectedPlayer; //플레이어 트랜스

    private float normalSpeed;
    private float chaseSpeed = 5;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        normalSpeed = agent.speed;

        if (PhotonNetwork.IsMasterClient)
        {
            TriggerWatch();
        }
        
    }

    [PunRPC]
    private void SetTargetPlayer(int playerId)
    {
        detectedPlayer = PhotonView.Find(playerId).transform;
        SwitchToRunAfterIdle();
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


    [PunRPC]
    public void OnSoundHeard(Vector3 soundPos)
    {
        currentTarget = soundPos;

        if (!isChasing)
        {
            SoundManager.Instance.PlayGrowlingSound("Find_SoundMonster");
            SwitchToRunAfterIdle();
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
        currentState = MonsterState.Patrol;
        StartCoroutine(WanderRandomly());
    }

    private void SwitchToRunAfterIdle()
    {
        SoundManager.Instance.PlayGrowlingSound("Find_SoundMonster");
        currentState = MonsterState.Chase;
        agent.speed = chaseSpeed;
        StopCoroutine(WanderRandomly());
        StartCoroutine(UpdateChasingTarget());
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
                            agent.SetDestination(hit.position);
                            SoundManager.Instance.PlayGrowlingSound("SoundMonster_Growl");
                            currentState = MonsterState.Patrol;
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
                        agent.SetDestination(hit.position);
                        currentState = MonsterState.Patrol;
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 50f);
    }

    private IEnumerator HandlePostAttack()
    {
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

    [PunRPC]
    public void TriggerAttack(int playerViewId)
    {
        if (isAttacking || detectedPlayer == null)
        {
            return;
        }

        PhotonView playerView = PhotonView.Find(playerViewId);
        if (playerView != null)
        {
            detectedPlayer = playerView.transform;

            float distanceToPlayer = Vector3.Distance(transform.position, detectedPlayer.position);
            if (distanceToPlayer > agent.stoppingDistance)
            {
                return;
            }

            isAttacking = true;

            Vector3 directionToPlayer = (detectedPlayer.position - transform.position).normalized;
            directionToPlayer.y = 0;
            transform.rotation = Quaternion.LookRotation(directionToPlayer);

            agent.isStopped = true;

            animator.SetTrigger("isAttack");
            currentState = MonsterState.Attack;

            StartCoroutine(HandlePostAttack());
        }
    }

    [PunRPC]
    public void TriggerWatch()
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
        if (!isDetect && !isAttacking)
        {
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
                    photonView.RPC("TriggerAttack", RpcTarget.All, collider.transform.parent.GetComponent<PhotonView>().ViewID);
                    Debug.Log($"Player detected. Wall count: {wallCount}, Adjusted radius: {adjustRad}");
                    agent.SetDestination(detectedPlayer.position);

                    SwitchToRunAfterIdle();

                    return;
                }
            }
            detectedPlayer = null;
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
        if (photonView.IsMine)
        {
            DetectPlayer();

            if (isChasing && currentTarget.HasValue)
            {
                agent.SetDestination(currentTarget.Value);

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    StopChasing();
                }
            }

            switch (currentState)
            {
                case MonsterState.Patrol:
                    //Debug.Log("Monster state is patrol");
                    SoundManager.Instance.PlayerFootstep(0.2f, "SoundMonster_Walk", transform);
                    animator.SetInteger("isWalking", 0);
                    break;
                case MonsterState.Chase:
                    //Debug.Log("Monster state is chase");
                    SoundManager.Instance.PlayerFootstep(0.1f, "SoundMonster_Walk", transform);
                    animator.SetInteger("isWalking", 1);
                    break;
                case MonsterState.Idle:
                    //Debug.Log("Monster state is idle");
                    break;
                case MonsterState.Attack:
                    //Debug.Log("Monster state is attack");
                    break;
                default:
                    //Debug.Log("Monster state is null");
                    break;
            }
        }
    }
}
