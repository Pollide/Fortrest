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
        public bool Unlocked;
        public bool Recent;
        public string ActiveString;

        public bool leftBool;
        public bool rightBool;
        public bool topBool;
        public bool bottomBool;
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
            bool isOutsideCanvas = leftBool || rightBool || bottomBool || topBool;

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

            }

            if (leftBool)
            {
                //if (!CustomSprite)
                MainData.transform.localEulerAngles = new Vector3(0, 0, -90);
                transition += Vector3.right;
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

            bool close = distance < 22;

            if (close)
            {
                if (!Unlocked)
                    Recent = true;

                Unlocked = true;

            }
            else
            {
                Recent = false;
            }

            if (Unlocked && !CustomSprite)
            {
                MainData.ArrowText.text = ActiveString;

                if (MapData)
                {
                    MapData.ArrowText.text = MainData.ArrowText.text;
                }
            }

            bool active = (Unlocked || distance < 240) && (Recent || !close);

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

                if (data != this)
                {
                    if (AppearBool && data.AppearBool && topBool == data.topBool && bottomBool == data.bottomBool && leftBool == data.leftBool && rightBool == data.rightBool)
                        shift += 13;
                }
                else
                {
                    break;
                }
            }

            Offset = new Vector3(shift, shift, 0);
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
            if (!IndicatorList[i].ActiveTarget)
            {
                if (IndicatorList[i].DestroyedTimerFloat == -1)
                {
                    IndicatorList[i].MainData.ArrowImage.sprite = RemovedSprite;

                    IndicatorList[i].DestroyedTimerFloat = 10;

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

    public void AddIndicator(Transform activeTarget, Color color, string nameString, bool unlocked = true, Sprite customSprite = null)
    {
        IndicatorData indicatorData = new IndicatorData();

        indicatorData.ActiveTarget = activeTarget;

        indicatorData.MainData = Instantiate(arrowPrefab, transform).GetComponent<ArrowData>();
        indicatorData.MainData.ArrowImage.color = color;
        indicatorData.MainData.CustomImage.color = color;
        indicatorData.MainData.ArrowText.text = "?";

        indicatorData.ActiveString = nameString;
        indicatorData.Unlocked = unlocked;
        indicatorData.CustomSprite = customSprite;

        indicatorData.MainData.ArrowTextLocalPosition = indicatorData.MainData.ArrowText.transform.localPosition;
        indicatorData.MainData.CustomImageLocalPosition = indicatorData.MainData.CustomImage.transform.localPosition;

        IndicatorList.Add(indicatorData);

        if (!unlocked || customSprite)
        {
            indicatorData.MapData = Instantiate(arrowPrefab, PlayerController.global.MapSpotHolder).GetComponent<ArrowData>();
            indicatorData.MapData.GetComponent<Animation>().enabled = false;
            indicatorData.MapData.transform.localScale *= 2.0f;
            indicatorData.MapData.ArrowImage.gameObject.SetActive(false);
            indicatorData.MapData.CustomImage.gameObject.SetActive(true);

            indicatorData.MapData.CustomImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            indicatorData.MapData.CustomImage.color = color;

            indicatorData.MapData.ArrowText.text = indicatorData.MainData.ArrowText.text;

            if (customSprite)
            {
                indicatorData.MainData.CustomImage.gameObject.SetActive(true);

                indicatorData.MainData.CustomImage.sprite = customSprite;
                indicatorData.MapData.CustomImage.sprite = customSprite;

                indicatorData.MainData.ArrowText.text = "";
                indicatorData.MapData.ArrowText.text = "";
            }
        }
    }
}
