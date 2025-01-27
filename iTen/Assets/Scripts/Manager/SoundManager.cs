using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource audioSource;
    private Dictionary<string, AudioClip> soundClips = new Dictionary<string, AudioClip>();

    public LayerMask mosterLayer;
    private List<Vector3> soundPos = new List<Vector3>();

    private bool isFollowing = false;
    private Transform followTarget;

    private float footstepTimer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

        audioSource = GetComponent<AudioSource>();
        LoadClips();
    }

    // Resources에 있는 오디오 가져와서 딕셔너리에 할당
    private void LoadClips()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio");
        foreach (AudioClip cl in clips)
        {
            soundClips.Add(cl.name, cl);
        }
    }

    public void PlayerFootstep(float interval, string name, Transform sourceTransform)
    {
        if (footstepTimer <= 0f)
        {
            float minSpeed = 1f;
            float maxSpeed = 4f;
            float minInterval = 0.2f;
            float maxInterval = 0.6f;

            float speedRatio = Mathf.InverseLerp(minSpeed, maxSpeed, interval);
            float footstepInterval = Mathf.Lerp(maxInterval, minInterval, speedRatio);

            var footstepClips = soundClips.Where(kvp => kvp.Key.StartsWith(name)).Select(kvp => kvp.Value).ToList();

            if (footstepClips.Count > 0)
            {
                int randomIndex = Random.Range(0, footstepClips.Count);

                audioSource.transform.position = sourceTransform.position;
                audioSource.spatialBlend = 1.0f;  // 3D 사운드
                audioSource.minDistance = 5f;     // 최소 거리
                audioSource.maxDistance = 20f;    // 최대 거리

                audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
                audioSource.PlayOneShot(footstepClips[randomIndex]);
            }

            footstepTimer = footstepInterval;
        }
    }

    // 음악 재생 
    public void PlaySound(Transform target, float range, string clipKey)
    {
        Debug.Log($"Playing sound '{clipKey}' at {target.position}");
        if (soundClips.ContainsKey(clipKey))
        {
            AudioClip clip = soundClips[clipKey];
            audioSource.spatialBlend = 1.0f;
            audioSource.minDistance = range;
            audioSource.maxDistance = range * 2;
            audioSource.PlayOneShot(clip);

            followTarget = target;
            isFollowing = true;

            RegisterSoundPosition(target.position);
            StartCoroutine(EmitSoundContinuously(range));
            Invoke(nameof(StopSound), 10f);
        }
        else
        {
            Debug.LogError($"Sound clip with key '{clipKey}' not found!");
        }
    }

    public void StopSound()
    {
        audioSource.Stop();
        isFollowing = false;
        followTarget = null;
    }

    public void EmitSound(Vector3 pos, float range)
    {
        Collider[] hitMonsters = Physics.OverlapSphere(pos, range, mosterLayer);

        foreach (Collider monster in hitMonsters)
        {
            SoundMonster soundMonster = monster.GetComponent<SoundMonster>();
            if (soundMonster != null)
            {
                Debug.Log("EmitSound triggered.");
                soundMonster.OnSoundHeard(pos);
            }
        }
    }

    private IEnumerator EmitSoundContinuously(float range)
    {
        while (isFollowing && followTarget != null)
        {
            EmitSound(followTarget.position, range);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var pos in soundPos)
        {
            Gizmos.DrawWireSphere(pos, 1000f);
        }
    }

    public void RegisterSoundPosition(Vector3 pos)
    {
        Debug.Log("RegisterSoundPos");
        soundPos.Add(pos);
    }

    public void PlayJumpScareSound(string name)
    {
        if (!soundClips.ContainsKey(name))
        {
            Debug.LogWarning($"Sound '{name}' not found in soundClips.");
            return;
        }

        AudioClip clip = soundClips[name];
        float originalVol = audioSource.volume;

        audioSource.volume = Mathf.Clamp(1.5f, 0f, 1f);
        audioSource.PlayOneShot(clip);

        audioSource.volume = originalVol;
    }

    public void PlayGrowlingSound(string name)
    {
        AudioClip clip = soundClips[name];
        audioSource.PlayOneShot(clip);
    }

    private void Update()
    {
        if (isFollowing && followTarget != null)
        {
            audioSource.transform.position = followTarget.position;
        }

        if (footstepTimer > 0f)
        {
            footstepTimer -= Time.deltaTime;
        }
    }
}
