﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;


/// <summary>
/// Bullet Class
/// 
/// bullet 하나에 몰아서 만들고 if문으로 총알을 나눌 수도 있고 아니면 나중에 총알 종류마다 클래스 나누어서
/// bullet 상속받고 나뉠수도 있음, 일반 총알, 레이저 총알 따로 만드는 식으로
/// </summary>

public class Bullet : MonoBehaviour
{
    #region variables
    public BulletInfo info;
    public Transform objTransform;
    public BoxCollider2D boxCollider;
    public CircleCollider2D circleCollider;
    public Rigidbody2D objRigidbody;
    [SerializeField]
    // 레이저용 lineRenderer
    private LineRenderer lineRenderer;

    // spirte, 애니메이션 용 sprite 포함 object
    [SerializeField]
    private Transform viewTransform;
    [SerializeField]
    private GameObject spriteAnimatorObj;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField]
    private GameObject paticleObj;


    [SerializeField] private GameObject laserViewObj;
    [SerializeField] private Transform laserStartPoint;
    [SerializeField] private Animator laserStartPointAnimator;
    [SerializeField] private Transform laserEndPoint;
    [SerializeField] private Animator laserEndPointAnimator;


    private Coroutine bulletUpdate;
    private Coroutine rotationAnimation;
    private Coroutine scaleAnimation;
    private Coroutine deleteOnlifeTime;
        

    private Vector3 dirVector; // 총알 방향 벡터
    private float dirDegree;      // 총알 방향 각도.

    private OwnerType ownerType;
    private DelGetPosition ownerDirVec;
    private DelGetPosition ownerPos;
    private float addDirVecMagnitude;

    // 코루틴 deltaTime
    private float coroutineDeltaTime = 0.016f;
    #endregion

    #region getter / setter
    public Transform LaserStartPoint { get { return laserStartPoint; } set { laserStartPoint = value; } }
    public Transform LaserEndPoint { get { return laserEndPoint; } set { laserEndPoint = value; } }

    public LineRenderer GetLineRenderer() { return lineRenderer; }
    public OwnerType GetOwnerType() { return ownerType; }
    public DelGetPosition GetOwnerDirVec() { return ownerDirVec; }
    public DelGetPosition GetOwnerPos() { return ownerPos; }

    // 현재 바라보는 방향의 euler z 각도 반환
    public Vector3 GetPosition() { return objTransform.position; }
    public float GetAddDirVecMagnitude() { return addDirVecMagnitude; }

    public float GetDirDegree() { return dirDegree; }
    // 현재 바라보는 방향의 vector 반환
    public Vector3 GetDirVector() { return dirVector; }
    #endregion
    #region unityFunction
    void Awake()
    {
        //gameObject.hideFlags = HideFlags.HideInHierarchy;
        objTransform = GetComponent<Transform>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        objRigidbody = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        animator = GetComponentInChildren<Animator>();
        // 총알 끼리 무시, 총알 레이어 무시, 현재 임시로 Enemy 13, Wall 14, Bullet 15, Player 16번 쓰고 있음.
        // Physics2D.IgnoreLayerCollision(15, 15);
        // edit -> project settings -> physics2D 에서 레이어별 충돌 무시 설정 가능, 거기서 일단 설정했음
        // PlayerBullet, Player 충돌 무시
        // EnemyBullet, Enemy 충돌 무시
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < info.updatePropertiesLength; i++)
        {
            info.updateProperties[i].Update();
        }
    }
    #endregion
    #region function
    // 총알 class 초기화

    // 일반(투사체) 총알 초기화 - position이랑 direction만 받음
    public void Init(int bulletId, OwnerType ownerType, Vector3 pos, float direction = 0)
    {
        info = DataStore.Instance.GetBulletInfo(bulletId, ownerType);

        // 투사체 총알 속성 초기화
        InitProjectileProperty();

        // Owner 정보 초기화
        InitOwnerInfo(ownerType);

        // 총알 속성들 초기화
        InitPropertyClass();

        // 처음 위치 설정
        objTransform.position = pos;
        objTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        objTransform.localScale = new Vector3(info.scaleX, info.scaleY, 1f);
        dirDegree = 0f;
        dirVector = Vector3.right;
        SetDirection(direction);
    }

    // 일반(투사체) 총알 초기화
    public void Init(int bulletId, OwnerType ownerType, Vector3 pos, float direction, float speed, float range, float damage, float knockBack, float criticalRate)
    {
        info = DataStore.Instance.GetBulletInfo(bulletId, ownerType);

        // bullet 고유의 정보가 아닌 bulletPattern이나 weapon의 정보를 따라 쓰려고 할 때, 값을 덮어씀.
        if (speed != 0)
        {
            info.speed = speed;
        }
        if (range != 0)
        {
            info.range = range;
        }
        if (damage != 0)
        {
            info.damage = damage;
        }
        if (knockBack != 0)
        {
            info.knockBack = knockBack;
        }
        if (criticalRate != 0)
        {
            info.criticalRate = criticalRate;
        }
        //--------------------------------


        // 투사체 총알 속성 초기화
        InitProjectileProperty();

        // Owner 정보 초기화
        InitOwnerInfo(ownerType);

        // 총알 속성들 초기화
        InitPropertyClass();


        // 처음 위치 설정
        objTransform.position = pos;
        objTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        objTransform.localScale = new Vector3(info.scaleX, info.scaleY, 1f);
        dirDegree = 0f;
        dirVector = Vector3.right;
        SetDirection(direction);
    }

    // 레이저 총알 초기화
    // 레이저 나중에 빔 모양 말고 처음 시작 지점, raycast hit된 지점에 동그란 원 추가 생성 할 수도 있음.
    public void Init(int bulletId, OwnerType ownerType , float addDirVecMagnitude, DelGetPosition ownerPos, DelGetPosition ownerDirVec, float damage, float knockBack, float criticalRate)
    {
        info = DataStore.Instance.GetBulletInfo(bulletId, ownerType);

        // bullet 고유의 정보가 아닌 bulletPattern이나 weapon의 정보를 따라 쓰려고 할 때, 값을 덮어씀.
        if (damage != 0)
        {
            info.damage = damage;
        }
        if (knockBack != 0)
        {
            info.knockBack = knockBack;
        }
        if (criticalRate != 0)
        {
            info.criticalRate = criticalRate;
        }
        //--------------------------------


        // component on/off
        boxCollider.enabled = false;
        circleCollider.enabled = false;
        lineRenderer.enabled = true;

        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.cyan;
        lineRenderer.startWidth = 0.4f;
        lineRenderer.endWidth = 0.4f;
        lineRenderer.SetPosition(0, Vector2.zero);
        lineRenderer.SetPosition(1, Vector2.zero);

        // Owner 정보 초기화
        InitOwnerInfo(ownerType);

        spriteAnimatorObj.SetActive(false);
        spriteRenderer.sprite = null;
        laserViewObj.SetActive(true);

        Debug.Log(info.damage);
        // 0529 레이저 임시, 파란색 레이저
        laserStartPointAnimator.SetTrigger("BlueLaser");
        laserEndPointAnimator.SetTrigger("BlueLaser");
        objTransform.localScale = new Vector3(info.scaleX, info.scaleY, 1f);
        this.ownerPos = ownerPos;
        this.ownerDirVec = ownerDirVec;
        this.addDirVecMagnitude = addDirVecMagnitude;
        objTransform.position = ownerPos();
        InitPropertyClass();
        //bulletUpdate = StartCoroutine("BulletUpdate");
    }

    private void InitOwnerInfo(OwnerType ownerType)
    {
        this.ownerType = ownerType;
        // Enemy 13, Wall 14, Bullet 15, Player 16번
        switch (ownerType)
        {
            case OwnerType.Enemy:
                gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
                break;
            case OwnerType.Player:
                gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Projectile 총알 collision, update, delete 속성 Class이외의
    /// 기타 bullet 고유 속성들 초기화 및 적용
    /// </summary>
    private void InitProjectileProperty()
    {
        // sprite 애니메이션 적용
        if (BulletAnimationType.NotPlaySpriteAnimation 
            != info.spriteAnimation)
        {
            spriteAnimatorObj.SetActive(true);
            animator.SetTrigger(info.spriteAnimation.ToString());
            spriteRenderer.sprite = null;
        }
        // sprite 애니메이션 미 적용
        else
        {
            spriteAnimatorObj.SetActive(false);
            spriteRenderer.sprite = info.bulletSprite;
        }

        // rotate 360도 계속 회전하는 애니메이션 적용
        if (info.showsRotationAnimation == true)
        {
            rotationAnimation = StartCoroutine("RotationAnimation");
        }

        // scale이 바뀌면서 커지고 작아지는 애니메이션 적용
        if (info.showsScaleAnimation == true)
        {
            scaleAnimation = StartCoroutine("ScaleAnimation");
        }

        // lifeTime이 0 초과되는 값을 가지면 시간이 lifeTime이 지나면 delete 속성 실행
        if (info.lifeTime > 0)
        {
            deleteOnlifeTime = StartCoroutine("DeleteOnLifeTime");
        }

        if (info.soundId >= 0)
        {
            AudioManager.Instance.PlaySound(info.soundId);
        }

        // 파티클이 포함되어있는 오브젝트 on/ off
        paticleObj.SetActive(info.showsParticle);

        laserViewObj.SetActive(false);

        // component on/off
        boxCollider.enabled = true;
        lineRenderer.enabled = false;
        //lineRenderer.positionCount = 0;

        // 튕기는 총알 테스트 용, 일단 컬라이더 임시로 박스만 쓰는 중
        if (info.bounceAble == true)
        {
            boxCollider.isTrigger = false;
        }
        else
        {
            boxCollider.isTrigger = true;
        }
    }

    /// <summary> collision, update, delete 속성 Class들 초기화  </summary>
    private void InitPropertyClass()
    {
        // 총알 충돌 속성 초기화
        for (int i = 0; i < info.collisionPropertiesLength; i++)
        {
            info.collisionProperties[i].Init(this);
        }
        // 총알 이동 속성 초기화
        for (int i = 0; i < info.updatePropertiesLength; i++)
        {
            info.updateProperties[i].Init(this);
        }
        // 총알 삭제 속성 초기화
        for (int i = 0; i < info.deletePropertiesLength; i++)
        {
            info.deleteProperties[i].Init(this);
        }
    }

    /// <summary> 해당 Vector 방향으로 총알을 회전하고 속도를 설정한다. </summary>
    public void SetDirection(Vector3 dirVector)
    {
        this.dirVector = dirVector;
        this.dirDegree = dirVector.GetDegFromVector();
        if (info.isFixedAngle == false)
        {
            objTransform.rotation = Quaternion.Euler(0, 0, this.dirDegree);
        }
        objRigidbody.velocity = info.speed * dirVector;
    }

    /// <summary> 해당 각도로 rotation.z 값을 설정하고 속도를 지정한다. </summary>
    public void SetDirection(float dirDegree)
    {
        this.dirDegree = dirDegree;
        dirVector = MathCalculator.VectorRotate(dirVector, dirDegree);
        if (info.isFixedAngle == false)
        {
            objTransform.rotation = Quaternion.Euler(0, 0, this.dirDegree);
        }
        objRigidbody.velocity = info.speed * dirVector;
    }

    /// <summary> 현재 방향의 각도와 방향벡터에서 매개변수로 받은 각도만큼 회전 및 속도를 지정한다. </summary>
    public void RotateDirection(float dirDegree)
    {
        this.dirDegree += dirDegree;
        dirVector = MathCalculator.VectorRotate(dirVector, dirDegree);
        if (info.isFixedAngle == false)
        {
            objTransform.rotation = Quaternion.Euler(0, 0, this.dirDegree);
        }
        objRigidbody.velocity = info.speed * dirVector;
    }

    // velocity 바꾸는 함수는 계속 구조 개선 및 수정될 예정. 

    /// <summary> 속력만 바뀌는데 속력 0미만 되면 방향 원래 벡터의 반대 방향으로 바꿈 </summary>
    public void SetVelocity(float speed)
    {
        if(speed >= 0)
        {
            info.speed = speed;
            RotateDirection(0);
        }
        else
        {
            info.speed = -speed;
        }
    }

    /// <summary> 충돌 처리 Collision </summary>
    public void OnCollisionEnter2D(Collision2D coll)
    {
        CollisionBullet(coll);
    }

    /// <summary> 충돌 처리 Trigger </summary>
    public void OnTriggerEnter2D(Collider2D coll)
    {
        CollisionBullet(coll);
    }

    /// <summary> 충돌 속성 실행 Collision </summary>
    public void CollisionBullet(Collision2D coll)
    {
        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("Enemy") || coll.transform.CompareTag("Wall"))
        {
            //Debug.Log("Collision 벽 충돌");
            for (int i = 0; i < info.collisionPropertiesLength; i++)
            {
                info.collisionProperties[i].Collision(ref coll);
            }
        }
    }
    
    /// <summary> 충돌 속성 실행 Trigger </summary>
    public void CollisionBullet(Collider2D coll)
    {
        if (coll.CompareTag("Player") || coll.CompareTag("Enemy") || coll.CompareTag("Wall"))
        {
            //Debug.Log("Trigger 벽 충돌");
            for (int i = 0; i < info.collisionPropertiesLength; i++)
            {
                info.collisionProperties[i].Collision(ref coll);
            }
        }
    }

    /// <summary> 삭제 속성 실행 </summary>
    public void DestroyBullet()
    {
        // 실행 중인 코루틴이 있으면 코루틴 멈춤
        if(bulletUpdate != null)
        {
            StopCoroutine(bulletUpdate);
        }
        if (rotationAnimation != null)
        {
            StopCoroutine(rotationAnimation);
        }
        if (scaleAnimation != null)
        {
            StopCoroutine(scaleAnimation);
        }
        if (deleteOnlifeTime != null)
        {
            StopCoroutine(deleteOnlifeTime);
        }

        viewTransform.localRotation = Quaternion.Euler(0, 0, 0);
        viewTransform.localScale = new Vector3(1f, 1f, 1f);

        // 삭제 속성 모두 실행
        for (int i = 0; i < info.deletePropertiesLength; i++)
        {
            info.deleteProperties[i].DestroyBullet();
        }
    }

    #endregion



    #region coroutine

    // 안쓸 듯
    // 총알 Update 코루틴
    private IEnumerator BulletUpdate()
    {
        while (true)
        {
            // 총알 update 속성 실행
            for (int i = 0; i < info.updatePropertiesLength; i++)
            {
                info.updateProperties[i].Update();
            }
            yield return YieldInstructionCache.WaitForSeconds(0.016f);  // 일단은 약 60 fps 정도로 실행
        }
    }


    /// <summary>
    /// rotation Z 360도 회전하는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator RotationAnimation()
    {
        float eulerAngleZ = 0f;
        while (true)
        {
            eulerAngleZ += 12f;
            viewTransform.localRotation = Quaternion.Euler(0f, 0f, eulerAngleZ);
            yield return YieldInstructionCache.WaitForSeconds(coroutineDeltaTime);  // 일단은 약 60 fps 정도로 실행
        }
    }

    /// <summary> scale 1.0 ~ 2.0 커졌다 작아졌다 하는 코루틴 </summary>
    private IEnumerator ScaleAnimation()
    {
        float deltaScale = 0;
        float scale = 0;
        float sign = 1;
        while (true)
        {
            deltaScale += 0.05f * sign;
            scale = 1 + deltaScale;
            viewTransform.localScale = new Vector3(scale, scale, 1f);
            if (scale <= 1.0f)
            {
                sign = 1;
            }
            else if (scale >= 2.0f)
            {
                sign = -1;
            }
            yield return YieldInstructionCache.WaitForSeconds(coroutineDeltaTime);  // 일단은 약 60 fps 정도로 실행
        }
    }


    // invoke로 하면 메모리풀에서 on / off시 남아있어서 오류가 생겨서 코루틴으로 바꿈

    /// <summary> 시간이 lifeTime 되면 삭제 </summary>
    private IEnumerator DeleteOnLifeTime()
    {
        float time = 0;
        while(true)
        {
            if(time >= info.lifeTime)
            {
                DestroyBullet();
            }
            time += coroutineDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(coroutineDeltaTime);
        }
    }
    #endregion

}

/*
// 기본 총알
public class BaseBullet : Bullet
{

}*/
