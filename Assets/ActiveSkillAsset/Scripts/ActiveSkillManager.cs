using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillManager : MonoBehaviourSingleton<ActiveSkillManager>
{
    Vector3 upVector = Vector3.up;
    #region public
    public BT.State Charm(Character user, object victim, int idx, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        StartCoroutine(CoroutineSkill(Charm, user, victim, delay, amount));
        user.isCasting = false;
        return BT.State.SUCCESS;
    }

    public BT.State IncreaseStatus(Character user, object victim, int idx, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        StartCoroutine(CoroutineSkill(IncreaseStatus, user, victim, delay, amount));
        user.isCasting = false;
        return BT.State.SUCCESS;
    }

    public BT.State Confuse(Character user, object victim, int idx, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        StartCoroutine(CoroutineSkill(Confuse, user, victim, delay, amount));
        user.isCasting = false;
        return BT.State.SUCCESS;
    }

    public BT.State HandUp(Character user, object radius, int idx, float delay, float amount, float num)
    {
        if (!user || delay < 0 || amount < 0 || num < 1)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        for (int i = 0; i < num; i++)
        {
            float randDelay = UnityEngine.Random.Range(0, delay + 1);
            StartCoroutine(CoroutineSkill(HandUp, user, radius, randDelay, amount));
        }
        user.isCasting = false;
        return BT.State.SUCCESS;
    }

    public BT.State HandClap(Character user, object unneeded, int idx, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        StartCoroutine(CoroutineSkill(HandClap, user, user.transform.position + Vector3.left, 0, amount));
        StartCoroutine(CoroutineSkill(HandClap, user, user.transform.position + Vector3.right, delay, amount));
        user.isCasting = false;
        return BT.State.SUCCESS;
    }

    public BT.State SpawnServant(Character user, object servantData, int idx, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        StartCoroutine(CoroutineSkill(HandClap, user, servantData, delay, amount));
        user.isCasting = false;
        return BT.State.SUCCESS;
    }

    public BT.State RangeAttack(Character user, object radius, int idx, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = user.transform.position;
        gameObject.AddComponent<CollisionSkill>().Init(user as Character, amount, (float)radius);
        gameObject.GetComponent<CollisionSkill>().SetAvailableFalse();
        user.GetCharacterComponents().AnimationHandler.SetLapsedAction(gameObject.GetComponent<CollisionSkill>().SetAvailableTrue);
        user.GetCharacterComponents().AnimationHandler.SetEndAction(gameObject.GetComponent<CollisionSkill>().EndAnimation);
        user.GetCharacterComponents().AnimationHandler.Skill(idx);
        return BT.State.SUCCESS;
    }

    public BT.State Flash(Character user, object position, int idx, float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = user.transform.position;
        gameObject.AddComponent<CollisionSkill>().Init(user as Character, position, amount, Flash);

        user.GetCharacterComponents().AnimationHandler.SetLapsedAction(gameObject.GetComponent<CollisionSkill>().LapseAnimation);
        user.GetCharacterComponents().AnimationHandler.SetEndAction(gameObject.GetComponent<CollisionSkill>().EndAnimation);
        user.GetCharacterComponents().AnimationHandler.Skill(idx);
        return BT.State.SUCCESS;
    }

    public BT.State Jump(Character user, object victim, int idx,float delay, float amount)
    {
        if (!user || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        user.isCasting = true;
        StartCoroutine(CoroutineSkill(Jump, user, victim, delay, amount));
        return BT.State.SUCCESS;
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
        if (!user)
            return;
        float rad = (float)radius;
        Vector2 randPos = UnityEngine.Random.insideUnitCircle * rad;
        GameObject gameObject = ResourceManager.Instance.objectPool.GetPooledObject();
        gameObject.transform.position = new Vector2(user.transform.position.x + randPos.x, user.transform.position.y + randPos.y);
        gameObject.AddComponent<Alert>();
        gameObject.GetComponent<Alert>().Init(HandUpPart, user, amount, 1);
        gameObject.GetComponent<Alert>().Active();
    }

    private void HandUpPart(Vector3 pos, object user, float amount)
    {
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = pos;
        gameObject.AddComponent<CollisionSkill>().Init(user as Character, amount, "handUp");
    }

    private void HandClap(Character user, object unneeded, float amount)
    {
        Vector3 pos = (Vector3)unneeded;

        HandClapPart(pos, user, amount);
    }

    private void HandClapPart(Vector3 pos, Character user, float amount)
    {
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = pos;
        gameObject.AddComponent<CollisionSkill>().Init(user, amount, "handClap");
    }

    private void Flash(Character user, object position, float amount)
    {
        user.transform.position = (Vector2)position;
    }

    private void Jump(Character user, object victim, float amount)
    {
        Vector2 targetPos = ((victim as Character).transform).position + upVector;
        if (user)
        {
            user.GetCharacterComponents().AIController.StopMove();
            user.GetCharacterComponents().CircleCollider2D.enabled = false;
            StartCoroutine(CoroutineJump(user, user.transform.position, targetPos));
        }
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
    IEnumerator CoroutineJump(Character user, Vector3 src, Vector3 dest)
    {
        Transform userTransform = user.transform;
        user.GetCharacterComponents().SpriteRenderer.sortingLayerName = "Effect";
        Vector3 shadowScaleSrc = user.GetCharacterComponents().ShadowTransform.localScale;
        Vector3 shadowScaleDest = shadowScaleSrc * 0.5f;
        Vector3 shadowSrc = user.GetCharacterComponents().ShadowTransform.localPosition;
        Vector3 shadowDest = user.GetCharacterComponents().ShadowTransform.localPosition - upVector;
        float startTime = Time.time;
        float elapsedTime = 0;
        float durationOfFlight = 0.5f;
        while (elapsedTime < durationOfFlight)
        {
            elapsedTime = Time.time - startTime;

            userTransform.position = Vector2.Lerp(src, dest, elapsedTime / durationOfFlight);
            user.GetCharacterComponents().ShadowTransform.localPosition = Vector2.Lerp(shadowSrc, shadowDest, elapsedTime / durationOfFlight);
            user.GetCharacterComponents().ShadowTransform.localScale = Vector2.Lerp(shadowScaleSrc, shadowScaleDest, elapsedTime / durationOfFlight);
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }
        yield return YieldInstructionCache.WaitForSeconds(0.1f); // delay Flight state;
        startTime = Time.time;
        elapsedTime = 0;
        durationOfFlight = 0.1f;
        Vector2 newDest = dest - upVector;
        while (elapsedTime < durationOfFlight)
        {
            elapsedTime = Time.time - startTime;

            userTransform.position = Vector2.Lerp(dest, newDest, elapsedTime / durationOfFlight);
            user.GetCharacterComponents().ShadowTransform.localPosition = Vector2.Lerp(shadowDest, shadowSrc, elapsedTime / durationOfFlight);
            user.GetCharacterComponents().ShadowTransform.localScale = Vector2.Lerp(shadowScaleDest, shadowScaleSrc, elapsedTime / durationOfFlight);
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }
        user.GetCharacterComponents().SpriteRenderer.sortingLayerName = "Default";
        if (user)
        {
            user.GetCharacterComponents().CircleCollider2D.enabled = true;
            user.GetCharacterComponents().AIController.PlayMove();
            user.GetComponent<Character>().isCasting = false;
        }
    }
    #endregion
}
