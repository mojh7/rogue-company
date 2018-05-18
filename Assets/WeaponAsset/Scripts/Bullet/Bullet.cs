using System.Collections;
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

    private Coroutine bulletUpdate;
    private Coroutine rotationAnimation;
    private Coroutine scaleAnimation;

    private Vector3 dirVector; // 총알 방향 벡터

    private DelGetPosition ownerDirVec;
    private DelGetPosition ownerPos;
    private float addDirVecMagnitude;

    #endregion
    #region getter
    public LineRenderer GetLineRenderer() { return lineRenderer; }
    public DelGetPosition GetOwnerDirVec() { return ownerDirVec; }
    public DelGetPosition GetOwnerPos() { return ownerPos; }

    // 현재 바라보는 방향의 euler z 각도 반환
    public float GetDirDegree() {return objTransform.rotation.eulerAngles.z; }
    public Vector3 GetPosition() { return objTransform.position; }
    public float GetAddDirVecMagnitude() { return addDirVecMagnitude; }

    // 현재 바라보는 방향의 vector 반환
    public Vector3 GetDirVector() { return dirVector; }
    #endregion
    #region setter
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
        // 총알 끼리 무시, 총알 레이어 무시, 현재 임시로 Bullet layer 15번, Wall layer 14번 쓰고 있음.
        Physics2D.IgnoreLayerCollision(15, 15);
        Physics2D.IgnoreLayerCollision(15, 16);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < info.updatePropertiesLength; i++)
        {
            info.updateProperties[i].Update();
        }
    }
    #endregion
    #region Function
    // 총알 class 초기화
    // 일반 총알 초기화
    public void Init(int bulletId, float speed, float range, int effectId, Vector3 pos, float direction)
    {
        info = DataStore.Instance.GetBulletInfo(bulletId);

        // bullet 고유의 정보가 아닌 bulletPattern이나 weapon의 정보를 따라 쓰려고 할 때, 값을 덮어씀.
        if (speed != 0)
        {
            info.speed = speed;
        }
        if (range != 0)
        {
            info.range = range;
        }
        if (effectId != -1)
        {
            info.effectId = effectId;
        }

        //--------------------------------

        // sprite 애니메이션 적용
        if (info.spriteAnimation != BulletAnimationType.NotPlaySpriteAnimation)
        {
            spriteAnimatorObj.SetActive(true);
            PlaySpriteAnimation(info.spriteAnimation);
            spriteRenderer.sprite = null;
        }
        else // sprite 애니메이션 미 적용
        {
            spriteAnimatorObj.SetActive(false);
            spriteRenderer.sprite = info.bulletSprite;
        }

        if (info.showsRotationAnimation == true)
        {
            StartCoroutine("RotationAnimation");
        }

        if (info.showsScaleAnimation == true)
        {
            StartCoroutine("ScaleAnimation");
        }

        paticleObj.SetActive(info.showsParticle);


        // component on/off
        boxCollider.enabled = true;
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;

        // 튕기는 총알 테스트 용, 일단 컬라이더 임시로 박스만 쓰는 중
        if (info.bounceAble == true)
        {
            boxCollider.isTrigger = false;
        }
        else
        {
            boxCollider.isTrigger = true;
        }


        // 처음 위치 설정
        objTransform.position = pos;
        objTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        objTransform.localScale = new Vector3(info.scaleX, info.scaleY, 1f);

        // 총알 속성들 초기화
        InitProperty();

        UpdateDirection(direction);
    }

    // 레이저 총알 초기화
    // 레이저 나중에 빔 모양 말고 처음 시작 지점, raycast hit된 지점에 동그란 원 추가 생성 할 수도 있음.
    public void Init(int bulletId, float addDirVecMagnitude, DelGetPosition ownerPos, DelGetPosition ownerDirVec)
    {
        info = DataStore.Instance.GetBulletInfo(bulletId);
        // component on/off
        boxCollider.enabled = false;
        circleCollider.enabled = false;
        lineRenderer.enabled = true;

        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.cyan;
        lineRenderer.startWidth = 0.4f;
        lineRenderer.endWidth = 0.4f;
        lineRenderer.positionCount = 2;

        this.ownerPos = ownerPos;
        this.ownerDirVec = ownerDirVec;
        this.addDirVecMagnitude = addDirVecMagnitude;
        objTransform.position = ownerPos();
        InitProperty();
        bulletUpdate = StartCoroutine("BulletUpdate");
    }

    // collision, update, delete 속성 초기화
    private void InitProperty()
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

    /// <summary>
    /// 해당 vector 혹은 degree 방향으로 총알을 회전하고 속도를 설정한다.
    /// </summary>
    /// <param name="dirVector"></param>
    public void UpdateDirection(Vector3 dirVector)
    {
        this.dirVector = dirVector;
        if(info.isFixedAngle == false)
        {
            objTransform.rotation = Quaternion.Euler(0, 0, dirVector.GetDegFromVector());

        }
        objRigidbody.velocity = info.speed * dirVector;
    }
    public void UpdateDirection(float degree)
    {
        dirVector = MathCalculator.VectorRotate(Vector3.right, degree);
        if (info.isFixedAngle == false)
        {
            objTransform.rotation = Quaternion.Euler(0, 0, degree);
        }
        objRigidbody.velocity = info.speed * dirVector;
    }

    // 충돌 처리 Collision
    public void OnCollisionEnter2D(Collision2D coll)
    {
        CollisionBullet(coll);
    }

    // 충돌 처리 Trigger
    public void OnTriggerEnter2D(Collider2D coll)
    {
        CollisionBullet(coll);
    }

    // 충돌 속성 실행 Collision
    public void CollisionBullet(Collision2D coll)
    {
        if (coll.transform.CompareTag("Wall"))
        {
            //Debug.Log("Collision 벽 충돌");
            for (int i = 0; i < info.collisionPropertiesLength; i++)
            {
                info.collisionProperties[i].Collision(ref coll);
            }
        }
    }
    // 충돌 속성 실행 Trigger
    public void CollisionBullet(Collider2D coll)
    {
        if (coll.CompareTag("Wall"))
        {
            //Debug.Log("Trigger 벽 충돌");
            for (int i = 0; i < info.collisionPropertiesLength; i++)
            {
                info.collisionProperties[i].Collision(ref coll);
            }
        }
    }

    /// <summary>
    /// 삭제 속성 실행 
    /// </summary>
    public void DestroyBullet()
    {
        //Debug.Log(this + "Destroy Bullet");
        // update 코루틴 멈춤
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

        viewTransform.localRotation = Quaternion.Euler(0, 0, 0);
        viewTransform.localScale = new Vector3(1f, 1f, 1f);

        // 삭제 속성 모두 실행
        for (int i = 0; i < info.deletePropertiesLength; i++)
        {
            info.deleteProperties[i].DestroyBullet();
        }
    }

    /// <summary>
    /// BulletAniType enum과 1대1 대응해서 animation을 실행한다.
    /// </summary>
    /// <param name="type"></param>
    public void PlaySpriteAnimation(BulletAnimationType type)
    {
        switch (type)
        {
            case BulletAnimationType.BashAfterImage:
                animator.SetTrigger("bashAfterImage");
                break;
            case BulletAnimationType.PowerBullet:
                animator.SetTrigger("powerBullet");
                break;
            case BulletAnimationType.Wind:
                animator.SetTrigger("wind");
                break;
            default:
                break;
        }
    }

    #endregion



    #region coroutine
    #endregion
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
            yield return YieldInstructionCache.WaitForSeconds(0.016f);  // 일단은 약 60 fps 정도로 실행
        }
    }

    /// <summary>
    /// scale 1.0배 ~ 1.5배 왔다갔다 하는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator ScaleAnimation()
    {
        float deltaScale = 0;
        float scale = 0;
        while (true)
        {
            deltaScale += 0.2f;
            scale = 1 + Mathf.PingPong(deltaScale, 1f);
            viewTransform.localScale = new Vector3(scale, scale, 1f);
            yield return YieldInstructionCache.WaitForSeconds(0.016f);  // 일단은 약 60 fps 정도로 실행
        }
    }
}

/*
// 기본 총알
public class BaseBullet : Bullet
{

}*/
