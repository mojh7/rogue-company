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

    [Space]

    [SerializeField]
    private RandomTile []floorTile;
    public RandomTile cafeTile;
    public RandomTile restTile;
    public RandomTile hallTile;

    public RuleTile shadowTile;
    public RuleTile wallRuleTile;
    public RuleTile wallRuleTile_h;
    public RuleTile wallRuleTile_v;
    public RuleTile fogTile;

    public RandomTile GetSpriteTile()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        int rand = Random.Range(0, floorTile.Length);
        return floorTile[rand];
    }

    //[SerializeField]
    //private MultiSpriteTile multiSprite;

    //public MultiSpriteTile GetSpriteTile()
    //{
    //    multiSprite.SetIdx();
    //    return multiSprite;
    //}

    public GameObject horizonTilePrefab;
    public GameObject LbwordTilePrefab;
    public GameObject FwordTilePrefab;
    public GameObject RbwordTilePrefab;

    public void DrawBottomLine(Vector3 pos,int op)
    {
        GameObject @object = null;
        if (op == 0)
        {
            @object = Object.Instantiate<GameObject>(LbwordTilePrefab);
        }
        else if(op == 1)
        {
            @object = Object.Instantiate<GameObject>(FwordTilePrefab);
        }
        else if (op == 2)
        {
            @object = Object.Instantiate<GameObject>(RbwordTilePrefab);
        }
        else
        {
            @object = Object.Instantiate<GameObject>(horizonTilePrefab);
        }
        @object.hideFlags = HideFlags.HideInHierarchy;
        @object.transform.position = pos;
    }
}
