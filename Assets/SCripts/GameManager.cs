using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int currentPlayer = 1;
    public int[,] grid = new int[7, 7]; 
    public int tokensLostOnLoss = 1;

    public void PlayerMove(int row, int col)
    {
        // Place the piece in the grid
        grid[row, col] = currentPlayer;

        // Check if the current player has four in a row
        if (CheckFourInARow(currentPlayer))
        {
            Debug.Log("Player " + currentPlayer + " has four in a row!");

            // The other player loses a token
            int otherPlayer = currentPlayer == 1 ? 2 : 1;
            Debug.Log("Player " + otherPlayer + " loses a token!");
        }

        // Switch to the other player
        currentPlayer = currentPlayer == 1 ? 2 : 1;
        Debug.Log("It's now player " + currentPlayer + "'s turn.");
    }

    bool CheckFourInARow(int player)
    {
        // Check for four in a row in the grid for the given player
        // This is a simple and not very efficient way to do it
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (grid[i, j] == player && grid[i, j + 1] == player && grid[i, j + 2] == player && grid[i, j + 3] == player)
                {
                    return true;
                }
            }
        }

        return false;
    }
}


  