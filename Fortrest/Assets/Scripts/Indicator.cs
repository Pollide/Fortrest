using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    public static Indicator global;

    public GameObject arrowPrefab;

    public List<IndicatorData> IndicatorList = new List<IndicatorData>();

    public Sprite RemovedSprite;

    public Sprite HomeSprite;
    public Sprite MountSprite;

    public int bottomRightStack;
    public int bottomLeftStack;

    public int topRightStack;
    public int topLeftStack;
    public int ShiftAmount = 13;
    public int TextMulti = -25;


    [System.Serializable]
    public class IndicatorData
    {
        public ArrowData MainData;
        public ArrowData MapData;
        public Sprite CustomSprite;
        public Transform ActiveTarget;
        public bool AppearBool = true;
        public float DestroyedTimerFloat = -1;

        public Vector3 WorldPosition;
        public Vector3 Offset;
        public bool Permenant;
        public bool Visible;
        public string ActiveString;
        public Color ActiveColor;

        public bool leftBool;
        public bool rightBool;
        public bool topBool;
        public bool bottomBool;
        public bool isOutsideCanvas;
        public Terrain onTerrain;
        public float ignoreX;
        public float ignoreY;
        public void Refresh()
        {
            if (ActiveTarget)
            {
                WorldPosition = ActiveTarget.position;

            }

            Vector3 worldToScreenPointVector = LevelManager.global.SceneCamera.WorldToScreenPoint(WorldPosition);

            RectTransform canvasRect = MainData.transform.parent as RectTransform;

            //this only works if the canvas is attached as a child of the camera. Took me a long time to figure that out
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, worldToScreenPointVector, LevelManager.global.SceneCamera, out Vector2 pointVector);

            //  rectTransform.gameObject.SetActive(worldToScreenPointVector.z > 0);

            //  Vector2 velocity = pointVector - rectTransform.anchoredPosition;
            Vector2 canvasSize = canvasRect.sizeDelta;
            Vector2 imagePosition = MainData.MainRect.anchoredPosition;
            // Calculate the boundaries of the canvas
            float leftBoundary = -canvasSize.x / 2;
            float rightBoundary = canvasSize.x / 2;
            float bottomBoundary = -canvasSize.y / 2;
            float topBoundary = canvasSize.y / 2;

            float closeFloat = 0.1f;

            leftBool = imagePosition.x <= leftBoundary + closeFloat;
            rightBool = imagePosition.x >= rightBoundary - closeFloat;
            topBool = imagePosition.y >= topBoundary - closeFloat;
            bottomBool = imagePosition.y <= bottomBoundary + closeFloat;

            // Check if the image is outside the canvas boundaries
            isOutsideCanvas = leftBool || rightBool || bottomBool || topBool;

            // Clamp the image's position within the canvas boundaries
            float clampedX = Mathf.Clamp(pointVector.x, leftBoundary, rightBoundary);
            float clampedY = Mathf.Clamp(pointVector.y, bottomBoundary, topBoundary);

            Vector2 clamp = new Vector2(clampedX, clampedY);



            MainData.transform.localEulerAngles = Vector3.zero;

            Vector3 transition = Vector3.zero;


            if (rightBool)
            {
                //  if (!CustomSprite)
                MainData.transform.localEulerAngles = new Vector3(0, 0, 90);
                transition -= Vector3.right;
                MainData.ArrowText.alignment = TextAnchor.MiddleRight;
            }
            else if (leftBool)
            {
                //if (!CustomSprite)
                MainData.transform.localEulerAngles = new Vector3(0, 0, -90);
                transition += Vector3.right;
                MainData.ArrowText.alignment = TextAnchor.MiddleLeft;
            }
            else
            {
                MainData.ArrowText.alignment = TextAnchor.MiddleCenter;
            }

            if (topBool)
            {
                //  if (!CustomSprite)
                MainData.transform.localEulerAngles = new Vector3(0, 0, 180);
                transition -= Vector3.up;
            }


            if ((bottomBool || topBool) && (leftBool || rightBool))
            {
                //  Debug.Log(1);
                MainData.transform.localEulerAngles += new Vector3(0, 0, 45 * (rightBool ? -1 : 1));
            }

            if (CustomSprite)
            {
                MainData.CustomImage.transform.localEulerAngles = -MainData.transform.localEulerAngles;
            }

            float distance = Vector3.Distance(PlayerController.global.transform.position, WorldPosition);
            bool closeBool = distance < 22;

            if (closeBool || Visible)
            {
                Visible = true;

                if (!CustomSprite)
                    MainData.ArrowText.text = ActiveString;

                MainData.ArrowText.color = ActiveColor;
                MainData.ArrowImage.color = ActiveColor;

                if (MapData)
                {
                    if (!CustomSprite)
                        MapData.ArrowText.text = ActiveString;

                    MapData.ArrowText.color = ActiveColor;

                    MapData.CustomImage.gameObject.SetActive(MapData.CustomImage);
                }


            }

            bool active = !onTerrain ? !closeBool : onTerrain == LevelManager.global.currentTerrainData.terrain;

            if (active != AppearBool)
            {
                AppearBool = active;

                GameManager.PlayAnimation(MainData.GetComponent<Animation>(), "Arrow Appear", active);
            }

            MainData.ArrowText.transform.localEulerAngles = -MainData.transform.localEulerAngles; //so text is always readable

            MainData.MainRect.anchoredPosition = clamp;
            //MainData.MainRect.anchoredPosition = Vector3.Slerp(MainData.MainRect.anchoredPosition, clamp, Time.deltaTime);

            if (MapData)
                MapData.MainRect.anchoredPosition = PlayerController.global.ConvertToMapCoordinates(WorldPosition);

            MainData.HolderTransform.localPosition = Vector3.Slerp(MainData.HolderTransform.localPosition, transition, Time.deltaTime);


            int shift = 0;

            for (int i = 0; i < global.IndicatorList.Count; i++)
            {
                IndicatorData data = global.IndicatorList[i];

                if (data != this && isOutsideCanvas && (leftBool || rightBool) && (bottomBool || topBool)) //needs to be in the corner
                {
                    if (data.isOutsideCanvas && AppearBool && data.AppearBool && topBool == data.topBool && bottomBool == data.bottomBool && leftBool == data.leftBool && rightBool == data.rightBool)
                        shift += global.ShiftAmount;
                }
                else
                {
                    break;
                }
            }

            float flip = (bottomBool && leftBool ? -1 : 1);

            Offset = new Vector3(ignoreX != 0 ? ignoreX : shift * flip, ignoreY != 0 ? ignoreY : shift, 0) + (leftBool || rightBool ? transition * global.TextMulti * flip : Vector3.zero);
            // shift = 0;

            MainData.CustomImage.transform.localPosition = MainData.CustomImageLocalPosition + Offset;
            MainData.ArrowText.transform.localPosition = MainData.ArrowTextLocalPosition + Offset;
        }
    }



    void Awake()
    {
        global = this;
    }



    private void FixedUpdate()
    {
        if (!PlayerModeHandler.global)
        {
            return;
        }

        bottomRightStack = 0;
        bottomLeftStack = 0;

        topRightStack = 0;
        topLeftStack = 0;

        // GetComponent<Canvas>().enabled = !PlayerModeHandler.global.inTheFortress;
        for (int i = 0; i < IndicatorList.Count; i++)
        {
            bool enemy = IndicatorList[i].ActiveTarget && IndicatorList[i].ActiveTarget.GetComponent<EnemyController>() && IndicatorList[i].ActiveTarget.GetComponent<EnemyController>().health <= 0;
            bool boss = IndicatorList[i].ActiveTarget && IndicatorList[i].ActiveTarget.GetComponent<BossSpawner>() && IndicatorList[i].ActiveTarget.GetComponent<BossSpawner>().health <= 0;

            if (!IndicatorList[i].ActiveTarget || enemy || boss)
            {
                if (IndicatorList[i].DestroyedTimerFloat == -1)
                {
                    IndicatorList[i].MainData.ArrowImage.sprite = RemovedSprite;

                    IndicatorList[i].DestroyedTimerFloat = 5;

                    if (IndicatorList[i].AppearBool)
                        GameManager.PlayAnimation(IndicatorList[i].MainData.GetComponent<Animation>(), "Arrow Appear", false);
                }

                IndicatorList[i].DestroyedTimerFloat -= Time.fixedDeltaTime;

                if (IndicatorList[i].DestroyedTimerFloat <= 0)
                {
                    Destroy(IndicatorList[i].MainData.gameObject);

                    if (IndicatorList[i].MapData)
                        Destroy(IndicatorList[i].MapData.gameObject);
                    IndicatorList.RemoveAt(i);

                    continue;
                }
            }

            IndicatorList[i].Refresh();
        }
    }

    public void AddIndicator(Transform activeTarget, Color color, string nameString, bool permenant = true, Sprite customSprite = null)
    {
        IndicatorData indicatorData = new IndicatorData();

        indicatorData.ActiveTarget = activeTarget;

        indicatorData.MainData = Instantiate(arrowPrefab, transform).GetComponent<ArrowData>();

        indicatorData.MainData.ArrowText.text = "?";

        indicatorData.ActiveString = nameString;
        indicatorData.ActiveColor = color;

        indicatorData.Permenant = permenant;
        indicatorData.Visible = permenant;

        indicatorData.CustomSprite = customSprite;

        indicatorData.MainData.ArrowTextLocalPosition = indicatorData.MainData.ArrowText.transform.localPosition;
        indicatorData.MainData.CustomImageLocalPosition = indicatorData.MainData.CustomImage.transform.localPosition;

        IndicatorList.Add(indicatorData);

        if (!permenant && Physics.Raycast(activeTarget.transform.position + Vector3.up * 2, -Vector3.up, out RaycastHit raycastHit, Mathf.Infinity, GameManager.ReturnBitShift(new string[] { "Terrain" })))
        {
            indicatorData.onTerrain = raycastHit.transform.GetComponent<Terrain>();
        }

        if (!permenant || customSprite)
        {
            indicatorData.MapData = Instantiate(arrowPrefab, PlayerController.global.MapSpotHolder).GetComponent<ArrowData>();
            indicatorData.MapData.GetComponent<Animation>().enabled = false;
            indicatorData.MapData.transform.localScale *= 2.0f;
            indicatorData.MapData.ArrowImage.gameObject.SetActive(false);

            // indicatorData.MapData.CustomImage.gameObject.SetActive(true);

            //   RectTransform imageTransform = indicatorData.MapData.CustomImage.GetComponent<RectTransform>();
            //indicatorData.MapData.ArrowText.GetComponent<RectTransform>().anchoredPosition = imageTransform.anchoredPosition;
            indicatorData.MapData.ArrowText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            indicatorData.MapData.CustomImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            // indicatorData.MapData.CustomImage.color = color;

            indicatorData.MapData.ArrowText.text = indicatorData.MainData.ArrowText.text;

            if (customSprite)
            {
                indicatorData.MainData.CustomImage.sprite = customSprite;
                indicatorData.MapData.CustomImage.sprite = customSprite;

                indicatorData.MainData.ArrowText.text = "";
                indicatorData.MapData.ArrowText.text = "";

                if (permenant)
                {
                    indicatorData.MainData.CustomImage.color = color;
                    indicatorData.MapData.CustomImage.color = color;
                }
            }
        }
    }
}
