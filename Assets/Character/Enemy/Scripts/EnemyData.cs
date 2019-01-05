using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "CharData/EnemyData")]
public class EnemyData : ScriptableObject
{
    #region serializeFiled
    [SerializeField]
    private Color color = Color.white;
    [SerializeField]
    private int price;
    [SerializeField]
    private new string name;
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private float hp;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float size;
    [SerializeField]
    private RuntimeAnimatorController animatorController;
    [SerializeField]
    private List<WeaponInfo> weaponInfo;
    [SerializeField]
    private BT.Task task;
    [SerializeField]
    private SkillData[] skillDatas;
    [SerializeField]
    private EnemyData[] servantDatas;
    [Header("소환 몹 전용, 보스 타겟")]
    [SerializeField]
    private bool followBoss;
    #endregion

    #region property
    public Color Color
    {
        get
        {
            return color;
        }
    }
    public int Price
    {
        get
        {
            return price;
        }
    }
    public string Name
    {
        get
        {
            return name;
        }
    }
    public Sprite Sprite
    {
        get
        {
            return sprite;
        }
    }
    public float HP
    {
        get
        {
            return hp;
        }
    }
    public float Speed
    {
        get
        {
            return speed;
        }
    }
    public float Size
    {
        get
        {
            return size;
        }
    }
    public RuntimeAnimatorController AnimatorController
    {
        get
        {
            return animatorController;
        }
    }
    public List<WeaponInfo> WeaponInfo
    {
        get
        {
            return weaponInfo;
        }
    }
    public BT.Task Task
    {
        get
        {
            return task;
        }
    }
    public SkillData[] SkillDatas
    {
        get
        {
            return skillDatas;
        }
    }
    public EnemyData[] ServantDatas
    {
        get
        {
            return servantDatas;
        }
    }
    public bool FollowBoss
    {
        get
        {
            return followBoss;
        }
    }
    #endregion

}
