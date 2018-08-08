using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviourSingleton<TileManager> {
    public Tilemap EventFloorTileMap;
    public Tilemap floorTileMap;
    public Tilemap verticalWallTileMap;
    public Tilemap horizonWallTileMap;
    public Tilemap shadowTileMap;
    public Tilemap fogTileMap;

    public RandomTile floorTile;
    public RandomTile cafeTile;
    public RandomTile restTile;

    public RuleTile shadowTile;
    public RuleTile verticalWallRuleTile;
    public RuleTile horizonWallRuleTile;
    public RuleTile fogTile;
}
