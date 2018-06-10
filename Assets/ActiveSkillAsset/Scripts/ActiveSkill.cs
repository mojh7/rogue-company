using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * 캐릭터 특수 공격 등 다양한 액티브 스킬들 관련 된 것 만들어야 될 것 같아서 만들었다.
 * 
 * weaponAsset 내용이랑 연계해서 만들 것 같다.
 * 
 */


public class ActiveSkill : ScriptableObject
{
    WeaponManager weaponManager;
    EffectApplyType effectApplyTypes;
}