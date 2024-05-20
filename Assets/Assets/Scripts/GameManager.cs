using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*public GameObject piece;
    
    private GameObject[,] positions = new GameObject[6,6];
    private GameObject[] playerGreen = new GameObject[20];
    private GameObject[] playerBlue = new GameObject[20];

    private string currentPlayer = "green";

    private bool gameOver = false;
    
    void Start()
    {
        playerGreen = new GameObject[]
        {
            Create("green_Token", 6, 0), Create("green_Stone", 6, 1), Create("green_Stone", 5, 0)
        };
        playerBlue = new GameObject[]
        {
            Create("blue_Token", 0, 6), Create("blue_Stone", 0, 5), Create("blue_Stone", 1, 6)
        };

        for (int i = 0; i < playerGreen.Length; i++)
        {
            SetPosition(playerGreen[i]);
            SetPosition(playerBlue[i]);
        }
        //Instantiate(piece, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(piece, new Vector3(0, 0, -1), Quaternion.identity);
        Movement1 mv = obj.GetComponent<Movement1>();
        mv.name = name;
        mv.SetXBoard(x);
        mv.SetYBoard(y);
        mv.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Movement1 mv = obj.GetComponent<Movement1>();

        positions[mv.GetXBoard(), mv.GetYBoard()] = obj;
    }*/
    
    public static GameManager instance;
    public GameObject Player1Panel;
    public GameObject Player2Panel;
    public GameObject endSetupPanel;

    public enum Player { Player1, Player2 }
    public Player currentPlayer;

    public List<DraggableItem> player1Pieces;
    public List<DraggableItem> player2Pieces;
    
    private int player1MovedCount = 0;
    private int player2MovedCount = 0;

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
    
    private void InitializePieces()
    {
        // Find and filter pieces by their tag using LINQ
        player1Pieces = FindObjectsOfType<DraggableItem>().Where(p => p.CompareTag("Player1Piece")).ToList();
        player2Pieces = FindObjectsOfType<DraggableItem>().Where(p => p.CompareTag("Player2Piece")).ToList();
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

        // Check if all pieces have been moved
        if (player1MovedCount == player1Pieces.Count && player2MovedCount == player2Pieces.Count)
        {
            Player1Panel.SetActive(false);
            Player2Panel.SetActive(false);
            endSetupPanel.SetActive(true);
            //ShowSetupEndPanel();
        }
        else
        {
            SwitchTurn();
        }
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
        UpdatePieceInteractivity();
    }

    private void UpdatePieceInteractivity()
    {
        foreach (var piece in player1Pieces)
        {
            //piece.SetInteractable(currentPlayer == Player.Player1);
            piece.SetInteractable(currentPlayer == Player.Player1 && !piece.CompareTag("Player1PieceNotMovable"));
        }

        foreach (var piece in player2Pieces)
        {
            //piece.SetInteractable(currentPlayer == Player.Player2);
            piece.SetInteractable(currentPlayer == Player.Player2 && !piece.CompareTag("Player2PieceNotMovable"));
        }
    }

    public void CloseSetupEndPanel()
    {
        endSetupPanel.SetActive(false);
    }
}
