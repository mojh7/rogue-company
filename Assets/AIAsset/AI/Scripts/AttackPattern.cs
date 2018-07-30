using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPattern : MonoBehaviour
{ 
    public void Skill(Character character, Character target,float time,float num)
    {
        ActiveSkillManager.Instance.HandTrap(character, target, time, num);
    }
}
