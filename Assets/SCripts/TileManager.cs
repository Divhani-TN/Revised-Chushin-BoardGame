using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;
    public Dictionary<string, Transform> tiles = new Dictionary<string, Transform>();

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

        foreach (Transform tile in transform)
        {
            tiles[tile.name] = tile;
        }
    }
}
