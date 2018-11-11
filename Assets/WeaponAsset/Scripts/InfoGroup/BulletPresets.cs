using UnityEngine;
using WeaponAsset;

[System.Serializable]
public class BulletPresetInfo
{
    public BulletPresetType presetType;
    public Sprite sprite;
    public float scaleX;
    public float scaleY;
    public ColliderType colliderType;
    public BulletAnimationType spriteAnimation;
}

public class BulletPresets : MonoBehaviourSingleton<BulletPresets>
{
    public BulletPresetInfo[] bulletPresetInfoList;
}