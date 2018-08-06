using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffect : MonoBehaviour
{
    [SerializeField]
    private float lifeTime;
    [SerializeField]
    private GameObject spriteObj;

    private void OnEnable()
    {
        spriteObj.SetActive(true);
        Invoke("DisableSprite", lifeTime);
    }

    private void DisableSprite()
    {
        spriteObj.SetActive(false);
    }
}
