using UnityEngine;
using WeaponAsset;

/// <summary>
/// 정보 일괄 수정을 위해 bullet preset화, 주로 외형적인 정보만 설정
/// </summary>
[System.Serializable]
public class BulletPresetInfo
{
    public BulletPresetType presetType;
    public Sprite sprite;
    public float scaleX = 0;
    public float scaleY = 0;
    public ColliderType colliderType;
    public BulletAnimationType spriteAnimation;
    public float lifeTime = -1;
    public int effectId = -1;
}

public class BulletPresets : MonoBehaviourSingleton<BulletPresets>
{
    public BulletPresetInfo[] bulletPresetInfoList;
}