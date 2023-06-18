using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BridgeBuilder : MonoBehaviour
{
    [SerializeField] private Material BPMat;
    private Material originalMat;

    private void Awake()
    {
        if (GetComponent<MeshRenderer>())
        {
            originalMat = GetComponent<MeshRenderer>().material;
        }
        else
        {
            Debug.Log("No Mesh Renderer attached");
        }
    }

    private void Start()
    {
        if (BPMat != null)
        {
            GetComponent<MeshRenderer>().material = BPMat;
        }
    }
}
