using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillManager : MonoBehaviourSingleton<ActiveSkillManager>
{
    #region public
    public bool Charm(Character attacker, Character victim, float time)
    {
        if (attacker || victim || time < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(Charm, attacker, victim, time));
        return true;
    }

    public bool IncreaseStatus(Character attacker, Character victim, float time)
    {
        if (attacker || victim || time < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(IncreaseStatus, attacker, victim, time));
        return true;
    }

    public bool Confuse(Character attacker, Character victim, float time)
    {
        if (attacker || victim || time < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(Confuse, attacker, victim, time));
        return true;
    }

    public bool HandTrap(Character attacker, Character victim,float time)
    {
        if (attacker || victim || time < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(HandTrap, attacker, victim, time));
        return true;
    }

    #endregion
    #region private
    private void Charm(Character attacker, Character victim)
    {

    }

    private void IncreaseStatus(Character attacker, Character victim)
    {

    }

    private void Confuse(Character attacker, Character victim)
    {

    }

    private void HandTrap(Character attacker, Character victim)
    {

    }

    #endregion
    #region coroutine
    IEnumerator CoroutineSkill(Action<Character, Character> action, Character attacker, Character victim, float time)
    {
        float startTime = Time.deltaTime;
        float elapsedTime = 0;
        while (time < elapsedTime)
        {
            elapsedTime = Time.deltaTime - startTime;
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        action(attacker, victim);
    }
    #endregion
} 
