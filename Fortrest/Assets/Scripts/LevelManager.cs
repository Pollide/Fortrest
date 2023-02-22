using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    public static LevelManager global;

    public Camera SceneCamera;

    public float PanSpeed = 20f;
    public float ZoomSpeedTouch = 0.1f;
    public float ZoomSpeedMouse = 0.5f;

    public float[] BoundsX = new float[] { -10f, 5f };
    public float[] BoundsZ = new float[] { -18f, -4f };
    public float[] ZoomBounds = new float[] { 10f, 85f };
    RaycastHit BuildingHit;

    Vector3 lastPanPosition;
    bool OnceDetection;

    public List<Transform> TargetsList = new List<Transform>();
    public List<Transform> buildingList = new List<Transform>();

    private void Awake()
    {
        global = this;

        if (!GameManager.global)
        {
            PlayerPrefs.SetInt("Quick Load", SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.global.NextScene(0);
            enabled = false;
            return;
        }

        HandleMouse();

        HandleBuildingVoid();
    }

    void HandleBuildingVoid()
    {
        if (Input.GetMouseButton(0))
        {
            if (BuildingHit.transform || !EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject == null)
            {
                // Vector3 cameraPositionVector = SceneCamera.ScreenPointToRay(Input.mousePosition).p;

                if (Physics.Raycast(SceneCamera.ScreenPointToRay(Input.mousePosition), out BuildingHit, Mathf.Infinity, GameManager.ReturnBitShift(new string[] { "Building" })))
                {
                    //  BuildingHit.transform.position = new Vector3(BuildingHit.point.x, BuildingHit.transform.position.y, BuildingHit.point.z); //move building to mouse position
                    //  BuildingHit.transform.GetComponent<Building>().DragBuildingVoid(                                                                                                                     // CurrentUnitSelected.transform.parent.position = new Vector3(Input.mousePosition.x, 0f, Input.mousePosition.z); //Position
                }
            }
        }
        else
        {
            BuildingHit = new RaycastHit();
        }
    }
    void HandleMouse()
    {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            PanCamera(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.Minus))
        {
            scroll = -0.1f;
        }
        if (Input.GetKey(KeyCode.Equals))
        {
            scroll = 0.1f;
        }

        if (scroll != 0)
        {
            SceneCamera.orthographicSize = Mathf.Clamp(SceneCamera.orthographicSize - (scroll * ZoomSpeedMouse), ZoomBounds[0], ZoomBounds[1]);
        }

        PanSpeed = SceneCamera.orthographicSize / 2.5f;
    }

    void PanCamera(Vector3 newPanPosition)
    {
        // Determine how much to move the camera

        Vector3 offset = SceneCamera.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(offset.x * PanSpeed, 0, offset.y * PanSpeed);

        if (!OnceDetection)
        {
            OnceDetection = true;
            float MaxMovement = 0.5f;
            float DefaultMovement = 0.1f;
            if (move.x > MaxMovement)
            {
                move.x = DefaultMovement;
            }
            if (move.z > MaxMovement)
            {
                move.z = DefaultMovement;
            }

            if (move.x < -MaxMovement)
            {
                move.x = -DefaultMovement;
            }
            if (move.z < -MaxMovement)
            {
                move.z = -DefaultMovement;
            }
        }

        SceneCamera.transform.Translate(move, Space.World);

        //Debug.Log("POS: " + move + " |  SceneCamera.transform: " +  SceneCamera.transform.position);

        // Ensure the camera remains within bounds.
        Vector3 pos = SceneCamera.transform.position;
        pos.x = Mathf.Clamp(SceneCamera.transform.position.x, BoundsX[0], BoundsX[1]);
        pos.z = Mathf.Clamp(SceneCamera.transform.position.z, BoundsZ[0], BoundsZ[1]);

        SceneCamera.transform.position = pos;
        // Cache the position
        lastPanPosition = newPanPosition;
    }
}
