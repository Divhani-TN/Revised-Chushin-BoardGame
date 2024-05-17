using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject piece;
    
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
        Movement mv = obj.GetComponent<Movement>();
        mv.name = name;
        mv.SetXBoard(x);
        mv.SetYBoard(y);
        mv.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Movement mv = obj.GetComponent<Movement>();

        positions[mv.GetXBoard(), mv.GetYBoard()] = obj;
    }
}
