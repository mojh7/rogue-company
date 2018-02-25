using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEditor : EditorWindow
{
    int width, height;

    GameObject obj;
    [MenuItem("Custom/Map")]

    static public void ShowWindow()
    {
        // 윈도우 생성
        MapEditor window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));

    }
 

    void OnGUI()
    {
        obj = (GameObject)EditorGUILayout.ObjectField("Map", obj, typeof(GameObject));
        width = EditorGUILayout.IntField("width", 30);
        height = EditorGUILayout.IntField("height", 30);

        if (GUILayout.Button("Show Boundary 30 duration"))
        {
            DrawBoundary();
        }
        if (GUILayout.Button("Create Tilemap"))
        {
            CreateTilemap();
        }
        if (GUILayout.Button("Save Tilemap"))
        {
            SaveTilemap();
        }
        if (GUILayout.Button("Remove Tilemap"))
        {
            RemoveTilemap();
        }
    }

    void CreateTilemap()
    {
        if (obj == null)
            return;
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<Tilemap>();
        gameObject.AddComponent<TilemapRenderer>();
        gameObject.transform.parent = obj.transform;
    }
    
    void SaveTilemap()
    {
        if (obj == null)
            return;
        Tilemap tilemap = obj.transform.GetChild(0).GetComponent<Tilemap>();
        //List<TileBase> FloorTile = new List<TileBase>();
        //List<TileBase> WallTile = new List<TileBase>();
        //List<TileBase> DoorTile = new List<TileBase>();

        //for (int i = 0; i < width; i++)
        //{
        //    for(int j=0;j < height; j++)
        //    {
        //        TileBase tileBase = tilemap.GetTile(new Vector3Int(i, j, 0));

        //        if (tileBase == null)
        //        {

        //        }
        //        else if(tileBase.name == "Floor")
        //        {
        //            FloorTile.Add(tileBase);
        //        }
        //        else if(tileBase.name == "Wall")
        //        {
        //            WallTile.Add(tileBase);
        //        }
        //        else if(tileBase.name == "Door")
        //        {
        //            DoorTile.Add(tileBase);
        //        }
        //    }
        //}
    }
    void RemoveTilemap()
    {
        if (obj == null)
            return;
        Tilemap tilemap = obj.transform.GetChild(0).GetComponent<Tilemap>();
        tilemap.ClearAllTiles();
    }

    void DrawBoundary()
    {
        Debug.DrawLine(Vector2.zero, new Vector2(0, height), Color.blue , 30);
        Debug.DrawLine(new Vector2(0, height), new Vector2(width, height), Color.blue, 30);
        Debug.DrawLine(new Vector2(width, height), new Vector2(width, 0), Color.blue, 30);
        Debug.DrawLine(new Vector2(width, 0), Vector2.zero, Color.blue, 30);
        Debug.DrawLine(new Vector2(width, height), Vector2.zero, Color.blue, 30);
        Debug.DrawLine(new Vector2(width, 0), new Vector2(0, height), Color.blue, 30);
    }
}
