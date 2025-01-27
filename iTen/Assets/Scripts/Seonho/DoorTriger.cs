using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTriger : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private LayerMask Item;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & Item) != 0)
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}