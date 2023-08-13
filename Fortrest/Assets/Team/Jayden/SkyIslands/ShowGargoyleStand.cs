using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGargoyleStand : MonoBehaviour
{
    // This function allows a single prefab to be used instead of multiple, allowing for a change in rendered assets. 

    //Bools
    public bool ShowAsset = false;
    public bool IsWaterfall = false;

    //Components
    public MeshRenderer AssetMeshRenderer;
    public MeshFilter AssetMeshFilter;

    //Stored Meshes
    public Mesh WaterfallVariation;
    public Mesh Base;


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

        if (IsWaterfall == true)
        {
            AssetMeshFilter.mesh = WaterfallVariation;
        }
        else
        {
            AssetMeshFilter.mesh = Base;
        }
    }

    
    
}
