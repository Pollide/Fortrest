using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BridgeBuilder : MonoBehaviour
{
    [SerializeField] private Material BPMat;
    private Material originalMat;
    public bool isBuilt = false;
    public string sceneToSpawn;
    [SerializeField] private bool hasRun = false;
    public SerializableDictionary<string, int> resourceCostList = new SerializableDictionary<string, int>();

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
        if (sceneToSpawn == "Tussockland" && BPMat != null && !GameManager.global.unlockTussock)
        {
            GetComponent<MeshRenderer>().material = BPMat;
        }
        if (sceneToSpawn == "Marsh" && BPMat != null && !GameManager.global.unlockMarsh)
        {
            GetComponent<MeshRenderer>().material = BPMat;
        }
    }

    private void Update()
    {
        if (isBuilt && !hasRun)
        {
            GetComponent<MeshRenderer>().material = originalMat;

            InventoryManager inventory = InventoryManager.global;
            for (int i = 0; i < resourceCostList.Count; i++)
            {
                inventory.RemoveItem(resourceCostList.Keys[i], resourceCostList.Values[i]);
            }

            if (sceneToSpawn == "Tussockland")
            {
                GameManager.global.unlockTussock = true;
            }
            if (sceneToSpawn == "Marsh")
            {
                GameManager.global.unlockMarsh = true;
            }

            hasRun = true;
        }
    }
}
