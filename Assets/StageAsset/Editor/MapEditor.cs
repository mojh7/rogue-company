using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class MapEditor : EditorWindow
{
    int width, height, size, gage;
    string roomName;
    GameObject obj;
    GameObject roomObj;
    ObjectType objectType;
    RoomType roomType;
    int spriteNum;
    Sprite objectSprite;
    Sprite[] objectSprites;
    RoomSet worldRoomSet;
    [MenuItem("Custom/RoomEditor")]

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
        roomName = EditorGUILayout.TextField("roomName", roomName);
        width = EditorGUILayout.IntField("width", width);
        height = EditorGUILayout.IntField("height", height);
        gage = EditorGUILayout.IntField("gage", gage);
        roomType = (RoomType)EditorGUILayout.EnumPopup("RoomType", roomType);
        size = EditorGUILayout.IntField("size", size);
        if (GUILayout.Button("Create Room"))
        {
            CreateTilemap();
        }
        if (GUILayout.Button("Remove Room"))
        {
            RemoveTilemap();
        }
        objectType = (ObjectType)EditorGUILayout.EnumPopup("ObjectType", objectType);
        EditorGUI.BeginChangeCheck();
        spriteNum = EditorGUILayout.IntField("spriteNum", spriteNum);
        if (EditorGUI.EndChangeCheck())
        {
            objectSprites = new Sprite[spriteNum];
        }
        for (int i = 0; i < spriteNum; i++)
        {
            objectSprites[i] = (Sprite)EditorGUILayout.ObjectField("ObjectSprite", objectSprites[i], typeof(Sprite), allowSceneObjects: true);
        }
        if (GUILayout.Button("Create Object"))
            CreateObject();
        if (GUILayout.Button("Save Roomset"))
            SaveRoomset();
        worldRoomSet = (RoomSet)EditorGUILayout.ObjectField("RoomSet", worldRoomSet, typeof(RoomSet), allowSceneObjects: true);
        if (GUILayout.Button("Load Roomset"))
            LoadRoomset();
    }

    void CreateObject()
    {
        if (roomObj == null)
            return;
        if (roomType == RoomType.MONSTER)
        {
            bool op = false;
            foreach (Transform child in roomObj.GetComponentsInChildren<Transform>())
            {
                if (child.GetComponent<CustomObject>() != null)
                {
                    CustomObject customObject = child.GetComponent<CustomObject>();
                    if (customObject.objectType == ObjectType.SPAWNER)
                        op = true;
                }
            }
            if (op && objectType == ObjectType.NONE || objectType == ObjectType.SPAWNER)
                return;
        }
   
        GameObject gameObject = new GameObject();
        gameObject.name = "Object";
        gameObject.transform.parent = roomObj.transform;
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.AddComponent<PolygonCollider2D>();
        gameObject.AddComponent<Rigidbody2D>();
        ObjectData objectData = new ObjectData(Vector3.zero, objectType, objectSprites);
        objectData.LoadObject(gameObject);
        gameObject.GetComponent<SpriteRenderer>().sprite = objectSprites[0];
    }

    CustomObject CreateObject(ObjectType objectType)
    {
        if (roomObj == null)
            return null;
        GameObject gameObject = new GameObject();
        gameObject.name = "Object";
        gameObject.transform.parent = roomObj.transform;
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.AddComponent<PolygonCollider2D>();

        ObjectData objectData = new ObjectData(Vector3.zero, objectType, objectSprites);
        objectData.LoadObject(gameObject);
        return gameObject.GetComponent<CustomObject>();
    }

    void SaveRoomset()
    {
        if (obj == null || roomObj == null || size == 0 || width == 0 || height == 0)
            return;
        RoomSet roomSet = new RoomSet(width, height, size, gage, roomType);

        foreach (Transform child in roomObj.GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<CustomObject>() != null)
            {
                CustomObject customObject = child.GetComponent<CustomObject>();
                customObject.SetPosition();
                roomSet.Add(new ObjectData(customObject.position, customObject.objectType, customObject.sprites));
            }
        }
        if (roomType == RoomType.MONSTER || roomType == RoomType.BOSS)
        {
            bool op = false;
            foreach (Transform child in roomObj.GetComponentsInChildren<Transform>())
            {
                if (child.GetComponent<CustomObject>() != null)
                {
                    CustomObject customObject = child.GetComponent<CustomObject>();

                    if (customObject.objectType == ObjectType.SPAWNER)
                        op = true;
                }
            }
            if (!op)
            {
                CustomObject tempObj = CreateObject(ObjectType.SPAWNER);
                roomSet.Add(new ObjectData(tempObj.position, tempObj.objectType, tempObj.sprites));
            }
            if(roomSet.gage == 0)
            {
                roomSet.gage = width * height;
            }
        }
        else
        {
            roomSet.gage = 0;
        }
        SaveRoomSet(roomName, roomSet);
    }

    void SaveRoomSet(string _name, RoomSet _roomSet)
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets/StageAsset/GameData/RoomSet/";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path +width+'x'+height +' '+ _name + ".asset");

        AssetDatabase.CreateAsset(_roomSet, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = _roomSet;
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
        RuleTile horizonWallRuleTile = TileManager.Instance.horizonWallRuleTile;
        RuleTile verticalWallRuleTile = TileManager.Instance.verticalWallRuleTile;

        for (int i = 0; i < width * size; i++)
        {
            for(int j = 0; j < height * size; j++)
            {
                if (i == 0 || i == width * size - 1)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), verticalWallRuleTile);
                }
                else if (j == height * size - 1 || j == 0)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), horizonWallRuleTile);
                }
            }
        }
    }
 
    void RemoveTilemap()
    {
        if (obj == null || roomObj == null)
            return;
        DestroyImmediate(roomObj);
    }

    void LoadRoomset()
    {
        if (worldRoomSet == null)
            return;
        RemoveTilemap();
        width = worldRoomSet.width;
        height = worldRoomSet.height;
        size = worldRoomSet.size;
        CreateTilemap();
        for (int i = 0; i < worldRoomSet.objectDatas.Count; i++)
            DataToObject(worldRoomSet.objectDatas[i]);
    }

    void DataToObject(ObjectData _objectData)
    {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.AddComponent<PolygonCollider2D>();
        gameObject.AddComponent<Rigidbody2D>();
        _objectData.LoadObject(gameObject);
        gameObject.name = "Object";
        gameObject.transform.position = new Vector3(_objectData.position.x,_objectData.position.y,0);
        gameObject.transform.parent = roomObj.transform;
        if(_objectData.sprites.Length > 0)
            gameObject.GetComponent<SpriteRenderer>().sprite = _objectData.sprites[0];
    }
}
