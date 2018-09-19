using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillManager : MonoBehaviourSingleton<ActiveSkillManager>
{
    Vector3 upVector = Vector3.up;
    #region public
    public void StartCoroutine(Action<Character, object, float> action, Character user, object parameter, float delay, float amount)
    {
        StartCoroutine(CoroutineSkill(action, user, parameter, delay, amount));
    }

    public void StartJumpCoroutine(Character user, Vector3 src, Vector3 dest)
    {
        StartCoroutine(CoroutineJump(user, user.transform.position, dest + upVector));
    }

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
