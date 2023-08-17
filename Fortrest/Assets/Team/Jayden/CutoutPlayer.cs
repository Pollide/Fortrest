using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutPlayer : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;

    [SerializeField]
    private LayerMask wallMask;

    private Camera cutoutMainCamera;

    public float CutoutSize = 0.1f;
    public float FalloffSize = 0.05f;


    // Start is called before the first frame update
    private void Awake()
    {
        cutoutMainCamera = GetComponent<Camera>();  
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 cutoutPos = cutoutMainCamera.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        for (int i=0; i< hitObjects.Length; ++i)
        {
            Debug.Log("ObjectDoBeHit");
            Material[] materials = hitObjects[i].transform.GetComponent<MeshRenderer>().materials;
            

            for (int m = 0; m < materials.Length; ++m)
            {
                materials[m].SetVector("_CutoutPos", cutoutPos);
                materials[m].SetFloat("_CutoutSize", 0.1f);
                materials[m].SetFloat("_FalloffSize", 0.05f);
            }
        }
    }
}
