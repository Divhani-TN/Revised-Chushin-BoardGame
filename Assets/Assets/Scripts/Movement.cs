using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject manager;
    public GameObject movePlate;

    private int xBoard = -1;
    private int yBoard = -1;

    private string Player;

    public Sprite green_Stone, green_Token;
    public Sprite blue_Stone, blue_Token;

    public void Activate()
    {
        manager = GameObject.FindGameObjectWithTag("GameController");
        SetCoords();

        switch (this.name)
        {
            case "green_Stone": this.GetComponent<SpriteRenderer>().sprite = green_Stone;
                break;
            case "green_Token": this.GetComponent<SpriteRenderer>().sprite = green_Token;
                break;
            
            case "blue_Stone": this.GetComponent<SpriteRenderer>().sprite = blue_Stone;
                break;
            case "blue_Token": this.GetComponent<SpriteRenderer>().sprite = blue_Token;
                break;
        }
    }

    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard()
    {
        return xBoard;
    }
    
    public int GetYBoard()
    {
        return yBoard;
    }
    
    public void SetXBoard(int x)
    {
        xBoard = x;
    }
    
    public void SetYBoard(int y)
    {
        xBoard = y;
    }
}
