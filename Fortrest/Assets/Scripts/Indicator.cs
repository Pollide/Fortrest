using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    public static Indicator global;

    public GameObject arrowPrefab;

    public List<IndicatorData> IndicatorList = new List<IndicatorData>();

    public Vector3 offsets = Vector3.one;

    [System.Serializable]
    public class IndicatorData
    {
        public RectTransform ImageRect;

        public Transform target;

        public void Refresh()
        {
            Vector2 pointVector = LevelManager.global.SceneCamera.WorldToScreenPoint(target.position);
            //   pointVector.x *= global.offsets.x;
            //   pointVector.y *= global.offsets.y;

            RectTransform canvasRect = ImageRect.parent as RectTransform;

            //RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, worldToScreenPointVector, LevelManager.global.SceneCamera, out Vector2 pointVector);


            //    ImageRect.gameObject.SetActive(worldToScreenPointVector.z > 0);

            Vector2 velocity = pointVector - ImageRect.anchoredPosition;
            Vector2 canvasSize = canvasRect.sizeDelta;
            Vector2 imagePosition = ImageRect.anchoredPosition;
            // Calculate the boundaries of the canvas
            float leftBoundary = -canvasSize.x / 2;
            float rightBoundary = canvasSize.x / 2;
            float bottomBoundary = -canvasSize.y / 2;
            float topBoundary = canvasSize.y / 2;

            bool leftBool = imagePosition.x < leftBoundary;
            bool rightBool = imagePosition.x > rightBoundary;
            bool topBool = imagePosition.y > topBoundary;
            bool bottomBool = imagePosition.y > topBoundary;

            // Check if the image is outside the canvas boundaries
            bool isOutsideCanvas = leftBool || rightBool || bottomBool || topBool;

            // Clamp the image's position within the canvas boundaries
            float clampedX = Mathf.Clamp(pointVector.x, leftBoundary, rightBoundary);
            float clampedY = Mathf.Clamp(pointVector.y, bottomBoundary, topBoundary);

            ImageRect.localEulerAngles = Vector3.zero;

            if (rightBool)
                ImageRect.localEulerAngles = new Vector3(0, 0, 90);

            if (leftBool)
                ImageRect.localEulerAngles = new Vector3(0, 0, -90);

            if (topBool)
                ImageRect.localEulerAngles = new Vector3(0, 0, 180);

            ImageRect.anchoredPosition = Vector2.SmoothDamp(ImageRect.anchoredPosition, new Vector2(clampedX, clampedY), ref velocity, 3 * Time.fixedDeltaTime);
        }
    }

    void Awake()
    {
        global = this;
    }

    private void LateUpdate()
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

    public void AddIndicator(Transform target)
    {
        return;
        IndicatorData indicatorData = new IndicatorData();

        indicatorData.target = target;

        indicatorData.ImageRect = Instantiate(arrowPrefab, transform).GetComponent<RectTransform>();

        IndicatorList.Add(indicatorData);
    }
}
