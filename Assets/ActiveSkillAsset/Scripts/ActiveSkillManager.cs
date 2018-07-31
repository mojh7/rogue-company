using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillManager : MonoBehaviourSingleton<ActiveSkillManager>
{
    #region public
    public bool Charm(Character attacker, object victim, float time, float amount)
    {
        if (!attacker || time < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(Charm, attacker, victim, time, amount));
        return true;
    }

    public bool IncreaseStatus(Character attacker, object victim, float time, float amount)
    {
        if (!attacker || time < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(IncreaseStatus, attacker, victim, time, amount));
        return true;
    }

    public bool Confuse(Character attacker, object victim, float time, float amount)
    {
        if (!attacker || time < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(Confuse, attacker, victim, time, amount));
        return true;
    }

    public bool HandUp(Character attacker, object radius, float time, float amount, float num)
    {
        if (!attacker || time < 0 || amount < 0 || num < 1)
        {
            return false;
        }
        for (int i = 0; i < num; i++)
        {
            float randTime = UnityEngine.Random.Range(0, time + 1);
            StartCoroutine(CoroutineSkill(HandUp, attacker, radius, randTime, amount));
        }
        return true;
    }

    public bool HandClap(Character attacker, object unneeded, float time, float amount)
    {
        if (!attacker|| time < 0 || amount < 0)
        {
            return false;
        }

        StartCoroutine(CoroutineSkill(HandClap, attacker, attacker.transform.position + Vector3.left, 0, amount));
        StartCoroutine(CoroutineSkill(HandClap, attacker, attacker.transform.position + Vector3.right, time, amount));
        return true;
    }

    public bool SpawnServant(Character attacker, object servantData, float time, float amount)
    {
        if (!attacker || time < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(HandClap, attacker, servantData, time, amount));
        return true;
    }
    #endregion
    #region private
    private void Charm(Character attacker, object victim, float amount)
    {

    }

    private void IncreaseStatus(Character attacker, object victim, float amount)
    {

    }

    private void Confuse(Character attacker, object victim, float amount)
    {

    }

    private void HandUp(Character attacker, object radius, float amount)
    {
        float rad = (float)radius;
        Vector2 randPos = UnityEngine.Random.insideUnitCircle * rad;
        GameObject gameObject = ResourceManager.Instance.objectPool.GetPooledObject();
        gameObject.transform.position = new Vector2(attacker.transform.position.x + randPos.x, attacker.transform.position.y + randPos.y);
        gameObject.AddComponent<Alert>();
        gameObject.GetComponent<Alert>().Init(HandUpPart, attacker, amount, 1);
        gameObject.GetComponent<Alert>().Active();
    }

    private void HandUpPart(Vector3 pos, object character, float amount)
    {
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = pos;
        gameObject.AddComponent<CollisionSkill>().Init(character as Character, amount, "handUp");
    }

    private void HandClap(Character attacker, object unneeded, float amount)
    {
        Vector3 pos = (Vector3)unneeded;

        HandClapPart(pos, attacker, amount);
    }

    private void HandClapPart(Vector3 pos, Character character, float amount)
    {
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = pos;
        gameObject.AddComponent<CollisionSkill>().Init(character, amount, "handClap");
    }
    #endregion
    #region coroutine
    IEnumerator CoroutineSkill(Action<Character, object, float> action, Character attacker, object parameter, float time, float amount)
    {
        float startTime = Time.time;
        float elapsedTime = 0;
        while (time >= elapsedTime)
        {
            elapsedTime = Time.time - startTime;
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        action(attacker, parameter, amount);
    }
    #endregion
}
