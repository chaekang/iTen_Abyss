using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

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
