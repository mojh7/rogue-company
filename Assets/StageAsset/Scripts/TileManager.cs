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

    public RandomTile cafeTile;
    public RandomTile restTile;
    public RandomTile hallTile;

    public RuleTile shadowTile;
    public RuleTile wallRuleTile;
    public RuleTile wallRuleTile_h;
    public RuleTile wallRuleTile_v;
    public RuleTile fogTile;


    //[SerializeField]
    //private MultiSpriteTile multiSprite;

    //public MultiSpriteTile GetSpriteTile()
    //{
    //    multiSprite.SetIdx();
    //    return multiSprite;
    //}

    public Sprite horizonTilePrefab;
    public Sprite LbwordTilePrefab;
    public Sprite FwordTilePrefab;
    public Sprite RbwordTilePrefab;

    public void DrawBottomLine(Vector3 pos,int op)
    {
        GameObject @object = @object = Object.Instantiate<GameObject>(ResourceManager.Instance.wallPrefab);
        if (op == 0)
        {
            @object.GetComponent<SpriteRenderer>().sprite = LbwordTilePrefab;
        }
        else if(op == 1)
        {
            @object.GetComponent<SpriteRenderer>().sprite = FwordTilePrefab;
        }
        else if (op == 2)
        {
            @object.GetComponent<SpriteRenderer>().sprite = RbwordTilePrefab;
        }
        else
        {
            @object.GetComponent<SpriteRenderer>().sprite = horizonTilePrefab;
        }
        @object.hideFlags = HideFlags.HideInHierarchy;
        @object.transform.position = pos;
    }
}
