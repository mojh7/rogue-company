using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviourSingleton<TileManager> {

    public Tilemap floorTileMap;
    public Tilemap verticalWallTileMap;
    public Tilemap horizonWallTileMap;
    public Tilemap shadowTileMap;

    public RandomTile floorTile;
    public RuleTile shadowTile;
    public RuleTile verticalWallRuleTile;
    public RuleTile horizonWallRuleTile;
}
