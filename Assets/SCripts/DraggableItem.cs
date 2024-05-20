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
    
    private LayerMask tileLayerMask;
    private LayerMask pieceLayerMask;
    
    //public bool isInitiallyMovable = true;
    
    private void Start()
    {
        // Set interactable based on the tag
        //isInteractable = !CompareTag("Player1PieceNotMovable") && !CompareTag("Player2PieceNotMovable");
        tileLayerMask = LayerMask.GetMask("Tile");
        pieceLayerMask = LayerMask.GetMask("Piece");
        
        isInteractable = !CompareTag("Player1PieceNotMovable") &&
                         !CompareTag("Player2PieceNotMovable") &&
                         !CompareTag("Player1Token") &&
                         !CompareTag("Player2Token");
        SetInteractable(isInteractable);
    }

    public void SetInteractable(bool interactable)
    {
        if (!hasMoved || GameManager.instance.initialPlacementComplete)
        {
            isInteractable = interactable;
            image.raycastTarget = interactable;   
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        //if (!isInteractable || hasMoved) return;
        if (!isInteractable) return;

        if (!GameManager.instance.initialPlacementComplete)
        {
            if (hasMoved) return;
        }
        
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
        //if (!isInteractable || hasMoved) return;
        if (!isInteractable) return;
        
        if (!GameManager.instance.initialPlacementComplete)
        {
            if (hasMoved) return;
        }
        
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if (!isInteractable || hasMoved) return;
        //if (!isInteractable || hasMoved || !isDragging) return;
        if (!isInteractable) return;
        
        if (!GameManager.instance.initialPlacementComplete)
        {
            if (hasMoved) return;
        }
        
        //isDragging = false;
        
        if (parentAfterDrag != null && parentAfterDrag.childCount == 1 && parentBeforeDrag != parentAfterDrag && !GameManager.instance.initialPlacementComplete)
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
        else if (parentAfterDrag != null && GameManager.instance.initialPlacementComplete && parentAfterDrag.childCount is 1 or 2 && parentBeforeDrag != parentAfterDrag)
        {
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
            //hasMoved = true;
            //SetInteractable(false);

            if (IsValidMove())
            {
                isDragging = false;
                HandleMove();
            }
            else
            {
                RevertMove();
            }
        }
        else
        {
            // Invalid drop, revert to the original parent
            transform.SetParent(parentBeforeDrag);
            image.raycastTarget = true;
        }
    }
    
    private bool IsValidMove()
    {
        Vector3 direction = parentAfterDrag.position - parentBeforeDrag.position;
        float distance = direction.magnitude;

        if (CompareTag("Player1Token") || CompareTag("Player2Token"))
        {
            return IsValidTokenMove(direction, distance);
        }
        else
        {
            return IsValidNormalMove(direction, distance);
        }
    }

    private bool IsValidNormalMove(Vector3 direction, float distance)
    {
        //if (distance > 1.1f) return false;
        Debug.Log("Check Validity");
        //if (distance > 45.1f) return false;
        if (distance > 150.1f) return false;
        Debug.Log("Is valid");

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, tileLayerMask);
        return hit.collider != null && hit.collider.transform == parentAfterDrag;
    }

    private bool IsValidTokenMove(Vector3 direction, float distance)
    {
        //if (distance > 3.1f) return false; // Allowing up to 3 tiles move
        Debug.Log("Check Validity");
        if (distance > 135.1f) return false;
        Debug.Log("Is valid");

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, tileLayerMask);
        return hit.collider != null && hit.collider.transform == parentAfterDrag;
    }

    private void HandleMove()
    {
        Transform occupyingPiece = GetPieceAtPosition(parentAfterDrag.position);

        if (occupyingPiece != null)
        {
            Vector3 direction = parentAfterDrag.position - parentBeforeDrag.position;
            Transform nextTile = GetNextTile(occupyingPiece.position, direction);

            if (nextTile != null && GetPieceAtPosition(nextTile.position) == null)
            {
                occupyingPiece.SetParent(nextTile);
                occupyingPiece.position = nextTile.position;
            }
            else
            {
                RevertMove();
                return;
            }
        }

        // Move the current piece to the new tile
        transform.SetParent(parentAfterDrag);
        transform.position = parentAfterDrag.position;
        image.raycastTarget = true;
        //hasMoved = true;
        //SetInteractable(false);

        if (GameManager.instance != null)
        {
            GameManager.instance.SwitchTurn();
        }
        else
        {
            Debug.LogError("GameManager instance is null");
        }
    }

    private void RevertMove()
    {
        transform.SetParent(parentBeforeDrag);
        transform.position = parentBeforeDrag.position;
        image.raycastTarget = true;
    }

    private Transform GetTileUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, tileLayerMask);

        if (hit.collider != null)
        {
            return hit.collider.transform;
        }

        return null;
    }

    private Transform GetPieceAtPosition(Vector3 position)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position, pieceLayerMask);

        foreach (var hit in hits)
        {
            if (hit.transform != transform)
            {
                return hit.transform;
            }
        }

        return null;
    }

    private Transform GetNextTile(Vector3 currentPos, Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(currentPos, direction, 1.1f, tileLayerMask);
        return hit.collider != null ? hit.collider.transform : null;
    }

    private void Update()
    {
        if (isDragging)
        {
            parentAfterDrag = GetTileUnderMouse();
        }
    }
}
