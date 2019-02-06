using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBasicImpact : MonoBehaviour
{
    [SerializeField]
    private float lifeTime;
    [SerializeField]
    private GameObject spriteObj;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private string colorName;

    private void OnEnable()
    {
        spriteObj.SetActive(true);
        animator.SetTrigger(colorName);
        Invoke("DisableSprite", lifeTime);
    }

    private void DisableSprite()
    {
        spriteObj.SetActive(false);
    }
}
