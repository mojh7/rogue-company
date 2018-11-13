using UnityEngine;
using WeaponAsset;

[System.Serializable]
public class BulletPresetInfo
{
    public BulletPresetType presetType;
    public Sprite sprite;
    public float scaleX = 1;
    public float scaleY = 1;
    public ColliderType colliderType;
    public BulletAnimationType spriteAnimation;
    public float lifeTime = -1;
    public int effectId = -1;
}

public class BulletPresets : MonoBehaviourSingleton<BulletPresets>
{
    public BulletPresetInfo[] bulletPresetInfoList;
}