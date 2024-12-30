using System.Collections;
using System.Collections.Generic;
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
        AudioClip soundBoxClip = Resources.Load<AudioClip>("Audio/SoundBox");
        if (soundBoxClip != null)
        {
            soundClips["SoundBoxClip"] = soundBoxClip;
        }
    }

    // 음악 재생
    public void PlaySound(Transform target, float range, string clipKey)
    {
        if (soundClips.ContainsKey(clipKey))
        {
            Debug.Log("PlaySound");
            AudioClip clip = soundClips[clipKey];
            audioSource.clip = clip;
            audioSource.spatialBlend = 1.0f;
            audioSource.minDistance = range;
            audioSource.maxDistance = range * 2;
            audioSource.Play();

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

    private void Update()
    {
        if (isFollowing && followTarget != null)
        {
            audioSource.transform.position = followTarget.position;
        }
    }
}
