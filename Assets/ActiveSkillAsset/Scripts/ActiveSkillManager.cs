using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillManager : MonoBehaviourSingleton<ActiveSkillManager>
{
    public static Vector3 nullVector = new Vector3(-1000, -1000, -1000);
    Vector3 upVector = Vector3.up;
    #region public
    public void StartJumpCoroutine(Character caster, Vector3 src, Vector3 dest)
    {
        StartCoroutine(CoroutineJump(caster, caster.GetPosition(), dest + upVector));
    }
    public void StartSequence(
        Vector3 mPos,
      Character caster, Character other, CustomObject customObject,
      SkillData preSkillData, SkillData mainSkillData, SkillData postSkillData,
        int frontAnimIdx, int backAnimIdx,
          float frontAnimTime, float backAnimTime)
    {
        StartCoroutine(CoroutineSequence(mPos, 
            caster, other, customObject, 
            preSkillData, mainSkillData, postSkillData, 
            frontAnimIdx, backAnimIdx, frontAnimTime, backAnimTime));
    }
    public void DelaySkill(
    Vector3 mPos, SkillData skillData,
  Character caster, Character other, CustomObject customObject,float delay)
    {
        StartCoroutine(CoroutineDelay(mPos, skillData, caster, other, customObject, delay));
    }
    #endregion
    #region coroutine
    IEnumerator CoroutineJump(Character caster, Vector3 src, Vector3 dest)
    {
        Transform userTransform = caster.transform;
        caster.GetCharacterComponents().SpriteRenderer.sortingLayerName = "Effect";
        Vector3 shadowScaleSrc = caster.GetCharacterComponents().ShadowTransform.localScale;
        Vector3 shadowScaleDest = shadowScaleSrc * 0.5f;
        Vector3 shadowSrc = caster.GetCharacterComponents().ShadowTransform.localPosition;
        Vector3 shadowDest = caster.GetCharacterComponents().ShadowTransform.localPosition - upVector;
        float startTime = Time.time;
        float elapsedTime = 0;
        float durationOfFlight = 0.5f;
        while (elapsedTime < durationOfFlight)
        {
            elapsedTime = Time.time - startTime;

            userTransform.position = Vector2.Lerp(src, dest, elapsedTime / durationOfFlight);
            caster.GetCharacterComponents().ShadowTransform.localPosition = Vector2.Lerp(shadowSrc, shadowDest, elapsedTime / durationOfFlight);
            caster.GetCharacterComponents().ShadowTransform.localScale = Vector2.Lerp(shadowScaleSrc, shadowScaleDest, elapsedTime / durationOfFlight);
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
            caster.GetCharacterComponents().ShadowTransform.localPosition = Vector2.Lerp(shadowDest, shadowSrc, elapsedTime / durationOfFlight);
            caster.GetCharacterComponents().ShadowTransform.localScale = Vector2.Lerp(shadowScaleDest, shadowScaleSrc, elapsedTime / durationOfFlight);
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }
        caster.GetCharacterComponents().SpriteRenderer.sortingLayerName = "Default";
        if (caster)
        {
            caster.GetCharacterComponents().CircleCollider2D.enabled = true;
            caster.GetCharacterComponents().AIController.PlayMove();
            caster.GetComponent<Character>().isCasting = false;
        }
    }
    IEnumerator CoroutineSequence(
        Vector3 mPos,
      Character caster, Character other, CustomObject customObject,
      SkillData preSkillData, SkillData mainSkillData, SkillData postSkillData,
        int frontAnimIdx, int backAnimIdx,
          float frontAnimTime, float backAnimTime)
    {
        caster.isCasting = true;
        if (preSkillData)
        {
            float lapsedTime = 9999;
            if (other)
            {
                preSkillData.Run(caster, other, mPos, ref lapsedTime);
            }
            else if (customObject)
            {
                preSkillData.Run(customObject, mPos, ref lapsedTime);
            }
            else
            {
                preSkillData.Run(caster, mPos, ref lapsedTime);
            }
        }

        if (frontAnimIdx > -1)
        {
            caster.GetCharacterComponents().AnimationHandler.Skill(frontAnimIdx);
            yield return YieldInstructionCache.WaitForSeconds(frontAnimTime + 0.1F);
            caster.GetCharacterComponents().AnimationHandler.Skill(-1);
        }
        if (mainSkillData)
        {
            float lapsedTime = 9999;

            if (other)
            {
                mainSkillData.Run(caster, other, mPos, ref lapsedTime);
            }
            else if (customObject)
            {
                mainSkillData.Run(customObject, mPos, ref lapsedTime);
            }
            else
            {
                mainSkillData.Run(caster, mPos, ref lapsedTime);
            }
        }
        if (backAnimIdx > -1)
        {
            caster.GetCharacterComponents().AnimationHandler.Skill(backAnimIdx);
            yield return YieldInstructionCache.WaitForSeconds(backAnimTime + 0.1F);
            caster.GetCharacterComponents().AnimationHandler.Skill(-1);
        }
        if (postSkillData)
        {
            float lapsedTime = 9999;

            if (other)
            {
                postSkillData.Run(caster, other, mPos, ref lapsedTime);
            }
            else if (customObject)
            {
                postSkillData.Run(customObject, mPos, ref lapsedTime);
            }
            else
            {
                postSkillData.Run(caster, mPos, ref lapsedTime);
            }
        }
        caster.isCasting = false;
    }
    IEnumerator CoroutineDelay(
     Vector3 mPos, SkillData skillData,
   Character caster, Character other, CustomObject customObject,float delay)
    {
        yield return YieldInstructionCache.WaitForSeconds(delay);
        if (skillData)
        {
            float lapsedTime = 9999;

            if (other)
            {
                skillData.Run(caster, other, mPos,ref lapsedTime);
            }
            else if (customObject)
            {
                skillData.Run(customObject, mPos,ref lapsedTime);
            }
            else
            {
                skillData.Run(caster, mPos,ref lapsedTime);
            }
        }
    }
    #endregion
}   
