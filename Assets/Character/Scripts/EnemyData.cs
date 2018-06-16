using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField]
    private float hp;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private WeaponInfo weaponInfo;

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
            return moveSpeed;
        }
    }
    public Animator Animator
    {
        get
        {
            return animator;
        }
    }
    public WeaponInfo WeaponInfo
    {
        get
        {
            return weaponInfo;
        }
    }
}
