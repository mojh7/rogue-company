using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class UtilityClass
{
    public static void ColorLerp(this MonoBehaviour me, UnityEngine.UI.Image spriteRenderer, Color src, Color dest, float time)
    {
        me.StartCoroutine(CoroutineLerp(spriteRenderer, src, dest, time));
    }
    private static IEnumerator CoroutineLerp(UnityEngine.UI.Image spriteRenderer, Color src, Color dest, float time)
    {
        float startTime = Time.time;
        float tempElapsedTime = 0;
        while (time >= tempElapsedTime)
        {
            tempElapsedTime = Time.time - startTime;

            spriteRenderer.color = Color.Lerp(src, dest, tempElapsedTime / time);

            yield return YieldInstructionCache.WaitForEndOfFrame;
        }

    }
    /// <summary>
    /// 레이어 체크
    /// </summary>
    /// <param name="layer">체크를 위한 레이어 Integer</param>
    /// <param name="layers">체크를 위한 레이어 그룹 Integers</param>
    /// <returns>체크 여부</returns>
    public static bool CheckLayer(int layer, params int[] layers)
    {
        LayerMask mainLayer = 1 << layer;
        LayerMask comparedLayer = 0;

        for(int i=0;i<layers.Length;i++)
        {
            comparedLayer |= (1 << layers[i]);
        }

        if((mainLayer & comparedLayer) == mainLayer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool CheckLayer(int layer, LayerMask comparedLayer)
    {
        LayerMask mainLayer = 1 << layer;

        if ((mainLayer & comparedLayer) == mainLayer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 확률 반환
    /// </summary>
    /// <param name="percent">퍼센트 1~100</param>
    /// <returns>참과 거짓</returns>
    public static bool CoinFlip(int percent)
    {
        return UnityEngine.Random.Range(0, 100) < percent;
    }
    /// <summary>
    /// 매개변수의 적 레이어 반환
    /// </summary>
    public static LayerMask GetEnemyLayer(Character me)
    {
        LayerMask enemyLayer = 1;

        if(me == null)
        {
            enemyLayer = 1 << 13;
            enemyLayer |= 1 << 16;

            return enemyLayer;
        }
        switch (me.GetOwnerType())
        {
            case CharacterInfo.OwnerType.Player:
            case CharacterInfo.OwnerType.Pet:
                enemyLayer = 1 << 13;
                break;
            case CharacterInfo.OwnerType.Enemy:
            case CharacterInfo.OwnerType.Object:
                enemyLayer = 1 << 16;
                break;
            default:
                break;
        }
        return enemyLayer;
    }

    public static LayerMask GetEnemyBulletLayer(Character me)
    {
        LayerMask enemyBulletLayer = 1;

        switch (me.GetOwnerType())
        {
            case CharacterInfo.OwnerType.Player:
            case CharacterInfo.OwnerType.Pet:
                enemyBulletLayer = enemyBulletLayer << 17;
                enemyBulletLayer |= (1 << 20);
                enemyBulletLayer |= (1 << 21);
                break;
            case CharacterInfo.OwnerType.Enemy:
            case CharacterInfo.OwnerType.Object:
                enemyBulletLayer = enemyBulletLayer << 15;
                enemyBulletLayer |= (1 << 18);
                enemyBulletLayer |= (1 << 19);
                break;
            default:
                break;
        }
        return enemyBulletLayer;
    }

    public static CharacterInfo.OwnerType GetOnwerTypeLayer(Character me)
    {
        switch (me.GetOwnerType())
        {
            case CharacterInfo.OwnerType.Player:
            case CharacterInfo.OwnerType.Pet:
                return CharacterInfo.OwnerType.Player;
            case CharacterInfo.OwnerType.Enemy:
            case CharacterInfo.OwnerType.Object:
                return CharacterInfo.OwnerType.Enemy;
            default:
                return CharacterInfo.OwnerType.Player;
        }
    }
    public static void Invoke(this MonoBehaviour me, Action theDelegate, float time)
    {
        me.StartCoroutine(ExecuteAfterTime(theDelegate, time));
    }

    private static IEnumerator ExecuteAfterTime(Action theDelegate, float delay)
    {
        yield return new WaitForSeconds(delay);
        theDelegate();
    }
}
