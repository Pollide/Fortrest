using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BridgeScene : MonoBehaviour
{
    [SerializeField] private GameObject unloadTerrain;
    [SerializeField] private GameObject loadTerrain;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            unloadTerrain.SetActive(false);
            loadTerrain.SetActive(true);
        }
    }
}
