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
    public bool hasMoved { get; private set; } = false;
    //private bool isDragging = false;
    
    private PieceMovement pieceMovement;
    
    public GameObject ErrEndTurnPanel;
    public GameObject ErrStoneMovePanel;
    public GameObject ErrTokenMovePanel;
    public GameObject ErrTokenPushPanel;
    
    
    private void Start()
    {
        pieceMovement = GetComponent<PieceMovement>();
        
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
        
        GameManager.instance.DisableErrPanels();
        
        //isDragging = true;
        parentBeforeDrag = transform.parent;
        parentAfterDrag = null;
        //parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        if (!GameManager.instance.initialPlacementComplete)
        {
            if (hasMoved) return;
        }
        
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
            
            bool validMove = pieceMovement.HandleMovement(transform, parentBeforeDrag, parentAfterDrag);
            
            /*if (validMove && GameManager.instance != null)
            {
                GameManager.instance.SwitchTurn();
            }
            else if (GameManager.instance == null)
            {
                Debug.LogError("GameManager instance is null");
            }*/
        }
        else
        {
            // Invalid drop, revert to the original parent
            transform.SetParent(parentBeforeDrag);
            image.raycastTarget = true;
        }
        image.raycastTarget = true;
    }
    
    /*private void IsValidMove()
    {
        Vector3 direction = parentAfterDrag.position - parentBeforeDrag.position;
        float distance = direction.magnitude;

        if (CompareTag("Player1Token") || CompareTag("Player2Token"))
        {
            //return IsValidTokenMove(direction, distance);
            IsValidTokenMove(direction, distance);
        }
        else
        {
            //return IsValidNormalMove(direction, distance);
            IsValidNormalMove(direction, distance);
        }
    }

    private void IsValidNormalMove(Vector3 direction, float distance)
    {
        //if (distance > 1.1f) return false;
        Debug.Log("Check Validity");
        //if (distance > 45.1f) return false;
        //if (distance > 40.1f) return false;
        if (distance > 200.1f)
        {
            Debug.Log("Stone move too far");
            RevertMove();
            return;
        }
        //Debug.Log("Is valid");

        Debug.Log("Raycasting for normal move...");
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, tileLayerMask);
        RaycastHit2D hit = Physics2D.Raycast(parentBeforeDrag.position, direction, distance, tileLayerMask);
        Debug.Log($"Raycast result: {hit.collider?.name}");
        
        if (hit.collider != null && hit.collider.transform == parentAfterDrag) //&& occupyingPiece == null)
        {
            Debug.Log("Raycast hit the target tile.");
            Transform occupyingPiece = GetPieceAtPosition(parentAfterDrag.position);
            if (occupyingPiece == null)
            {
                Debug.Log("Normal Stone Movement");
                HandleStoneMove();
            }
            else
            {
                Debug.Log("Drag Movement");
                HandlePieceDrag(occupyingPiece, direction);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit the target tile or hit a different tile.");
            RevertMove();
        }
    }

    private void IsValidTokenMove(Vector3 direction, float distance)
    {
        //if (distance > 3.1f) return false; // Allowing up to 3 tiles move
        Debug.Log("Check Validity");
        //if (distance > 135.1f) return false;
        if (distance > 600.1f)
        {
            Debug.Log("Token move too far");
            RevertMove();
            return;
        }
        
        if (Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0)
        {
            Debug.Log("Token can only move orthogonally.");
            RevertMove();
            return;
        }
        
        //Debug.Log("Is valid");

        //RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, tileLayerMask);
        RaycastHit2D hit = Physics2D.Raycast(parentBeforeDrag.position, direction, distance, tileLayerMask);
        
        //Transform occupyingPiece = GetPieceAtPosition(parentAfterDrag.position);
        //return hit.collider != null && hit.collider.transform == parentAfterDrag;
        if (hit.collider != null && hit.collider.transform == parentAfterDrag)// && occupyingPiece == null)
        {
            //HandleTokenMove();
            Transform occupyingPiece = GetPieceAtPosition(parentAfterDrag.position);
            if (occupyingPiece == null)
            {
                HandleTokenMove();
            }
            else
            {
                RevertMove();
            }
        }
        else
        {
            //HandlePieceDrag(occupyingPiece);
            RevertMove();
        }
    }
    
    private void HandleStoneMove()
    {
        //parentAfterDrag = transform;
        transform.SetParent(parentAfterDrag);
        //transform.SetParent(GetTileUnderMouse());
        //transform.position = parentAfterDrag.position;
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
    
    private void HandleTokenMove()
    {
        transform.SetParent(parentAfterDrag);
        //transform.position = parentAfterDrag.position;
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

    private void HandlePieceDrag(Transform occupyingPiece, Vector3 direction)
    {
        //Transform occupyingPiece = GetPieceAtPosition(parentAfterDrag.position);

        /*if (occupyingPiece != null)
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
        
        Transform nextTile = GetNextTile(occupyingPiece.position, direction);

        if (nextTile != null && GetPieceAtPosition(nextTile.position) == null)
        {
            occupyingPiece.SetParent(nextTile);
            //occupyingPiece.position = nextTile.position;

            transform.SetParent(parentAfterDrag);
            //transform.position = parentAfterDrag.position;
            image.raycastTarget = true;

            if (GameManager.instance != null)
            {
                GameManager.instance.SwitchTurn();
            }
            else
            {
                Debug.LogError("GameManager instance is null");
            }
        }
        else
        {
            RevertMove();
        }
    }

    private void RevertMove()
    {
        transform.SetParent(parentBeforeDrag);
        //transform.position = parentBeforeDrag.position;
        image.raycastTarget = true;
    }

    private Transform GetTileUnderMouse()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, tileLayerMask);
        
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, tileLayerMask);

        if (hit.collider != null)
        {
            return hit.collider.transform;
        }

        return null;
    }

    private Transform GetPieceAtPosition(Vector3 position)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position, pieceLayerMask);
        //var size = Physics2D.OverlapPointNonAlloc(position, hits, pieceLayerMask);

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
        RaycastHit2D hit = Physics2D.Raycast(currentPos, direction, 200.1f, tileLayerMask);
        return hit.collider != null ? hit.collider.transform : null;
    }

    private void Update()
    {
        if (isDragging)
        {
            parentAfterDrag = GetTileUnderMouse();
        }
    }
    
    private Transform GetTileUnderMouse()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPoint.z = 0;  // Ensure the z-coordinate is zero for 2D

        var hits = Physics2D.OverlapPointAll(worldPoint);

        foreach (var hit in hits)
        {
            if (hit.transform != null && hit.transform.CompareTag("Tile"))
            {
                return hit.transform;
            }
        }

        return null;
    }

    private void Update()
    {
        if (isDragging)
        {
            parentAfterDrag = GetTileUnderMouse();
        }
    }*/
}
