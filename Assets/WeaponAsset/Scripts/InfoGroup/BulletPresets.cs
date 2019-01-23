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
    public float autoSizeRatio = 1f;
    public BulletAnimationType spriteAnimation;
    public float lifeTime = -1;
    public int effectId = -1;
    public string bulletParticleName;
    public string impactParticleName;
    public Vector2 manualSize;
    public Vector2 colliderOffset;
    public float circleManualRadius;
    public bool isFixedAngle;
}

public class BulletPresets : MonoBehaviourSingleton<BulletPresets>
{
    //[ContextMenuItem("AutoEdit", "AutoEdit")]
    public BulletPresetInfo[] bulletPresetInfoList;

    //private void AutoEdit()
    //{
    //    for (int i = 37; i < bulletPresetInfoList.Length; i++)
    //    {
    //        int refIndex = 31 + ((i - 37) % 6);
    //        bulletPresetInfoList[i].scaleX = bulletPresetInfoList[refIndex].scaleX;
    //        bulletPresetInfoList[i].scaleY = bulletPresetInfoList[refIndex].scaleY;
    //        bulletPresetInfoList[i].colliderType = bulletPresetInfoList[refIndex].colliderType;
    //        bulletPresetInfoList[i].autoSizeRatio = bulletPresetInfoList[refIndex].autoSizeRatio;
    //        bulletPresetInfoList[i].spriteAnimation = bulletPresetInfoList[refIndex].spriteAnimation;
    //        bulletPresetInfoList[i].lifeTime = bulletPresetInfoList[refIndex].lifeTime;
    //        bulletPresetInfoList[i].effectId = 0;
    //        bulletPresetInfoList[i].manualSize = bulletPresetInfoList[refIndex].manualSize;
    //        bulletPresetInfoList[i].colliderOffset = bulletPresetInfoList[refIndex].colliderOffset;
    //        bulletPresetInfoList[i].circleManualRadius = bulletPresetInfoList[refIndex].circleManualRadius;
    //        bulletPresetInfoList[i].isFixedAngle = bulletPresetInfoList[refIndex].isFixedAngle;
    //    }  
    //}
}