using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    #region serializeFiled
    [SerializeField]
    private float hp;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private RuntimeAnimatorController animatorController;
    [SerializeField]
    private WeaponInfo weaponInfo;
    [SerializeField]
    private BT.Task task;
    #endregion
  
    #region property
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
    public RuntimeAnimatorController AnimatorController
    {
        get
        {
            return animatorController;
        }
    }
    public WeaponInfo WeaponInfo
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
    #endregion

}
