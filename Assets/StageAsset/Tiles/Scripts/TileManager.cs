using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviourSingleton<TileManager> {

    public Tilemap floorTileMap;
    public Tilemap wallTileMap;
    public Tilemap shadowTileMap;

    public RandomTile floorTile;
    public TileBase shadowTile;
    public RuleTile wallRuleTile;
}
