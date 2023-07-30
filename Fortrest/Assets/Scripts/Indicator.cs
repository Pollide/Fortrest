using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    public static Indicator global;

    public GameObject arrowPrefab;

    public List<IndicatorData> IndicatorList = new List<IndicatorData>();

    public Vector3 offsets = Vector3.zero;

    public Sprite RemovedSprite;

    public Sprite HomeSprite;


    [System.Serializable]
    public class IndicatorData
    {
        public ArrowData MainData;
        public ArrowData MapData;

        public Transform ActiveTarget;

        public float DestroyedTimerFloat = -1;

        public Vector3 WorldPosition;

        public bool Unlocked;

        public string ActiveString;

        public void Refresh()
        {
            if (ActiveTarget)
                WorldPosition = ActiveTarget.position;

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

            bool leftBool = imagePosition.x <= leftBoundary + closeFloat;
            bool rightBool = imagePosition.x >= rightBoundary - closeFloat;
            bool topBool = imagePosition.y >= topBoundary - closeFloat;
            bool bottomBool = imagePosition.y <= bottomBoundary + closeFloat;

            // Check if the image is outside the canvas boundaries
            bool isOutsideCanvas = leftBool || rightBool || bottomBool || topBool;

            // Clamp the image's position within the canvas boundaries
            float clampedX = Mathf.Clamp(pointVector.x, leftBoundary, rightBoundary);
            float clampedY = Mathf.Clamp(pointVector.y, bottomBoundary, topBoundary);

            Vector2 clamp = new Vector2(clampedX, clampedY);

            MainData.transform.localEulerAngles = Vector3.zero;
            MainData.HolderTransform.localPosition = Vector2.zero;

            if (rightBool)
            {
                MainData.transform.localEulerAngles = new Vector3(0, 0, 90);
                MainData.HolderTransform.localPosition -= Vector3.right;
            }

            if (leftBool)
            {
                MainData.transform.localEulerAngles = new Vector3(0, 0, -90);
                MainData.HolderTransform.localPosition += Vector3.right;
            }

            if (topBool)
            {
                MainData.transform.localEulerAngles = new Vector3(0, 0, 180);
                MainData.HolderTransform.localPosition -= Vector3.up;
            }

            if ((bottomBool || topBool) && (leftBool || rightBool))
            {
                //  Debug.Log(1);
                MainData.transform.localEulerAngles += new Vector3(0, 0, 45 * (rightBool ? -1 : 1));
            }

            if (!Unlocked)
            {
                float distance = Vector3.Distance(PlayerController.global.transform.position, ActiveTarget.position);

                if (distance < 20)
                {
                    Unlocked = true;
                }

                MainData.gameObject.SetActive(Unlocked || distance < 60);
            }
            else
            {
                MainData.ArrowText.text = ActiveString;

            }

            MainData.ArrowText.transform.localEulerAngles = -MainData.transform.localEulerAngles; //so text is always readable

            MainData.MainRect.anchoredPosition = clamp;
            MapData.MainRect.anchoredPosition = PlayerController.global.ConvertToMapCoordinates(WorldPosition);
        }
    }

    void Awake()
    {
        global = this;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < IndicatorList.Count; i++)
        {
            if (!IndicatorList[i].ActiveTarget)
            {
                if (IndicatorList[i].DestroyedTimerFloat == -1)
                {
                    IndicatorList[i].MainData.ArrowImage.sprite = RemovedSprite;

                    IndicatorList[i].DestroyedTimerFloat = 10;
                    IndicatorList[i].MapData.gameObject.SetActive(false);
                    GameManager.PlayAnimation(IndicatorList[i].MainData.GetComponent<Animation>(), "Arrow Appear", false);
                }

                IndicatorList[i].DestroyedTimerFloat -= Time.fixedDeltaTime;

                if (IndicatorList[i].DestroyedTimerFloat <= 0)
                {
                    IndicatorList.RemoveAt(i);

                    continue;
                }
            }

            IndicatorList[i].Refresh();
        }
    }

    public void AddIndicator(Transform activeTarget, Color color, string nameString, bool unlocked = true)
    {
        IndicatorData indicatorData = new IndicatorData();

        indicatorData.ActiveTarget = activeTarget;

        indicatorData.MainData = Instantiate(arrowPrefab, transform).GetComponent<ArrowData>();
        indicatorData.MainData.ArrowImage.color = color;
        indicatorData.MainData.ArrowText.text = "?";

        indicatorData.ActiveString = nameString;
        indicatorData.Unlocked = unlocked;
        IndicatorList.Add(indicatorData);

        indicatorData.MapData = Instantiate(arrowPrefab, PlayerController.global.MapCanvasGameObject.transform).GetComponent<ArrowData>();
        indicatorData.MapData.GetComponent<Animation>().enabled = false;
        indicatorData.MapData.transform.localScale *= 3;

        indicatorData.MapData.ArrowImage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        indicatorData.MapData.ArrowImage.sprite = RemovedSprite;
        indicatorData.MapData.ArrowImage.color = color;
        indicatorData.MapData.ArrowImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);
        // indicatorData.MapData.ArrowText.text = nameString;
        indicatorData.MapData.ArrowText.text = indicatorData.MainData.ArrowText.text;

        if (nameString == "Home")
        {
            indicatorData.MapData.ArrowImage.sprite = HomeSprite;
            indicatorData.MapData.ArrowText.text = "";
        }
    }
}
