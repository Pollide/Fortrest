using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BridgeScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GetComponentInParent<BridgeBuilder>().sceneToSpawn == "Tussockland")
            {
                GameManager.global.NextScene(2);
            }
            if (GetComponentInParent<BridgeBuilder>().sceneToSpawn == "Game")
            {
                GameManager.global.NextScene(1);
            }
            if (GetComponentInParent<BridgeBuilder>().sceneToSpawn == "Marsh")
            {
                GameManager.global.NextScene(3);
            }

        }
    }
}
