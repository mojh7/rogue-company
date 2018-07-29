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

    public bool HandTrap(Character attacker, Character victim, float time, float num)
    {
        if (attacker || victim || time < 0)
        {
            return false;
        }
        for(int i=0;i<num;i++)
        {
            float randTime = UnityEngine.Random.Range(0, time + 1);
            StartCoroutine(CoroutineSkill(HandTrap, attacker, victim, randTime));
        }
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
        float radius = Vector2.Distance(attacker.transform.position, victim.transform.position);
        Vector2 randPos = UnityEngine.Random.insideUnitCircle * radius;
        GameObject gameObject = ResourceManager.Instance.objectPool.GetPooledObject();
        gameObject.transform.position = new Vector2(attacker.transform.position.x + randPos.x, attacker.transform.position.y + randPos.y);
        gameObject.AddComponent<Alert>();
        gameObject.GetComponent<Alert>().Init(HandTrap, 0);
        gameObject.GetComponent<Alert>().Active();
    }

    private void HandTrap(Vector3 pos)
    {
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.AddComponent<HandTrap>();
        gameObject.transform.position = pos;
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
