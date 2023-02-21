/*********************************************************************
Bachelor of Software Engineering
Media Design School
Auckland
New Zealand
(c) 2023 Media Design School
File Name : Building.cs
Description : Sets up the building mechanics such as shooting enemies
Author :  Allister Hamilton
Mail : allister.hamilton @mds.ac.nz
**************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{

    Vector3 PreviousPos;
    Vector3 screenPoint;
    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        //Add a rigidbody to the building so the mouse raycasthit will return the top parent.
        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true; //prevents any forces acting upon it
        rigidbody.useGravity = false; //prevents the object from being affected by gravity

        //grabs all children transforms including itself by finding the component it matches, as I put transform there, it will just grab all of them
        List<Transform> transformList = GameManager.FindComponent<Transform>(transform);

        //create a for loop from the list
        for (int i = 0; i < transformList.Count; i++)
        {
            //sets all transforms to the building layer, so the mouse will easily be able to click the building
            transformList[i].gameObject.layer = LayerMask.NameToLayer("Building");
        }

        LevelManager.global.TargetsList.Add(transform);
    }

    public void OnMouseDown()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0))
        {
            PreviousPos = transform.position;
            screenPoint = Camera.main.WorldToScreenPoint(PreviousPos);
            offset = PreviousPos - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        }
    }
    public void OnMouseDrag()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            curPosition = new Vector3(Mathf.Round(curPosition.x), transform.position.y, Mathf.Round(curPosition.z));
            transform.position = curPosition;

            PreviousPos = transform.position;
        }
    }
}
