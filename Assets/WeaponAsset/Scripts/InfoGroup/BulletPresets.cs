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
    public BulletImpactType bulletImpactType;
    public string bulletParticleName;
    public string impactParticleName;
    public Vector2 manualSize;
    public Vector2 colliderOffset;
    public float circleManualRadius;
    public bool isFixedAngle;
}

public class BulletPresets : MonoBehaviourSingleton<BulletPresets>
{
    public BulletPresetInfo[] bulletPresetInfoList;

    [ContextMenu("AutoEdit")]
    public void Autoedit()
    {
        for(int i = 0; i < bulletPresetInfoList.Length; i++)
        {
            if(0 == bulletPresetInfoList[i].effectId)
            {
                bulletPresetInfoList[i].bulletImpactType = BulletImpactType.BasicImpactRed;
                Debug.Log(i + " : " + bulletPresetInfoList[i].effectId + ", red");
            }
            else
            {
                Debug.Log(i + " : " + bulletPresetInfoList[i].effectId + ", effectId 없음");
            }
        }

        //for (int i = 37; i < bulletpresetinfolist.length; i++)
        //{
        //    int refindex = 31 + ((i - 37) % 6);
        //    bulletpresetinfolist[i].scalex = bulletpresetinfolist[refindex].scalex;
        //    bulletpresetinfolist[i].scaley = bulletpresetinfolist[refindex].scaley;
        //    bulletpresetinfolist[i].collidertype = bulletpresetinfolist[refindex].collidertype;
        //    bulletpresetinfolist[i].autosizeratio = bulletpresetinfolist[refindex].autosizeratio;
        //    bulletpresetinfolist[i].spriteanimation = bulletpresetinfolist[refindex].spriteanimation;
        //    bulletpresetinfolist[i].lifetime = bulletpresetinfolist[refindex].lifetime;
        //    bulletpresetinfolist[i].effectid = 0;
        //    bulletpresetinfolist[i].manualsize = bulletpresetinfolist[refindex].manualsize;
        //    bulletpresetinfolist[i].collideroffset = bulletpresetinfolist[refindex].collideroffset;
        //    bulletpresetinfolist[i].circlemanualradius = bulletpresetinfolist[refindex].circlemanualradius;
        //    bulletpresetinfolist[i].isfixedangle = bulletpresetinfolist[refindex].isfixedangle;
        //}
    }
}