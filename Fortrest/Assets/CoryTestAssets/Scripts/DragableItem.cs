using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;

public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Image image;
    // Name of the item
    public new string name;
    public TMP_Text quantityText;

    public UnityEvent middleClick;
    public UnityEvent rightClick;

    [HideInInspector] public Transform parentAfterDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.parent.parent, false);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
        GetComponent<Animator>().enabled = true;
        // GameManager.global.SoundManager.PlaySound(GameManager.global.InventoryClickSound);
        transform.position = Input.mousePosition;
    }


    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Animator>().enabled = false;
        transform.localScale = Vector3.one;
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        //  GameManager.global.SoundManager.PlaySound(GameManager.global.InventoryClickSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.global.SoundManager.PlaySound(GameManager.global.InventoryClickSound);

        if (eventData.button == PointerEventData.InputButton.Right && rightClick != null)
            rightClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Middle && middleClick != null)
            middleClick.Invoke();
    }
}
