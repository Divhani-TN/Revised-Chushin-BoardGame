using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static DraggableItem instance; 
    public Image image;
    [HideInInspector] public Transform parentBeforeDrag;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public Transform lastPushedFromTile;
    
    private bool isInteractable = true;
    public bool hasMoved { get; private set; } = false;
    //private bool isDragging = false;
    
    private PieceMovement pieceMovement;
    private FourInARow fourInARow;
    public bool tokenMove;

    public SoundManager soundManager;
    
    //public GameObject ErrEndTurnPanel;
    //public GameObject ErrStoneMovePanel;
    //public GameObject ErrTokenMovePanel;
    //public GameObject ErrTokenPushPanel;
    
    /*private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }*/
    
    private void Start()
    {
        pieceMovement = GetComponent<PieceMovement>();
        //fourInARow = GetComponent<FourInARow>();
        fourInARow = FourInARow.instance;
        
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
        if (!isInteractable || fourInARow.forcedStoneRemoval) return;
        //if (!isInteractable || (fourInARow.forcedStoneRemoval && 
          //                      (CompareTag("Player1Token") && CompareTag("Player2Token")))) return;

        if (!GameManager.instance.initialPlacementComplete)
        {
            if (hasMoved) return;
        }
        
        //GameManager.instance.DisableErrPanels();
        
        //isDragging = true;
        
        //SoundManager.Instance.PlaySFX("Pickup Marble");
        soundManager.PlaySFX("Pickup Marble");
        parentBeforeDrag = transform.parent;
        parentAfterDrag = null;
        //parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInteractable || fourInARow.forcedStoneRemoval) return;
        
        if (!GameManager.instance.initialPlacementComplete)
        {
            if (hasMoved) return;
        }
        
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isInteractable || fourInARow.forcedStoneRemoval) return;
        
        if (!GameManager.instance.initialPlacementComplete)
        {
            if (hasMoved) return;
        }
        if (!fourInARow.forcedStoneRemoval)
        {
            if (parentAfterDrag != null && parentAfterDrag.childCount == 1 && parentBeforeDrag != parentAfterDrag &&
                !GameManager.instance.initialPlacementComplete && parentAfterDrag.name != "D4")
            {
                // Valid drop, set parent to the new parent
                transform.SetParent(parentAfterDrag);
                //SoundManager.Instance.PlaySFX("Drop Marble");
                soundManager.PlaySFX("Drop Marble");
                image.raycastTarget = true;
                hasMoved = true;
                SetInteractable(false);
                fourInARow.CheckForRow(transform, parentAfterDrag);
                /*if (fourInARow != null)
                {
                    //fourInARow.CheckForRow(transform, parentAfterDrag);
                    Debug.Log("Four in a row is not null");
                }
                else
                    Debug.Log("Four in a row is null");
                //fourInARow.CheckForRow(transform, parentAfterDrag);*/

                if (GameManager.instance != null)
                {
                    GameManager.instance.PieceMoved();
                }
                else
                {
                    Debug.LogError("GameManager instance is null");
                }
            }
            else if (parentAfterDrag != null && GameManager.instance.initialPlacementComplete &&
                     parentAfterDrag.childCount <= 2 && parentBeforeDrag != parentAfterDrag)
            {
                tokenMove = pieceMovement.HandleMovement(transform, parentBeforeDrag, parentAfterDrag);
                GameManager.instance.tokenMove = tokenMove;
                //fourInARow.CheckForRow(transform, parentAfterDrag);
                if (tokenMove && !fourInARow.forcedStoneRemoval)
                    GameManager.instance.SwitchTurn();
            }
            else
            {
                // Invalid drop, revert to the original parent
                transform.SetParent(parentBeforeDrag);
                image.raycastTarget = true;
            }
        }
        else
        {
            transform.SetParent(parentBeforeDrag);
            image.raycastTarget = true;
        }

        image.raycastTarget = true;
    }
}
