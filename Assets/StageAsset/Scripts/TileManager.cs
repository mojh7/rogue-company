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

    public GameObject tilePrefabs;

    public void DrawBottomLine(int width)
    {
        for(int i=0;i<width;i++)
        {
            GameObject @object = Object.Instantiate<GameObject>(tilePrefabs);
            @object.transform.position = new Vector3(i + 0.7f, .5f);
            @object.hideFlags = HideFlags.HideInHierarchy;
        }
    }
}
