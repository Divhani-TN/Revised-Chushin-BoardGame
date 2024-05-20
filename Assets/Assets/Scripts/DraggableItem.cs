using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    [HideInInspector] public Transform parentBeforeDrag;
    [HideInInspector] public Transform parentAfterDrag;
    
    private bool isInteractable = true;
    private bool hasMoved = false;

    public void SetInteractable(bool interactable)
    {
        if (!hasMoved)
        {
            isInteractable = interactable;
            image.raycastTarget = interactable;   
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInteractable || hasMoved) return;
        
        //parentBeforeDrag = transform.parent;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInteractable || hasMoved) return;
        
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isInteractable || hasMoved) return;
        
        //if (parentBeforeDrag != parentAfterDrag)
        //{
            Debug.Log("Change turns");
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
            
            hasMoved = true;
            SetInteractable(false);
        //}
        //else if (parentBeforeDrag == parentAfterDrag)
        //{
            //Debug.Log("Don't change turns");
            //transform.SetParent(parentBeforeDrag);
            //image.raycastTarget = true;
        //}
        
        GameManager.instance.SwitchTurn();
    }
}
