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
    //private bool hasMoved = false;
    public bool hasMoved { get; private set; } = false;
    private bool isDragging = false;
    
    //public bool isInitiallyMovable = true;
    
    private void Start()
    {
        // Set interactable based on the tag
        isInteractable = !CompareTag("Player1PieceNotMovable") && !CompareTag("Player2PieceNotMovable");
        SetInteractable(isInteractable);
    }

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
        
        isDragging = true;
        parentBeforeDrag = transform.parent;
        parentAfterDrag = null;
        //parentAfterDrag = transform.parent;
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
        //if (!isInteractable || hasMoved) return;
        if (!isInteractable || hasMoved || !isDragging) return;
        
        isDragging = false;
        
        if (parentAfterDrag != null && parentAfterDrag.childCount == 1 && parentBeforeDrag != parentAfterDrag)
        {
            // Valid drop, set parent to the new parent
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
            hasMoved = true;
            SetInteractable(false);

            if (GameManager.instance != null)
            {
                GameManager.instance.PieceMoved();
            }
            else
            {
                Debug.LogError("GameManager instance is null");
            }
        }
        else
        {
            // Invalid drop, revert to the original parent
            transform.SetParent(parentBeforeDrag);
            image.raycastTarget = true;
        }
    }
}
