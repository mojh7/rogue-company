using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEditor : EditorWindow
{
    int width, height,size;
    string name;
    GameObject obj;
    GameObject roomObj;
    ObjectType objectType;
    Sprite objectSprite;
    [MenuItem("Custom/Map")]

    static public void ShowWindow()
    {
        // 윈도우 생성
        MapEditor window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));

    }
 

    void OnGUI()
    {
        BeginWindows();
        obj = Object.FindObjectOfType<Map.MapManager>().gameObject;
        EndWindows();
        width = EditorGUILayout.IntField("width", width);
        height = EditorGUILayout.IntField("height", height);
        size = EditorGUILayout.IntField("size", size);
        name = EditorGUILayout.TextField("name", name);
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
        objectType = (ObjectType)EditorGUILayout.EnumPopup("ObjectType", objectType);
        objectSprite = (Sprite)EditorGUILayout.ObjectField("ObjectSprite", objectSprite, typeof(Sprite), allowSceneObjects: true);
        if(GUILayout.Button("Create Object"))
            CreateObject();
    }

    void CreateObject()
    {
        if (objectSprite == null|| roomObj == null)
            return;
        GameObject gameObject = new GameObject();
        gameObject.name = "Object";
        gameObject.transform.parent = roomObj.transform;
        gameObject.AddComponent<SpriteRenderer>();
        switch (objectType)
        {
            case ObjectType.BLOCK:
                gameObject.AddComponent<Block>().Init(objectSprite);
                break;
            case ObjectType.EVENT:
                gameObject.AddComponent<Block>().Init(objectSprite);
                break;
            case ObjectType.MOVED:
                gameObject.AddComponent<Block>().Init(objectSprite);
                break;
        }
    }
    
    void CreateTilemap()
    {
        if (obj == null || size == 0 || width == 0 || height == 0)
            return;
        Tilemap tilemap;

        if(roomObj == null)
        {
            GameObject gameObject = new GameObject();
            roomObj = gameObject;
            roomObj.transform.parent = obj.transform;
            gameObject.name = "Room";
            tilemap = gameObject.AddComponent<Tilemap>();
            gameObject.AddComponent<TilemapRenderer>();
            gameObject.AddComponent<CompositeCollider2D>();
            gameObject.AddComponent<TilemapCollider2D>();
            gameObject.transform.parent = obj.transform;
        }
        else
        {
            tilemap = roomObj.GetComponent<Tilemap>();
        }

        tilemap.ClearAllTiles();
        TileBase[] wall = TileManager.GetInstance().wallTile;

        for (int i = 0; i < width * size; i++)
        {
            for(int j = 0; j < height * size; j++)
            {
                if (i == 0 || j == 0 || i == width * size - 1 || j == height * size - 1)
                {
                    if (i == 0 || j == 0 || i == width * size - 1 || j == height * size - 1)
                    {
                        if (i == 0 & j == 0)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[0]);
                        else if (i == 0 && j == height * size - 1)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[5]);
                        else if (i == width * size - 1 && j == 0)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[2]);
                        else if (i == width * size - 1 && j == height * size - 1)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[7]);
                        else if (i == 0)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[3]);
                        else if (j == 0)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[1]);
                        else if (i == width * size - 1)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[4]);
                        else
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[6]);
                    }
                }
            }
        }
    }
    
    void SaveTilemap()
    {
        if (obj == null || roomObj == null || size == 0 || width == 0 || height == 0 || name == "")
            return;
        RoomSet roomSet = new RoomSet(width, height);

        foreach (Transform child in roomObj.GetComponentsInChildren<Transform>())
        {
            roomSet.Add(child.GetComponent<CustomObject>());
        }

    }

    void RoomsetToDate()
    {

    }
 
    void RemoveTilemap()
    {
        if (obj == null || roomObj == null)
            return;
        Tilemap tilemap = roomObj.GetComponent<Tilemap>();
        tilemap.ClearAllTiles();
    }

    void DrawBoundary()
    {
        Debug.DrawLine(Vector2.zero, new Vector2(0, height * size), Color.blue , 30);
        Debug.DrawLine(new Vector2(0, height * size), new Vector2(width * size, height * size), Color.blue, 30);
        Debug.DrawLine(new Vector2(width * size, height * size), new Vector2(width * size, 0), Color.blue, 30);
        Debug.DrawLine(new Vector2(width * size, 0), Vector2.zero, Color.blue, 30);
        Debug.DrawLine(new Vector2(width * size, height * size), Vector2.zero, Color.blue, 30);
        Debug.DrawLine(new Vector2(width * size, 0), new Vector2(0, height * size), Color.blue, 30);
    }
}
