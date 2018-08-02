using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillManager : MonoBehaviourSingleton<ActiveSkillManager>
{
    #region public
    public bool Charm(Character user, object victim, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(Charm, user, victim, delay, amount));
        return true;
    }

    public bool IncreaseStatus(Character user, object victim, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(IncreaseStatus, user, victim, delay, amount));
        return true;
    }

    public bool Confuse(Character user, object victim, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(Confuse, user, victim, delay, amount));
        return true;
    }

    public bool HandUp(Character user, object radius, float delay, float amount, float num)
    {
        if (!user || delay < 0 || amount < 0 || num < 1)
        {
            return false;
        }
        for (int i = 0; i < num; i++)
        {
            float randDelay = UnityEngine.Random.Range(0, delay + 1);
            StartCoroutine(CoroutineSkill(HandUp, user, radius, randDelay, amount));
        }
        return true;
    }

    public bool HandClap(Character user, object unneeded, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return false;
        }

        StartCoroutine(CoroutineSkill(HandClap, user, user.transform.position + Vector3.left, 0, amount));
        StartCoroutine(CoroutineSkill(HandClap, user, user.transform.position + Vector3.right, delay, amount));
        return true;
    }

    public bool SpawnServant(Character user, object servantData, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return false;
        }
        StartCoroutine(CoroutineSkill(HandClap, user, servantData, delay, amount));
        return true;
    }
    #endregion
    #region private
    private void Charm(Character user, object victim, float amount)
    {

    }

    private void IncreaseStatus(Character user, object victim, float amount)
    {

    }

    private void Confuse(Character user, object victim, float amount)
    {

    }

    private void HandUp(Character user, object radius, float amount)
    {
        float rad = (float)radius;
        Vector2 randPos = UnityEngine.Random.insideUnitCircle * rad;
        GameObject gameObject = ResourceManager.Instance.objectPool.GetPooledObject();
        gameObject.transform.position = new Vector2(user.transform.position.x + randPos.x, user.transform.position.y + randPos.y);
        gameObject.AddComponent<Alert>();
        gameObject.GetComponent<Alert>().Init(HandUpPart, user, amount, 1);
        gameObject.GetComponent<Alert>().Active();
    }

    private void HandUpPart(Vector3 pos, object character, float amount)
    {
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = pos;
        gameObject.AddComponent<CollisionSkill>().Init(character as Character, amount, "handUp");
    }

    private void HandClap(Character user, object unneeded, float amount)
    {
        Vector3 pos = (Vector3)unneeded;

        HandClapPart(pos, user, amount);
    }

    private void HandClapPart(Vector3 pos, Character character, float amount)
    {
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = pos;
        gameObject.AddComponent<CollisionSkill>().Init(character, amount, "handClap");
    }
    #endregion
    #region coroutine
    IEnumerator CoroutineSkill(Action<Character, object, float> action, Character user, object parameter, float delay, float amount)
    {
        float startTime = Time.time;
        float elapsedTime = 0;
        while (delay >= elapsedTime)
        {
            elapsedTime = Time.time - startTime;
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        action(user, parameter, amount);
    }
    #endregion
}
