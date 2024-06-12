using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FourInARow : MonoBehaviour
{
    public static FourInARow instance;
    private int player1RemovedCount = 0; // Track the number of stones Player 1 has removed
    private int player2RemovedCount = 0; // Track the number of stones Player 2 has removed

    private HashSet<string> usedRows = new HashSet<string>();
    
    public bool forcedStoneRemoval = false;
    
    public bool newStoneRemoval = false;
    private Transform newPiece;
    public Transform newTile;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("FourInARow instance initialized");
    }
    
    public void CheckForRow(Transform piece, Transform parentAfterDrag)
    {
        int playerNum = 0;
        
        foreach (DraggableItem item in GameManager.instance.player1Pieces)
        {
            if (item.transform == piece)
            {
                playerNum = 1;
            }
        }
        foreach (DraggableItem item in GameManager.instance.player2Pieces)
        {
            if (item.transform == piece)
            {
                playerNum = 2;
            }
        }
        
        if (playerNum == 1)
        {
            if (GameManager.instance.currentPlayer != GameManager.Player.Player1)
            {
                newPiece = piece;
                newTile = parentAfterDrag;
                newStoneRemoval = true;
                return;
            }
            
            if (DetectedRow(piece, GameManager.instance.player1Pieces, parentAfterDrag))
            {
                GameManager.instance.btnEndTurn.gameObject.SetActive(false);
                HandleRowFormed(GameManager.Player.Player1);
            }
        }
        if (playerNum == 2)
        {
            if (GameManager.instance.currentPlayer != GameManager.Player.Player2)
            {
                newStoneRemoval = true;
                return;
            }
            
            if (DetectedRow(piece, GameManager.instance.player2Pieces, parentAfterDrag))
            {
                //GameManager.instance.btnEndTurn.gameObject.SetActive(false);
                HandleRowFormed(GameManager.Player.Player2);
            }
        }
    }
    
    private bool DetectedRow(Transform piece, List<DraggableItem> pieces, Transform parentAfterDrag)
    {
        char currentRow = parentAfterDrag.name[0];
        int currentCol = 1;

        int colCount = 0;
        
        for (int col = 1; col <= 7; col++)
        {
            string currentTileName = currentRow.ToString() + col.ToString();
            GameObject currentTileObj = GameObject.Find(currentTileName);

            if (currentTileObj != null)
            {
                Transform currentTileTransform = currentTileObj.transform;
                Transform occupyingPiece = PieceMovement.GetPieceAtPosition(currentTileTransform);

                if (occupyingPiece != null && pieces.Any(p => p.transform == occupyingPiece))
                {
                    colCount++;
                    if (colCount >= 4 && !usedRows.Contains($"Row{currentRow}{colCount}"))
                    {
                        usedRows.Add($"Row{currentRow}{colCount}");
                        //forcedStoneRemoval = true;
                        return true;
                    }
                }
                else
                {
                    colCount = 0;
                }
            }
        }

        
        return false;
    }
    
    private void HandleRowFormed(GameManager.Player player)
    {
        if (player == GameManager.Player.Player1 && player1RemovedCount < 3)
        {
            player1RemovedCount++;
            // Force Player 1 to remove a stone
            ForceStoneRemoval(GameManager.Player.Player1);
        }
        else if (player == GameManager.Player.Player2 && player2RemovedCount < 3)
        {
            player2RemovedCount++;
            // Force Player 2 to remove a stone
            ForceStoneRemoval(GameManager.Player.Player2);
        }
        else forcedStoneRemoval = false;
    }
    
    private void ForceStoneRemoval(GameManager.Player player)
    {
        forcedStoneRemoval = true;
        GameManager.instance.EnableRemoveStoneUI();
        //GameManager.instance.UpdatePieceInteractivity();
    }

    public void CheckForForcedStoneRemoval()
    {
        if (!newStoneRemoval) return;
        
        CheckForRow(newPiece, newTile);
        
        // Wait for the player to drag a piece off the board
        //GameManager.instance.StoneRemovalPanel.SetActive(true);
        //forcedStoneRemoval = false;
    }
}
