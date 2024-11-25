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
    public void PlaySound(Vector3 position, float range, string clipKey)
    {
        if (soundClips.ContainsKey(clipKey))
        {
            AudioClip clip = soundClips[clipKey];
            audioSource.clip = clip;
            audioSource.transform.position = position;
            audioSource.spatialBlend = 1.0f;
            audioSource.minDistance = range;
            audioSource.maxDistance = range * 2;
            audioSource.Play();

            RegisterSoundPosition(position);
            EmitSound(position, range);
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
    }

    public void EmitSound(Vector3 pos, float range)
    {
        Collider[] hitMonsters = Physics.OverlapSphere(pos, range, mosterLayer);

        foreach (Collider monster in hitMonsters)
        {
            RaycastHit hit;
            if (Physics.Linecast(pos, monster.transform.position, out hit))
            {
                if (hit.collider.CompareTag("Monster"))
                {
                    SoundMonster soundMonster = monster.GetComponent<SoundMonster>();
                    if (soundMonster != null)
                    {
                        soundMonster.OnSoundHeard(pos);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var pos in soundPos)
        {
            Gizmos.DrawWireSphere(pos, 30f);
        }
    }

    public void RegisterSoundPosition(Vector3 pos)
    {
        soundPos.Add(pos);
    }

    public void ClearSoundPositions()
    {
        soundPos.Clear();
    }
}
