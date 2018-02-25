using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour {
    private static TileManager instance;

    public static TileManager GetInstance()
    {
        if (!instance)
        {
            instance = GameObject.FindObjectOfType(typeof(TileManager)) as TileManager;
        }

        return instance;
    }


    public TileBase floorTile;
    public TileBase doorTile;
    public TileBase wallTile;

    
}
