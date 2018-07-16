using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * 캐릭터 특수 공격 등 다양한 액티브 스킬들 관련 된 것 만들어야 될 것 같아서 만들었다.
 * 
 * weaponAsset 내용이랑 연계해서 만들 것 같다.
 * 
 */
 [System.Serializable]
public class SkillTimeline
{
    /// <summary> 
    /// ATTACK : weapon를 통한 공격 처리,
    /// BUFF : 버프 효과,
    /// EFFECT : 이펙트 생성(sprite, particle 등등)
    /// </summary>
    public enum BehaviorType { ATTACK, BUFF, EFFECT }
    [SerializeField]
    private BehaviorType behavior;
    [SerializeField]
    private int weaponIndex;
    [SerializeField]
    private int buffIndex;
    [SerializeField]
    private float time;

    public BehaviorType Behavior { get { return behavior; } }
    public int WeaponIndex { get { return weaponIndex; } }
    public int BuffIndex { get { return buffIndex; } }
    public float Time { get { return time; } }
}

[CreateAssetMenu(fileName = "ActiveSkillInfo", menuName = "ActiveSkillInfo")]
public class ActiveSkillInfo : ScriptableObject
{
    [Header("메모 공간")]
    [SerializeField]
    [TextArea(3, 100)] private string memo;

    [SerializeField]
    private string skillName;
    [SerializeField]
    private Weapon[] weapons;
    [SerializeField]
    private EffectApplyType[] effectApplyTypes;
    [SerializeField]
    private SkillTimeline[] skillTimeline;

    public string SkillName { get { return skillName; } }
    public Weapon[] Weapons { get { return weapons; } }
    public EffectApplyType[] EffectApplyTypes { get { return effectApplyTypes; } }
    public SkillTimeline[] SkillTimeline { get { return skillTimeline; } }
}