using UnityEngine;

public class PieceMovement : MonoBehaviour
{
    //private LayerMask pieceLayerMask;
    private GameManager gameManager;
    private FourInARow fourInARow;

    private bool canTokenMove;
    private bool canStoneMove;
    private bool canTokenBePushed;
    private bool canTokenMoveOverTiles;
    private bool canPush;

    private bool isRegister = false;

    private void Start()
    {
        //pieceLayerMask = LayerMask.GetMask("Piece");
        gameManager = GameManager.instance;
        fourInARow = FourInARow.instance;
    }

    private bool IsValidMove(Transform piece, Transform parentBeforeDrag, Transform parentAfterDrag)
    {
        string startTile = parentBeforeDrag.name;
        string endTile = parentAfterDrag.name;

        //Check if piece being moved is a Token or Stone and it's validity
        if (piece.CompareTag("Player1Token") || piece.CompareTag("Player2Token"))
        {
            return IsValidTokenMove(startTile, endTile);
        }
        else
        {
            return IsValidPieceMove(startTile, endTile);
        }
    }

    private bool IsValidPieceMove(string startTile, string endTile)
    {
        char startRow = startTile[0];
        int startCol = int.Parse(startTile.Substring(1));
        char endRow = endTile[0];
        int endCol = int.Parse(endTile.Substring(1));

        //Check if Stone is only moving 1 tile in any direction
        canStoneMove = Mathf.Abs(startRow - endRow) <= 1 && Mathf.Abs(startCol - endCol) <= 1;
        
        return canStoneMove;
    }

    private bool IsValidTokenMove(string startTile, string endTile)
    {
        char startRow = startTile[0];
        int startCol = int.Parse(startTile.Substring(1));
        char endRow = endTile[0];
        int endCol = int.Parse(endTile.Substring(1));

        //Check if Token is moving 3 or less tiles and if it's moving orthogonally
        bool validOrthogonalMove = (startRow == endRow && Mathf.Abs(startCol - endCol) <= 3) ||
                                   (startCol == endCol && Mathf.Abs(startRow - endRow) <= 3);

        canTokenMove = validOrthogonalMove;
        canTokenMoveOverTiles = !IsPathBlocked(startTile, endTile);
        if (!canTokenMove)
        {
            return false;
        }
        else
        {
            return canTokenMoveOverTiles;
        }
        
    }

    private bool IsPathBlocked(string startTile, string endTile)
    {
        // Get row and column for start and end tiles
        char startRow = startTile[0];
        int startCol = int.Parse(startTile.Substring(1));
        char endRow = endTile[0];
        int endCol = int.Parse(endTile.Substring(1));

        // Determine the direction of movement
        int rowDirection = endRow > startRow ? 1 : (endRow < startRow ? -1 : 0);
        int colDirection = endCol > startCol ? 1 : (endCol < startCol ? -1 : 0);

        // Current position to check, start at the first tile after the start tile
        char currentRow = (char)(startRow + rowDirection);
        int currentCol = startCol + colDirection;

        // Iterate over each tile along the path
        while (currentRow != endRow || currentCol != endCol)
        {
            // Get the current tile name
            string currentTileName = currentRow.ToString() + currentCol.ToString();

            // Find the tile GameObject
            GameObject currentTileObject = GameObject.Find(currentTileName);

            // If the tile exists, check if it has any pieces
            if (currentTileObject != null)
            {
                Transform currentTileTransform = currentTileObject.transform;
                Transform occupyingPiece = GetPieceAtPosition(currentTileTransform);

                // If there's an occupying piece, the path is blocked
                if (occupyingPiece != null)
                {
                    return true;
                }
            }

            // Move to the next tile in the path
            currentRow = (char)(currentRow + rowDirection);
            currentCol = currentCol + colDirection;
        }

        // If no tiles in the path are occupied, the path is not blocked
        return false;
    }

