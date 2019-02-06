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
    public BulletImpactType bulletImpactType;
    public string bulletParticleName;
    public string impactParticleName;
    public Vector2 manualSize;
    public Vector2 colliderOffset;
    public float circleManualRadius;
    public bool isFixedAngle;
}

[System.Serializable]
public class BulletParticlePresetInfo
{
    public BulletParticleType presetType;
    public BulletImpactType bulletImpactType;
    public ColliderType colliderType;
    public float autoSizeRatio = 1f;
    public Vector2 manualSize;
    public Vector2 colliderOffset;
    public float circleManualRadius;
}

public class BulletPresets : MonoBehaviourSingleton<BulletPresets>
{
    public BulletPresetInfo[] bulletPresetInfoList;
    public BulletParticlePresetInfo[] bulletParticlePresetInfoList;

    [Header("파티클 프리팹 이름 반자동으로 생성 해주기")]
    public string originalName;
    public string missileName;
    public string impactName;
    public string[] colorName;
    public int[] colorOrder;

    [ContextMenu("Output Particle Name")]
    public void PrintParticleNames()
    {
        string missileStrTotal = "";
        string impactStrTotal = "";
        for (int i = 0; i < colorOrder.Length; i++)
        {
            missileStrTotal += originalName + missileName + colorName[colorOrder[i]] + ", ";
            impactStrTotal += originalName + impactName + colorName[colorOrder[i]] + ", ";
        }

        Debug.Log(missileStrTotal + "\n" + impactStrTotal);
    }


    [ContextMenu("AutoParticlePresetEdit")]
    public void AutoParticlePresetEdit()
    {
        for (int i = 0; i < bulletParticlePresetInfoList.Length; i++)
        {
            bulletParticlePresetInfoList[i].presetType = (BulletParticleType)(i + 1);
            bulletParticlePresetInfoList[i].bulletImpactType = (BulletImpactType)(i + 7);
            bulletParticlePresetInfoList[i].autoSizeRatio = 1f;
            bulletParticlePresetInfoList[i].colliderType = ColliderType.MANUAL_SIZE_CIRCLE;
            bulletParticlePresetInfoList[i].circleManualRadius = 0.4f;
        }
    }

    [ContextMenu("AutoPresetEdit")]
    public void AutoPresetEdit()
    {
        for (int i = 0; i < bulletPresetInfoList.Length; i++)
        {
            string str = bulletPresetInfoList[i].presetType.ToString();
            if(str.Contains("RED"))
            {
                bulletPresetInfoList[i].bulletImpactType = BulletImpactType.BasicImpactRed;
                Debug.Log(i + ", " + str + " : RED");
            }
            else if (str.Contains("YELLOW"))
            {
                bulletPresetInfoList[i].bulletImpactType = BulletImpactType.BasicImpactYellow;
                Debug.Log(i + ", " + str + " : YELLOW");
            }
            else if (str.Contains("GREEN"))
            {
                bulletPresetInfoList[i].bulletImpactType = BulletImpactType.BasicImpactGreen;
                Debug.Log(i + ", " + str + " : GREEN");
            }
            else if (str.Contains("BLUE"))
            {
                bulletPresetInfoList[i].bulletImpactType = BulletImpactType.BasicImpactBlue;
                Debug.Log(i + ", " + str + " : BLUE");
            }
            else if (str.Contains("PURPLE") || str.Contains("VIOLET"))
            {
                bulletPresetInfoList[i].bulletImpactType = BulletImpactType.BasicImpactPurple;
                Debug.Log(i + ", " + str + " : VIOLET~");
            }
            else if (str.Contains("PINK"))
            {
                bulletPresetInfoList[i].bulletImpactType = BulletImpactType.BasicImpactPink;
                Debug.Log(i + ", " + str + " : PINK");
            }
            else
            {
                Debug.Log(i + ", " + str);
            }
        }

            //for(int i = 0; i < bulletPresetInfoList.Length; i++)
            //{
            //    if(0 == bulletPresetInfoList[i].effectId)
            //    {
            //        bulletPresetInfoList[i].bulletImpactType = BulletImpactType.BasicImpactRed;
            //        Debug.Log(i + " : " + bulletPresetInfoList[i].effectId + ", red");
            //    }
            //    else
            //    {
            //        Debug.Log(i + " : " + bulletPresetInfoList[i].effectId + ", effectId 없음");
            //    }
            //}

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