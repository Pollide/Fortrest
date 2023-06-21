using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BridgeBuilder : MonoBehaviour
{
    [SerializeField] private Material BPMat;
    private Material originalMat;
    public bool isBuilt = false;

    [SerializeField] private bool hasRun = false;
    public SerializableDictionary<string, int> resourceCostList = new SerializableDictionary<string, int>();
    public GameObject DamagedGameObject;
    public GameObject RepairedGameObject;


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

            RepairedGameObject.SetActive(true);
            DamagedGameObject.SetActive(false);

            hasRun = true;
        }
    }
}
