using System.Collections;
using System.Collections.Generic;
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
    private Animator animator;
    private MonsterState currentState;

    private NavMeshAgent agent;
    private List<Vector3> heardSounds = new List<Vector3>();
    private bool isChasing = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();
        SetWalkingState(0); // Walking
    }

    public void OnSoundHeard(Vector3 soundPos)
    {
        heardSounds.Add(soundPos);
        UpdateClosestSound();
    }

    private void UpdateClosestSound()
    {
        if (heardSounds.Count > 0)
        {
            Vector3 closestSound = heardSounds[0];
            float closestDistance = Vector3.Distance(transform.position, closestSound);

            foreach (var sound in heardSounds)
            {
                float distance = Vector3.Distance(transform.position, sound);
                if (distance < closestDistance)
                {
                    closestSound = sound;
                    closestDistance = distance;
                }
            }

            agent.SetDestination(closestSound);
            isChasing = true;
            heardSounds.Clear();
        }
    }

    public void SetWalkingState(int walkingState)
    {
        animator.SetInteger("isWalking", walkingState);

        if (walkingState == 1 && currentState != MonsterState.Run)
        {
            // 뛰기 전에 Idle을 잠시 거쳐 가도록 설정
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

    public void TriggerAttack()
    {
        animator.SetTrigger("isAttack");
        currentState = MonsterState.Attack;
    }

    public void TriggerWatch()
    {
        animator.SetTrigger("isWatching");
        currentState = MonsterState.Watch;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetWalkingState(0); // Walking
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            SetWalkingState(1); // Running
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            TriggerAttack(); // Attack
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            TriggerWatch(); // Watch
        }

        if (isChasing)
        {
            Debug.Log("Monster is chasing");
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                isChasing = false;
            }
        }
    }
}
