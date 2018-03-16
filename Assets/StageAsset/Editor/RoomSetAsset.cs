using UnityEngine;
using UnityEditor;

public class RoomSetAsset
{
    [MenuItem("Assets/Create/RoomSet Asset")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<RoomSet>();
    }
}