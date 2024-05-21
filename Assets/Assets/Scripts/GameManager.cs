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
    
    public GameObject ErrEndTurnPanel;
    public GameObject ErrStoneMovePanel;
    public GameObject ErrTokenMovePanel;
    public GameObject ErrTokenPushPanel;
    public GameObject ErrTokenMoverOverPanel;

    public enum Player { Player1, Player2 }
    public Player currentPlayer;

    public List<DraggableItem> player1Pieces;
    public List<DraggableItem> player2Pieces;
    
    private int player1MovedCount = 0;
    private int player2MovedCount = 0;
    
    public bool initialPlacementComplete = false;
    private bool pieceMoved = false;
    
    public Button btnEndTurn;
    
    public GameObject Player1WinPanel;
    public GameObject Player2WinPanel;
    public Button btnEndGame;

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
    }

    private void Start()
    {
        currentPlayer = Player.Player1;
        Player1Panel.gameObject.SetActive(true);
        Player2Panel.gameObject.SetActive(false);
        InitializePieces();
        UpdatePieceInteractivity();
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

    public void DisableErrPanels()
    {
        ErrEndTurnPanel.gameObject.SetActive(false);
        ErrStoneMovePanel.gameObject.SetActive(false);
        ErrTokenMovePanel.gameObject.SetActive(false);
        ErrTokenPushPanel.gameObject.SetActive(false);
        ErrTokenMoverOverPanel.gameObject.SetActive(false);
    }

    public void EnableStoneErr()
    {
        ErrStoneMovePanel.gameObject.SetActive(true);
        ErrTokenMovePanel.gameObject.SetActive(false);
        ErrTokenPushPanel.gameObject.SetActive(false);
        ErrEndTurnPanel.gameObject.SetActive(false);
        ErrTokenMoverOverPanel.gameObject.SetActive(false);
    }

    public void EnableTokenErr()
    {
        ErrTokenMovePanel.gameObject.SetActive(true);
        ErrStoneMovePanel.gameObject.SetActive(false);
        ErrTokenPushPanel.gameObject.SetActive(false);
        ErrEndTurnPanel.gameObject.SetActive(false);
        ErrTokenMoverOverPanel.gameObject.SetActive(false);
    }

    public void EnableTokenPushErr()
    {
        ErrTokenPushPanel.gameObject.SetActive(true);
        ErrStoneMovePanel.gameObject.SetActive(false);
        ErrTokenMovePanel.gameObject.SetActive(false);
        ErrEndTurnPanel.gameObject.SetActive(false);
        ErrTokenMoverOverPanel.gameObject.SetActive(false);
    }

    public void EnableTokenMoverOverErr()
    {
        ErrStoneMovePanel.gameObject.SetActive(true);
        ErrTokenMovePanel.gameObject.SetActive(false);
        ErrTokenPushPanel.gameObject.SetActive(false);
        ErrEndTurnPanel.gameObject.SetActive(false);
        ErrTokenMoverOverPanel.gameObject.SetActive(true);
    }
    
    public void OnEndTurnButtonClicked()
    {
        if (pieceMoved == true)
        {
            SwitchTurn();
        }
        else
        {
            ErrEndTurnPanel.gameObject.SetActive(true);
        }
    }
    
    private void InitializePieces()
    {
        if (!initialPlacementComplete)
        {
            // Find and filter pieces by their tag using LINQ
            player1Pieces = FindObjectsOfType<DraggableItem>().Where(p => p.CompareTag("Player1Piece")).ToList();
            player2Pieces = FindObjectsOfType<DraggableItem>().Where(p => p.CompareTag("Player2Piece")).ToList();
        }
        else if (initialPlacementComplete)
        {
            player1Pieces = FindObjectsOfType<DraggableItem>().Where(p => p.CompareTag("Player1Piece") || p.CompareTag("Player1PieceNotMovable") || p.CompareTag("Player1Token")).ToList();
            player2Pieces = FindObjectsOfType<DraggableItem>().Where(p => p.CompareTag("Player2Piece") || p.CompareTag("Player2PieceNotMovable") || p.CompareTag("Player2Token")).ToList();
        }
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
        if (player1MovedCount == player1Pieces.Count && player2MovedCount == player2Pieces.Count && !initialPlacementComplete)
        {
            initialPlacementComplete = true;
            Player1Panel.SetActive(false);
            Player2Panel.SetActive(false);
            endSetupPanel.SetActive(true);
            UpdatePieceInteractivity();
        }
        
        SwitchTurn();
        
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
        
        UpdatePieceInteractivity();
    }

    private void UpdatePieceInteractivity()
    {
        if (!initialPlacementComplete)
        {
            foreach (var piece in player1Pieces)
            {
                piece.SetInteractable(currentPlayer == Player.Player1 && !piece.CompareTag("Player1PieceNotMovable") && !piece.CompareTag("Player1Token"));
            }

            foreach (var piece in player2Pieces)
            {
                piece.SetInteractable(currentPlayer == Player.Player2 && !piece.CompareTag("Player2PieceNotMovable") && !piece.CompareTag("Player2Token"));
            }
        }
        else if (initialPlacementComplete)
        {
            InitializePieces();
            foreach (var piece in player1Pieces)
            {
                piece.SetInteractable(currentPlayer == Player.Player1);
            }

            foreach (var piece in player2Pieces)
            {
                piece.SetInteractable(currentPlayer == Player.Player2);
            }
        }
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
