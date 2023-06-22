using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    // Name of the item
    public new string name;
    public TMP_Text quantityText;

    [HideInInspector] public Transform parentAfterDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.parent.parent);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
        GetComponent<Animator>().enabled = true;
        image.enabled = false; //hide for a frame so the instant position is not visible
    }


    public void OnDrag(PointerEventData eventData)
    {
        image.enabled = true; //unhide
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.enabled = true; //unhide
        GetComponent<Animator>().enabled = false;
        transform.localScale = Vector3.one;
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}
