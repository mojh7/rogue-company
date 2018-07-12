using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterComponets : MonoBehaviour {
    #region components
    [SerializeField]
    private WeaponManager weaponManager;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Transform spriteTransform;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private CircleCollider2D interactiveCollider2D;
    [SerializeField]
    private AnimationHandler animationHandler;
    private BuffManager buffManager;
    private Rigidbody2D rgbody;
    #endregion
    #region parameter
    public WeaponManager WeaponManager
    {
        get
        {
            return weaponManager;
        }
    }
    public SpriteRenderer SpriteRenderer
    {
        get
        {
            return spriteRenderer;
        }
    }
    public Transform SpriteTransform
    {
        get
        {
            return spriteTransform;
        }
    }
    public Animator Animator
    {
        get
        {
            return animator;
        }
    }
    public CircleCollider2D InteractiveCollider2D
    {
        get
        {
            return interactiveCollider2D;
        }
    }
    public AnimationHandler AnimationHandler
    {
        get
        {
            return animationHandler;
        }
    }
    public BuffManager BuffManager
    {
        get
        {
            return buffManager;
        }
    }
    public Rigidbody2D Rigidbody2D
    {
        get
        {
            return rgbody;
        }
    }
    #endregion
    #region UnityFunc
    private void Start()
    {
        buffManager = GetComponent<BuffManager>();
        rgbody = GetComponent<Rigidbody2D>();
    }
    #endregion
    
}