    public bool HandleMovement(Transform piece, Transform parentBeforeDrag, Transform parentAfterDrag)
    {
        DraggableItem draggableItem = piece.GetComponent<DraggableItem>();
        
        //Check for valid moves depending on the piece
        if (IsValidMove(piece, parentBeforeDrag, parentAfterDrag))
        {
            //Handle moving and pushing with Tokens
            if (piece.CompareTag("Player1Token") || piece.CompareTag("Player2Token"))
            {
                Transform occupyingPiece = GetPieceAtPosition(parentAfterDrag);
                if (occupyingPiece == null)
                {
                    //Handle normal Token move if the tile it's being moved to isn't occupied
                    HandleTokenMove(piece, parentAfterDrag);
                    draggableItem.lastPushedFromTile = null;
                    
                    //Check win condition after moving
                    if (parentAfterDrag.name == "D4")
                    {
                        if (piece.CompareTag("Player1Token"))
                        {
                            GameManager.instance.Player1Wins();
                        }
                        else if (piece.CompareTag("Player2Token"))
                        {
                            GameManager.instance.Player2Wins();
                        }
                    }
                    
                    //FourInARow.instance.CheckForRow(piece, parentAfterDrag);
                }
                else
                {
                    Vector3 direction = (parentAfterDrag.position - parentBeforeDrag.position).normalized;
                    Transform nextTile = GetNextTile(parentAfterDrag.name, direction);

                    //Check if there is a tile and that it's not a token
                    if (nextTile != null && GetPieceAtPosition(nextTile) == null && !occupyingPiece.CompareTag("Player1Token") && !occupyingPiece.CompareTag("Player2Token"))
                    {
                        DraggableItem occupyingDraggableItem = occupyingPiece.GetComponent<DraggableItem>();
                        if (occupyingDraggableItem != null && nextTile == occupyingDraggableItem.lastPushedFromTile)
                        {
                            RevertMove(piece, parentBeforeDrag);
                            return false;
                        }
                        
                        //Handle pushing a Stone with a Token
                        HandlePieceMove(occupyingPiece, nextTile);
                        HandleTokenMove(piece, parentAfterDrag);
                        
                        if (draggableItem != null) draggableItem.lastPushedFromTile = parentBeforeDrag;
                        else canPush = false;
                        
                        if (occupyingDraggableItem != null) occupyingDraggableItem.lastPushedFromTile = parentAfterDrag;
                        else canPush = false;
                        
                        gameManager.RegisterPushedPiece(draggableItem);
                        gameManager.RegisterPushedPiece(occupyingDraggableItem);
                        
                        //Check win condition after pushing
                        if (parentAfterDrag.name == "D4")
                        {
                            if (piece.CompareTag("Player1Token"))
                            {
                                GameManager.instance.Player1Wins();
                            }
                            else if (piece.CompareTag("Player2Token"))
                            {
                                GameManager.instance.Player2Wins();
                            }
                        }
                        
                        //FourInARow.instance.CheckForRow(piece, parentAfterDrag);
                    }
                    else
                    {
                        RevertMove(piece, parentBeforeDrag);
                        return false;
                    }
                }
            }
            
            //Handle moving and pushing with Stones
            else
            {
                Transform occupyingPiece = GetPieceAtPosition(parentAfterDrag);
                
                if (occupyingPiece == null)
                {
                    //Handle normal Stone moving if the space isn't occupied
                    HandlePieceMove(piece, parentAfterDrag);
                    draggableItem.lastPushedFromTile = null;
                    //FourInARow.instance.CheckForRow(piece, parentAfterDrag);
                }
                else
                {
                    Vector3 direction = (parentAfterDrag.position - parentBeforeDrag.position).normalized;
                    Transform nextTile = GetNextTile(parentAfterDrag.name, direction);

                    if (nextTile != null && GetPieceAtPosition(nextTile) == null)
                    {
                        DraggableItem occupyingDraggableItem = occupyingPiece.GetComponent<DraggableItem>();
                        if (occupyingDraggableItem != null && nextTile == occupyingDraggableItem.lastPushedFromTile)
                        {
                            RevertMove(piece, parentBeforeDrag);
                            return false;
                        }
                        
                        if (!occupyingPiece.CompareTag("Player1Token") && !occupyingPiece.CompareTag("Player2Token"))
                        {
                            //Handle pushing a Stone with a Stone
                            HandlePieceMove(occupyingPiece, nextTile);
                            HandlePieceMove(piece, parentAfterDrag);
                            
                            if (draggableItem != null) draggableItem.lastPushedFromTile = parentBeforeDrag;
                            else canPush = false;
                            
                            if (occupyingDraggableItem != null) occupyingDraggableItem.lastPushedFromTile = parentAfterDrag;
                            else canPush = false;
                            
                            gameManager.RegisterPushedPiece(draggableItem);
                            gameManager.RegisterPushedPiece(occupyingDraggableItem);
                            
                            //FourInARow.instance.CheckForRow(piece, parentAfterDrag);
                        }
                        else 
                        {
                            canTokenBePushed = IsValidTokenMove(parentAfterDrag.name, nextTile.name);
                            if (canTokenBePushed)
                            {
                                //Handle pushing a Token with a Stone
                                HandleTokenMove(occupyingPiece, nextTile);
                                HandlePieceMove(piece, parentAfterDrag);

                                if (draggableItem != null) draggableItem.lastPushedFromTile = parentBeforeDrag;
                                else canPush = false;

                                if (occupyingDraggableItem != null) occupyingDraggableItem.lastPushedFromTile = parentAfterDrag;
                                else canPush = false;
                                
                                gameManager.RegisterPushedPiece(draggableItem);
                                gameManager.RegisterPushedPiece(occupyingDraggableItem);
                                
                                if (nextTile.name == "D4")
                                {
                                    if (occupyingPiece.CompareTag("Player1Token"))
                                    {
                                        GameManager.instance.Player1Wins();
                                    }
                                    else if (occupyingPiece.CompareTag("Player2Token"))
                                    {
                                        GameManager.instance.Player2Wins();
                                    }
                                }
                                
                                //FourInARow.instance.CheckForRow(piece, parentAfterDrag);
                                
                                //Change turns if Token has been pushed by its own stone
                                /*if ((piece.CompareTag("Player1Piece") && occupyingPiece.CompareTag("Player1Token")) || (piece.CompareTag("Player2Piece") && occupyingPiece.CompareTag("Player2Token")))
                                {
                                    isRegister = true;
                                    gameManager.SwitchTurn();
                                }
                                //Disable Stones from moving if Stone has pushed or been moved
                                else
                                {
                                    gameManager.RegisterPieceMove();
                                }*/
                            }
                        }
                    }
                    else
                    {
                        RevertMove(piece, parentBeforeDrag);
                        return false;
                    }
                }
            }
            fourInARow.CheckForRow(piece, parentAfterDrag);
            
            //Change turns if Token has been moved
            if (piece.CompareTag("Player1Token") || piece.CompareTag("Player2Token"))// && !gameManager.hasSwitchedTurn)
            {
                //gameManager.SwitchTurn();
                return true;
            }
            //Disable Stones from moving if Stone has pushed or been moved
            else
            {
                //isRegister = false;
                gameManager.RegisterPieceMove();
            }
            
            //return true;
            return false;
        }
        else
        {
            //Show movement errors
            if (!canStoneMove)
            {
                gameManager.EnableStoneErr();
            }

            if (!canTokenMove)
            {
                gameManager.EnableTokenErr();
            }

            if (!canTokenBePushed)
            {
                gameManager.EnableTokenPushErr();
            }

            if (!canTokenMoveOverTiles)
            {
                gameManager.EnableTokenMoverOverErr();
            }

            if (!canPush)
                gameManager.EnablePushErr();
            
            RevertMove(piece, parentBeforeDrag);
            return false;
        }
    }

