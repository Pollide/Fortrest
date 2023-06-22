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

    [System.Serializable]
    public class IndicatorData
    {
        public RectTransform rectTransform;
        public Text ActiveText;

        public Transform target;

        public Transform holder;


        public void Refresh()
        {
            Vector3 worldToScreenPointVector = LevelManager.global.SceneCamera.WorldToScreenPoint(target.position);

            RectTransform canvasRect = rectTransform.transform.parent as RectTransform;

            //this only works if the canvas is attached as a child of the camera. Took me a long time to figure that out
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, worldToScreenPointVector, LevelManager.global.SceneCamera, out Vector2 pointVector);

            //  rectTransform.gameObject.SetActive(worldToScreenPointVector.z > 0);

            //  Vector2 velocity = pointVector - rectTransform.anchoredPosition;
            Vector2 canvasSize = canvasRect.sizeDelta;
            Vector2 imagePosition = rectTransform.anchoredPosition;
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

            rectTransform.localEulerAngles = Vector3.zero;
            holder.localPosition = Vector2.zero;

            if (rightBool)
            {
                rectTransform.localEulerAngles = new Vector3(0, 0, 90);
                holder.localPosition -= Vector3.right;
            }

            if (leftBool)
            {
                rectTransform.localEulerAngles = new Vector3(0, 0, -90);
                holder.localPosition += Vector3.right;
            }

            if (topBool)
            {
                rectTransform.localEulerAngles = new Vector3(0, 0, 180);
                holder.localPosition -= Vector3.up;
            }

            ActiveText.transform.localEulerAngles = -rectTransform.localEulerAngles; //so text is always readable

            rectTransform.anchoredPosition = clamp;
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
            if (IndicatorList[i].target)
            {
                IndicatorList[i].Refresh();
            }
            else
            {
                IndicatorList.RemoveAt(i);
            }
        }
    }

    public void AddIndicator(Transform target, Color color, string name)
    {
        IndicatorData indicatorData = new IndicatorData();

        indicatorData.target = target;

        indicatorData.rectTransform = Instantiate(arrowPrefab, transform).GetComponent<RectTransform>();

        indicatorData.holder = indicatorData.rectTransform.transform.GetChild(0);

        indicatorData.ActiveText = indicatorData.holder.GetChild(1).GetComponent<Text>();
        indicatorData.holder.GetChild(0).GetComponent<Image>().color = color;

        indicatorData.ActiveText.text = name;
        IndicatorList.Add(indicatorData);
    }
}
