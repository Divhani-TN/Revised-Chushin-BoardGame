using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject Player1Panel;
    public GameObject Player2Panel;
    
    public GameObject endSetupPanel;
    public GameObject SetupPanel;
    public GameObject MovementPanel;

    private PanelManager panelManager;
    /*public GameObject ErrEndTurnPanel;
    public GameObject ErrStoneMovePanel;
    public GameObject ErrTokenMovePanel;
    public GameObject ErrTokenPushPanel;
    public GameObject ErrTokenMoverOverPanel;
    public GameObject ErrCanPush;
    public GameObject overlay;
    private GameObject activePanel;*/

    public enum Player { Player1, Player2 }
    public Player currentPlayer;

    private DraggableItem draggableItem;
    public List<DraggableItem> player1Pieces;
    public List<DraggableItem> player2Pieces;
    private int player1Count = 0;
    private int player2Count = 0;
    public bool tokenMove;
    
    private int player1MovedCount = 0;
    private int player2MovedCount = 0;
    
    public bool initialPlacementComplete = false;
    private bool pieceMoved = false;
    
    public Button btnEndTurn;
    
    public GameObject Player1WinPanel;
    public GameObject Player2WinPanel;
    public Button btnEndGame;
    public GameObject Pause;
    
    private int turnCounter = 0;
    private Dictionary<DraggableItem, int> pushedPieces = new Dictionary<DraggableItem, int>();
    
    public GameObject StoneRemovalPanel;

    public GameObject Inventory1;
    public GameObject Inventory2;
    
    private FourInARow fourInARow;

    //public TileNew[] boardTile;
    public List<TileNew> boardTile = new List<TileNew>();
    public TileNew selectedTile;
    public Button btnRemoveStone;
    private bool removePiece = false;
    public bool hasSwitchedTurn = false;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        boardTile.AddRange(FindObjectsOfType<TileNew>());
    }

    private void Start()
    {
        panelManager = PanelManager.instance;
        draggableItem = DraggableItem.instance;
        fourInARow = FourInARow.instance;
        
        currentPlayer = Player.Player1;
        Player1Panel.gameObject.SetActive(true);
        Player2Panel.gameObject.SetActive(false);
        InitializePieces();
        UpdatePieceInteractivity();
        Pause.SetActive(false);
        
        StoneRemovalPanel.SetActive(false);
    }
    
    public void RegisterPieceMove()
    {
        pieceMoved = true;

        foreach (var piece in player1Pieces)
        {
            piece.SetInteractable(currentPlayer == Player.Player1 && !piece.CompareTag("Player1PieceNotMovable") && !piece.CompareTag("Player1Piece"));
        }
        foreach (var piece in player2Pieces)
        {
            piece.SetInteractable(currentPlayer == Player.Player2 && !piece.CompareTag("Player2PieceNotMovable") && !piece.CompareTag("Player2Piece"));
        }
 
    }
    
    
    public void OnEndTurnButtonClicked()
    {
        if (pieceMoved == true)
        {
            SwitchTurn();
        }
        else
        {
            panelManager.EnableErr(4);
            //ErrEndTurnPanel.gameObject.SetActive(true);
        }
    }
    
    private void InitializePieces()
    {
        /*if (!initialPlacementComplete) //&& !fourInARow.forcedStoneRemoval)
        {
            // Find and filter pieces by their tag using LINQ
            player1Pieces = FindObjectsOfType<DraggableItem>().Where(p => p.CompareTag("Player1Piece")).ToList();
            player2Pieces = FindObjectsOfType<DraggableItem>().Where(p => p.CompareTag("Player2Piece")).ToList();
        }
        else if (initialPlacementComplete)//&& !fourInARow.forcedStoneRemoval)*/
        {
            player1Pieces = FindObjectsOfType<DraggableItem>().Where(p => p.CompareTag("Player1Piece") || p.CompareTag("Player1PieceNotMovable") || p.CompareTag("Player1Token")).ToList();
            player1Count = player1Pieces.Count - 3;
            player2Pieces = FindObjectsOfType<DraggableItem>().Where(p => p.CompareTag("Player2Piece") || p.CompareTag("Player2PieceNotMovable") || p.CompareTag("Player2Token")).ToList();
            player2Count = player2Pieces.Count - 3;
        }
        /*else if (fourInARow.forcedStoneRemoval)
        {
            player1Pieces = FindObjectsOfType<DraggableItem>().Where(p => p.CompareTag("Player1Piece") || p.CompareTag("Player1PieceNotMovable")).ToList();
            player2Pieces = FindObjectsOfType<DraggableItem>().Where(p => p.CompareTag("Player2Piece") || p.CompareTag("Player2PieceNotMovable")).ToList();
        }*/
    }
    
    public void PieceMoved()
    {
        if (currentPlayer == Player.Player1)
        {
            player1MovedCount++;
        }
        else
        {
            player2MovedCount++;
        }

        // Check if all Stones have been placed on the board
        if (player1MovedCount >= player1Count && player2MovedCount >= player2Count && !initialPlacementComplete && !fourInARow.forcedStoneRemoval)
        {
            initialPlacementComplete = true;
            Player1Panel.SetActive(false);
            Player2Panel.SetActive(false);
            endSetupPanel.SetActive(true);
            UpdatePieceInteractivity();
        }
        
        //FourInARow.instance.CheckForRow();
        if (!fourInARow.forcedStoneRemoval)
            SwitchTurn();
        
    }

    
    public void EnableRemoveStoneUI()
    {
        StoneRemovalPanel.SetActive(true);
        btnRemoveStone.GameObject().SetActive(true);
        foreach (var piece in player1Pieces)
        {
            piece.SetInteractable(false);
        }

        foreach (var piece in player2Pieces)
        {
            piece.SetInteractable(false);
        }
    }

    public void AssignSelectedTile(TileNew tile)
    {
        selectedTile = tile;
    }

    public void DeselectAllTiles()
    {
        foreach (TileNew tile in boardTile)
        {
            if (tile != null)
            {
                /*if (tile.selectedShader != null)
                {
                    tile.selectedShader.SetActive(false);
                }*/
                tile.Deselect();
                //tile.thisTileSelected = false;
            }
        }

        selectedTile = null;
    }

    public void RemoveStone()
    {
        if (selectedTile != null)
        {
            if (selectedTile.transform.childCount == 2)
            {
                bool hasPlayerToken = false;
                
                foreach (Transform child in selectedTile.transform)
                {
                    if (child.CompareTag("Player1Token") || child.CompareTag("Player2Token"))
                    {
                        hasPlayerToken = true;
                        break;
                    }
                }
                
                if (!hasPlayerToken)
                {
                    Transform piece = selectedTile.transform.GetChild(1);
                    ReparentPiece(piece);
                    removePiece = true;
                    DeselectAllTiles();
                    DraggableItem pieceToRemove = piece.GetComponent<DraggableItem>();
                    //InitializePieces();
                    if (player1Pieces.Contains(pieceToRemove))
                    {
                        player1Pieces.Remove(pieceToRemove);
                    }
                    else if (player2Pieces.Contains(pieceToRemove))
                    {
                        player2Pieces.Remove(pieceToRemove);
                    }
                    pieceToRemove.SetInteractable(false);
                    UpdatePieceInteractivity();
                    removePiece = false;
                    //RegisterPieceMove();
                }
                else
                {
                    panelManager.EnableErr(6);
                    Debug.Log("You cannot remove a Token");
                }
            }
            else
            {
                panelManager.EnableErr(7);
                Debug.Log("Please select a tile with a Stone in it");
            }
        }
        else
        {
            panelManager.EnableErr(8);
            Debug.Log("No tile selected");
        }
    }
    
    void ReparentPiece(Transform piece)
    {
        if (currentPlayer == Player.Player1)
        {
            piece.SetParent(Inventory1.transform);
        }
        else
        {
            piece.SetParent(Inventory2.transform);
        }

        hasSwitchedTurn = false;
        piece.tag = "InventoryPiece";
        btnRemoveStone.gameObject.SetActive(false);
        fourInARow.forcedStoneRemoval = false;
        
        if (initialPlacementComplete)
        {
            RegisterPieceMove();
            if (tokenMove)
            {
                SwitchTurn();
                tokenMove = false;
                hasSwitchedTurn = true;
            }
            else
            {
                //RegisterPieceMove();
            }
            btnEndTurn.gameObject.SetActive(true);
        }
        else
        {
            PieceMoved();
        }
        //InitializePieces();
        
        //piece.tag = "InventoryPiece";

        //btnRemoveStone.gameObject.SetActive(false);
        //btnEndTurn.gameObject.SetActive(true);
    }
    

    public void SwitchTurn()
    {
        currentPlayer = (currentPlayer == Player.Player1) ? Player.Player2 : Player.Player1;
        if (currentPlayer == Player.Player1)
        {
            Player1Panel.gameObject.SetActive(true);
            Player2Panel.gameObject.SetActive(false);
        }
        else if (currentPlayer == Player.Player2)
        {
            Player1Panel.gameObject.SetActive(false);
            Player2Panel.gameObject.SetActive(true);
        }
        pieceMoved = false;

        if (initialPlacementComplete)
        {
            turnCounter++;

            // Reset the last pushed from tile tracking for pieces pushed two turns ago
            List<DraggableItem> piecesToReset = new List<DraggableItem>();
            foreach (var entry in pushedPieces)
            {
                if (turnCounter - entry.Value >= 2)
                {
                    piecesToReset.Add(entry.Key);
                }
            }

            foreach (var piece in piecesToReset)
            {
                piece.lastPushedFromTile = null;
                pushedPieces.Remove(piece);
            }

            //fourInARow.CheckForForcedStoneRemoval();
        }
            //else if (hasSwitchedTurn)
                hasSwitchedTurn = false;
        
            UpdatePieceInteractivity();
            fourInARow.CheckForForcedStoneRemoval();
    }
    
    
    public void RegisterPushedPiece(DraggableItem piece)
    {
        if (!pushedPieces.ContainsKey(piece))
        {
            pushedPieces.Add(piece, turnCounter);
        }
    }
    
    public void OnCloseStoneRemovalPanel()
    {
        StoneRemovalPanel.SetActive(false); // Hide the panel
        //UpdatePieceInteractivity(); // Allow the player to move a stone to inventory
    }

    public void UpdatePieceInteractivity()
    {
        if (!initialPlacementComplete) //&& !fourInARow.forcedStoneRemoval)
        {
            //InitializePieces();
            foreach (var piece in player1Pieces)
            {
                piece.SetInteractable(currentPlayer == Player.Player1 && !piece.CompareTag("Player1PieceNotMovable") && !piece.CompareTag("Player1Token"));
            }

            foreach (var piece in player2Pieces)
            {
                piece.SetInteractable(currentPlayer == Player.Player2 && !piece.CompareTag("Player2PieceNotMovable") && !piece.CompareTag("Player2Token"));
            }
        }
        else if (initialPlacementComplete && !removePiece) //&& !fourInARow.forcedStoneRemoval)
        {
            //InitializePieces();
            foreach (var piece in player1Pieces)
            {
                piece.SetInteractable(currentPlayer == Player.Player1);
            }

            foreach (var piece in player2Pieces)
            {
                piece.SetInteractable(currentPlayer == Player.Player2);
            }
        }
        else if (removePiece)
        {
            foreach (var piece in player1Pieces)
            {
                piece.SetInteractable(currentPlayer == Player.Player1 && !piece.CompareTag("Player1PieceNotMovable") && !piece.CompareTag("Player1Piece"));
            }

            foreach (var piece in player2Pieces)
            {
                piece.SetInteractable(currentPlayer == Player.Player2 && !piece.CompareTag("Player2PieceNotMovable") && !piece.CompareTag("Player2Piece"));
            }
        }
        /*else if (fourInARow.forcedStoneRemoval)
        {
            InitializePieces();
            foreach (var piece in player1Pieces)
            {
                piece.SetInteractable(currentPlayer == Player.Player1 && !piece.CompareTag("Player1Token"));
            }

            foreach (var piece in player2Pieces)
            {
                piece.SetInteractable(currentPlayer == Player.Player2 && !piece.CompareTag("Player2Token"));
            }
        }*/
    }

    public void CloseSetupEndPanel()
    {
        endSetupPanel.SetActive(false);
        UpdatePieceInteractivity();
        
        btnEndTurn.gameObject.SetActive(true);
        
        Player1Panel.gameObject.SetActive(false);
        Player2Panel.gameObject.SetActive(true);
        SetupPanel.SetActive(false);
        MovementPanel.SetActive(true);
        SwitchTurn();
    }
    
    public void Player1Wins()
    {
        Player1WinPanel.SetActive(true);
        DisableGameplayUI();
        btnEndGame.gameObject.SetActive(true);
    }

    public void Player2Wins()
    {
        Player2WinPanel.SetActive(true);
        DisableGameplayUI();
        btnEndGame.gameObject.SetActive(true);
    }

    private void DisableGameplayUI()
    {
        foreach (var piece in player1Pieces)
        {
            piece.SetInteractable(false);
        }

        foreach (var piece in player2Pieces)
        {
            piece.SetInteractable(false);
        }
        
        btnEndTurn.gameObject.SetActive(false);
    }
}
