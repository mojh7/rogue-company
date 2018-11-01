using UnityEngine;
using UnityEditor;
using WeaponAsset;

[System.Serializable]
public class BulletPreset
{
    public Sprite sprite;
    public float scaleX;
    public float scaleY;
    public ColliderType colliderType;
    public readonly BulletPresetType a;
}

public class BulletPresetInfo : MonoBehaviourSingleton<BulletPresetInfo>
{
    public BulletPreset[] bulletPreset;
}