    private void HandlePieceMove(Transform piece, Transform parentAfterDrag)
    {
        piece.SetParent(parentAfterDrag);
        piece.position = parentAfterDrag.position;
    }

    private void HandleTokenMove(Transform piece, Transform parentAfterDrag)
    {
        piece.SetParent(parentAfterDrag);
        piece.position = parentAfterDrag.position;
    }

    private void RevertMove(Transform piece, Transform parentBeforeDrag)
    {
        piece.SetParent(parentBeforeDrag);
        piece.position = parentBeforeDrag.position;
    }

    public static Transform GetPieceAtPosition(Transform tile)
    {
        foreach (Transform child in tile)
        {
            if (child.CompareTag("Player1Piece") || child.CompareTag("Player1Token") || child.CompareTag("Player1PieceNotMovable") || child.CompareTag("Player2Piece") || child.CompareTag("Player2Token") || child.CompareTag("Player2PieceNotMovable"))
            {
                return child;
            }
        }
        return null;
    }
    
    private Transform GetNextTile(string currentTile, Vector3 direction)
    {
        char currentRow = currentTile[0];
        int currentCol = int.Parse(currentTile.Substring(1));
        
        int rowOffset = Mathf.RoundToInt(direction.y);
        int colOffset = Mathf.RoundToInt(direction.x);

        char nextRow = (char)(currentRow - rowOffset);
        int nextCol = currentCol + colOffset;

        //Check that the next tile exists
        if (nextRow < 'A' || nextRow > 'G' || nextCol < 1 || nextCol > 7)
        {
            return null;
        }

        string nextTileName = nextRow.ToString() + nextCol.ToString();

        GameObject nextTileObject = GameObject.Find(nextTileName);
        return nextTileObject != null ? nextTileObject.transform : null;
    }
    
}

