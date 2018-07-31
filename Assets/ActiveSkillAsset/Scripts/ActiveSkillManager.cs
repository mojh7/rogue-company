using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillManager : MonoBehaviourSingleton<ActiveSkillManager>
{
    #region public
    public bool Charm(Character attacker, Character victim, float time, float amount)
    {
        if (attacker || victim || time < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(Charm, attacker, victim, time, amount));
        return true;
    }

    public bool IncreaseStatus(Character attacker, Character victim, float time, float amount)
    {
        if (attacker || victim || time < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(IncreaseStatus, attacker, victim, time, amount));
        return true;
    }

    public bool Confuse(Character attacker, Character victim, float time, float amount)
    {
        if (attacker || victim || time < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(Confuse, attacker, victim, time, amount));
        return true;
    }

    public bool HandUp(Character attacker, float time, float amount, float radius, float num)
    {
        if (!attacker || radius < 0 || time < 0 || amount < 0 || num < 1)
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

    public bool HandClap(Character attacker, Character victim, float time, float amount)
    {
        if (attacker || victim || time < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(HandClap, attacker, victim, time, amount));
        return true;
    }

    public bool SpawnServant(Character attacker, Character victim, float time, float amount, EnemyData servantData)
    {
        if (attacker || victim || time < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(HandClap, attacker, victim, time, amount));
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

    private void HandClap(Character attacker, object obj, float amount)
    {
        Vector2 pos = attacker.transform.position;

        HandClapPart(pos - Vector2.left, attacker, amount);
        HandClapPart(pos - Vector2.right, attacker, amount);
    }

    private void HandClapPart(Vector2 pos, Character character, float amount)
    {
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = pos;
        gameObject.AddComponent<CollisionSkill>().Init(character, amount, "handClap");
    }
    #endregion
    #region coroutine
    IEnumerator CoroutineSkill(Action<Character, object, float> action, Character attacker, object parameter, float time, float amount)
    {
        float startTime = Time.deltaTime;
        float elapsedTime = 0;
        while (time < elapsedTime)
        {
            elapsedTime = Time.deltaTime - startTime;
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        action(attacker, parameter, amount);
    }
    #endregion
}
