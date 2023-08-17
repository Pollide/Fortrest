using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGargoyleStand : MonoBehaviour
{
    // This function allows a single prefab to be used instead of multiple, allowing for a change in rendered assets. 

    //Bools
    public bool ShowAsset = false;
    public bool IsLoweredLip = false;

    //Components
    public MeshRenderer AssetMeshRenderer;
    public MeshFilter AssetMeshFilter;
    public MeshCollider AssetMeshCollider;

    //Stored Meshes
    public Mesh LoweredLipVariation;
    public Mesh Base;
    public Mesh DefaultCollider;
    public Mesh LoweredLipCollider;


    //Start Function
    void Start()
    {
        
        if (ShowAsset == true) //Checks the status of the bool
        {
            AssetMeshRenderer.enabled = true; //If the bool is true, the mesh is rendered
        } 
        else //Else to previous if
        {
            AssetMeshRenderer.enabled = false; //If the bool if false, the mesh is not rendered.
        }

        if (IsLoweredLip == true)
        {
            AssetMeshFilter.mesh = LoweredLipVariation;
            AssetMeshCollider.sharedMesh = LoweredLipCollider;
        }
        else
        {
            AssetMeshFilter.mesh = Base;
            AssetMeshCollider.sharedMesh = DefaultCollider;
        }
    }    
}
