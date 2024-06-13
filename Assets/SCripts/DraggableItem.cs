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
        
        //isDragging = false;

        /*if (parentAfterDrag == null && parentAfterDrag != parentBeforeDrag && fourInARow.forcedStoneRemoval)
        {
            //if ((!CompareTag("Player1Token") || !CompareTag("Player2Token")) && (parentAfterDrag.name is "Background" or "Inventory1" or "Inventory2"))
            if ((!CompareTag("Player1Token") || !CompareTag("Player2Token")) )//&& (parentAfterDrag.CompareTag("Background") || parentAfterDrag.CompareTag("Inventory1") || parentAfterDrag.CompareTag("Inventory2")))
            {
                Transform inventory = GameManager.instance.currentPlayer == GameManager.Player.Player1 ? GameObject.Find("Inventory1").transform : GameObject.Find("Inventory2").transform;
                transform.SetParent(inventory);
                SetInteractable(false); // Make the piece non-interactable
                gameObject.tag = "InventoryPiece"; // Update tag to indicate it is in the inventory

                fourInARow.forcedStoneRemoval = false; // Reset the flag
                GameManager.instance.StoneRemovalPanel.SetActive(false); // Hide the stone removal panel
                GameManager.instance.UpdatePieceInteractivity();
                GameManager.instance.btnEndGame.gameObject.SetActive(true);
            }
        }
        else
        {
            transform.SetParent(parentBeforeDrag);
            image.raycastTarget = true;
        }*/

        if (!fourInARow.forcedStoneRemoval)
        {
            if (parentAfterDrag != null && parentAfterDrag.childCount == 1 && parentBeforeDrag != parentAfterDrag &&
                !GameManager.instance.initialPlacementComplete && parentAfterDrag.name != "D4")
            {
                // Valid drop, set parent to the new parent
                transform.SetParent(parentAfterDrag);
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
