using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotHandler : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            DragableItem dragableItem = dropped.GetComponent<DragableItem>();
            dragableItem.parentAfterDrag = transform;
        }
        else
        {
            GameObject dropped = eventData.pointerDrag;
            DragableItem dragableItem = dropped.GetComponent<DragableItem>();
            DragableItem dragableItem2 = gameObject.GetComponentInChildren<DragableItem>();
            dragableItem2.transform.SetParent(dragableItem.parentAfterDrag);
            dragableItem.parentAfterDrag = transform;
        }
    }
}
