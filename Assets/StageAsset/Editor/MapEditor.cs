using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Collections.Generic;

public class MapEditor : EditorWindow
{
    public enum NPCType { Astrologer }
    [SerializeField]
    GameObject prefabs;

    public Vector2 scrollPosition = Vector2.zero;

    int width, height, size = 3, gage;
    string roomName;
    GameObject obj;
    GameObject roomObj;
    ObjectType objectType;
    ObjectAbnormalType objectAbnormalType;
    RoomType roomType;
    NPCType npcType;
    int spriteNum;
    Sprite multipleSprite;
    Sprite objectSprite;
    Sprite[] objectSprites;
    RoomSet worldRoomSet;

    static MapEditor window;
    [MenuItem("Custom/RoomEditor")]

    static public void ShowWindow()
    {
        // 윈도우 생성
        window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
    }

    void OnGUI()
    {
        BeginWindows();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true, GUILayout.Width(window.position.width), GUILayout.Height(window.position.height));
        obj = Object.FindObjectOfType<Map.MapManager>().gameObject;
        roomName = EditorGUILayout.TextField("roomName", roomName);
        width = EditorGUILayout.IntField("width", width);
        height = EditorGUILayout.IntField("height", height);
        gage = EditorGUILayout.IntField("gage", gage);
        roomType = (RoomType)EditorGUILayout.EnumPopup("RoomType", roomType);
        if (GUILayout.Button("Create Room"))
        {
            CreateTilemap();
        }
        if (GUILayout.Button("Remove Room"))
        {
            RemoveTilemap();
        }
        GUILayout.Space(20);
        objectType = (ObjectType)EditorGUILayout.EnumPopup("ObjectType", objectType);
        if (objectType == ObjectType.NPC)
        {
            npcType = (NPCType)EditorGUILayout.EnumPopup("NPCType", npcType);
        }
        if(objectType == ObjectType.SKILLBOX)
        {
            objectAbnormalType = (ObjectAbnormalType)EditorGUILayout.EnumPopup("ObjectAbnormalType", objectAbnormalType);
        }
        EditorGUI.BeginChangeCheck();
        spriteNum = EditorGUILayout.IntField("spriteNum", spriteNum);
        if (EditorGUI.EndChangeCheck())
        {
            objectSprites = new Sprite[spriteNum];
        }
        if (spriteNum == 0)
            multipleSprite = (Sprite)EditorGUILayout.ObjectField("multipleSprite", multipleSprite, typeof(Sprite), allowSceneObjects: true);
        for (int i = 0; i < spriteNum; i++)
        {
            objectSprites[i] = (Sprite)EditorGUILayout.ObjectField("ObjectSprite", objectSprites[i], typeof(Sprite), allowSceneObjects: true);
        }
        if (GUILayout.Button("Create Object"))
            CreateObject();
        if (GUILayout.Button("Save Object"))
            SaveObject();
        GUILayout.Space(20);
        worldRoomSet = (RoomSet)EditorGUILayout.ObjectField("RoomSet", worldRoomSet, typeof(RoomSet), allowSceneObjects: true);
        if (GUILayout.Button("Save Roomset"))
            SaveRoomset();
        if (GUILayout.Button("Load Roomset"))
            LoadRoomset();
        EditorGUILayout.EndScrollView();
        EndWindows();

    }

    void CreateObject()
    {
        if (roomObj == null)
            return;

        GameObject gameObject = Object.Instantiate(prefabs);
        gameObject.name = objectType.ToString();
        gameObject.transform.parent = roomObj.transform;
        ObjectData objectData;
        if (multipleSprite != null && spriteNum == 0)
        {
            Sprite[] objectSprites = GetSprites(multipleSprite);
            objectData = new ObjectData(Vector3.zero, Vector3.one, objectType, objectAbnormalType, objectSprites, npcType.ToString());
            objectData.LoadObject(gameObject);
            gameObject.GetComponent<SpriteRenderer>().sprite = objectSprites[0];
        }
        else
        {
            objectData = new ObjectData(Vector3.zero, Vector3.one, objectType, objectAbnormalType, objectSprites, npcType.ToString());
            objectData.LoadObject(gameObject);
            if(objectSprites != null)
                if (objectSprites.Length > 0)
                    gameObject.GetComponent<SpriteRenderer>().sprite = objectSprites[0];
        }
        objectSprites = new Sprite[spriteNum];
    }

    void SaveObject()
    {
        ObjectData currentData;
        if (multipleSprite != null && spriteNum == 0)
        {
            Sprite[] objectSprites = GetSprites(multipleSprite);
            currentData = new ObjectData(Vector3.zero,Vector3.one, objectType, objectAbnormalType, objectSprites, npcType.ToString());
        }
        else
        {
            currentData = new ObjectData(Vector3.zero, Vector3.one, objectType, objectAbnormalType, objectSprites, npcType.ToString());

        }
        objectSprites = new Sprite[spriteNum];

        ObjectSet objectSet = new ObjectSet();
        objectSet.Add(currentData);
        string path = "";

        if (path == "")
        {
            path = "Assets/ObjectAsset/GameData/";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + currentData.objectType + ".asset");

        AssetDatabase.CreateAsset(objectSet, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = objectSet;
    }

    CustomObject CreateObject(ObjectType objectType)
    {
        if (roomObj == null)
            return null;
        GameObject gameObject = Object.Instantiate(prefabs);
        gameObject.name = objectType.GetType().ToString();
        gameObject.transform.parent = roomObj.transform;

        ObjectData objectData = new ObjectData(Vector3.zero, Vector3.one, objectType, objectAbnormalType, objectSprites, npcType.ToString());
        objectData.LoadObject(gameObject);
        Selection.activeObject = gameObject;
        return gameObject.GetComponent<CustomObject>();
    }

    Sprite[] GetSprites(Sprite sprite)
    {
        string path = AssetDatabase.GetAssetPath(sprite);
        Object[] objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        List<Sprite> l = new List<Sprite>(objects.Length);
        foreach (var i in objects)
        {
            var s = i as Sprite;
            l.Add(s);
        }
        return l.ToArray();
    }

    void SaveRoomset()
    {
        if (obj == null || roomObj == null || size == 0 || width == 0 || height == 0)
            return;
        RoomSet roomSet = ScriptableObject.CreateInstance<RoomSet>();

        roomSet.Init(width, height, size, gage, roomType);
        foreach (Transform child in roomObj.GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<CustomObject>() != null)
            {
                CustomObject customObject = child.GetComponent<CustomObject>();
                roomSet.Add(new ObjectData(customObject.transform.position, customObject.transform.localScale, customObject.objectType, objectAbnormalType, customObject.sprites, customObject.GetType().ToString()));
            }
        }
        if (roomType == RoomType.MONSTER || roomType == RoomType.BOSS)
        {
            if(roomSet.gage == 0)
            {
                roomSet.gage = width * height * 2;
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
            path = "Assets/StageAsset/GameData/";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + width + 'x' + height + roomType.ToString() + " " + _name + ".asset");

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
        tilemap.tileAnchor = new Vector3(1.5f, 0, 0);
        tilemap.ClearAllTiles();
        RuleTile wallRuleTile = TileManager.Instance.wallRuleTile;

        for (int i = 0; i < width * size; i++)
        {
            for(int j = 0; j < height * size; j++)
            {
                if (i == 0 || i == width * size - 1)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), wallRuleTile);
                }
                else if (j == height * size - 1 || j == 0)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), wallRuleTile);
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
        roomType = worldRoomSet.roomType;
        CreateTilemap();
        for (int i = 0; i < worldRoomSet.objectDatas.Count; i++)
            DataToObject(worldRoomSet.objectDatas[i]);
    }

    void DataToObject(ObjectData _objectData)
    {
        GameObject gameObject = Object.Instantiate(prefabs);

        _objectData.LoadObject(gameObject);
        gameObject.name = _objectData.objectType.ToString();
        gameObject.transform.position = new Vector3(_objectData.position.x,_objectData.position.y,0);
        gameObject.transform.localScale = new Vector3(_objectData.scale.x, _objectData.scale.y, _objectData.scale.y);

        gameObject.transform.parent = roomObj.transform;
        if(_objectData.sprites != null)
            if (_objectData.sprites.Length > 0)
                gameObject.GetComponent<SpriteRenderer>().sprite = _objectData.sprites[0];
    }
}
