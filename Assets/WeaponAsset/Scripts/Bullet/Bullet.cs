using System.Collections;
using UnityEngine;
using WeaponAsset;
using CharacterInfo;

/// <summary>
/// Bullet Class
/// 
/// bullet 하나에 몰아서 만들고 if문으로 총알을 나눌 수도 있고 아니면 나중에 총알 종류마다 클래스 나누어서
/// bullet 상속받고 나뉠수도 있음, 일반 총알, 레이저 총알 따로 만드는 식으로
/// </summary>
public class Bullet : MonoBehaviour
{
    #region constants
    public const float LASER_COLLISION_PERIOD = 0.1f;
    public const float CRITICAL_DAMAGE = 1.5f;
    #endregion

    #region variables
    public enum DeletedCondition { ALIVE, TIME_LIMIT, COLLISION_OBJECT, COLLISION_TARGET }

    public BulletInfo info;
    public Transform objTransform;
    public BoxCollider2D boxCollider;
    public CircleCollider2D circleCollider;
    public PolygonCollider2D polygonCollider;
    public BoxCollider2D objectOnlyCollider;
    [SerializeField]
    private GameObject colliderObj;
    public Rigidbody2D objRigidbody;

    // 레이저용 lineRenderer
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private TrailRenderer trailRenderer;

    // spirte, 애니메이션 용 sprite 포함 object
    [SerializeField] private Transform viewTransform;
    [SerializeField] private GameObject spriteAnimatorObj;
    [SerializeField] private SpriteRenderer spriteAniRenderer;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] private GameObject paticleObj;

    [SerializeField] private GameObject laserViewObj;
    [SerializeField] private Transform laserStartPoint;
    [SerializeField] private Animator laserStartPointAnimator;
    [SerializeField] private Transform laserEndPoint;
    [SerializeField] private Animator laserEndPointAnimator;

    private Coroutine bulletUpdate;
    private Coroutine rotationAnimation;
    private Coroutine scaleAnimation;
    private Coroutine deleteOnlifeTime;
    private Coroutine setColliderSizeOfAniSprite;

    private Vector3 dirVector; // 총알 방향 벡터
    private float dirDegree;   // 총알 방향 각도.

    private OwnerType ownerType;
    private DelGetPosition ownerPos;
    private DelGetPosition ownerDirVec;
    private DelGetDirDegree ownerDirDegree;
    private BuffManager ownerBuff;
    private TransferBulletInfo transferBulletInfo;
    private float addDirVecMagnitude;
    private float additionalVerticalPos;

    private float timeCount;
    private float updateDelayTime;

    // 오브젝트 충돌 후 delete, lifeTime이 다 되서 delete 되었는지 구분을 위한 변수
    private bool isCollisionObject;
    
    // shapePattern 관련
    private Transform parentBulletTransform;
    private ChildBulletCommonProperty childBulletCommonProperty;
    private InitVector initVector;

    private bool active;
    private BulletPresetInfo bulletPresetInfo;
    private BulletParticlePresetInfo bulletParticlePresetInfo;

    private float eulerAngleZ;

    private ParticleSystem bulletParticle;
    private Vector3 perpendicularVec;

    public float laserAdditionalDegree;
    private DeletedCondition deletedCondition;
    #endregion

    #region getter / setter
    public Transform LaserStartPoint { get { return laserStartPoint; } set { laserStartPoint = value; } }
    public Transform LaserEndPoint { get { return laserEndPoint; } set { laserEndPoint = value; } }

    public LineRenderer GetLineRenderer() { return lineRenderer; }
    public TrailRenderer GetTrailRenderer() { return trailRenderer; }
    public OwnerType GetOwnerType() { return ownerType; }
    public DelGetPosition GetOwnerPos() { return ownerPos; }
    public DelGetPosition GetOwnerDirVec() { return ownerDirVec; }
    public DelGetDirDegree GetOwnerDirDegree() { return ownerDirDegree; }
    public BuffManager GetOwnerBuff() { return ownerBuff; }
    public TransferBulletInfo GetTransferBulletInfo() { return transferBulletInfo; }
    public StatusEffectInfo GetStatusEffectInfo() { return info.statusEffectInfo; }
    public GameObject GetColliderObj() { return colliderObj; }
    public ChildBulletCommonProperty GetChildBulletCommonProperty() { return childBulletCommonProperty; }
    public Transform GetParentBulletTransform() { return parentBulletTransform; }
    public InitVector GetInitVector() { return initVector; }


    public Vector3 GetPosition() { return objTransform.position; }
    public float GetAddDirVecMagnitude() { return addDirVecMagnitude; }
    public float GetAdditionalVerticalPos() { return additionalVerticalPos; }
    public ParticleSystem GetBulletParticle() { return bulletParticle; }

    public float GetDirDegree() { return dirDegree; }
    // 현재 바라보는 방향의 vector 반환
    public Vector3 GetDirVector() { return dirVector; }

    public DeletedCondition GetDeletedCondition() { return deletedCondition; }
    public void SetDeletedCondition(DeletedCondition deletedCondition) { this.deletedCondition = deletedCondition; }

    public void SetOwnerType(OwnerType ownerType)
    {
        this.ownerType = ownerType;
    }
    public void SetBulletParticle(ParticleSystem bulletParticle) { this.bulletParticle = bulletParticle; }

    public float DecreaseDamageAfterPierce { get; private set; }
    #endregion

    #region unityFunc
    void Awake()
    {
        active = false;
        //gameObject.hideFlags = HideFlags.HideInHierarchy;
        animator = GetComponentInChildren<Animator>();
        timeCount = 0;
        updateDelayTime = 0;
        // 총알 끼리 무시, 총알 레이어 무시, 현재 임시로 Enemy 13, Wall 14, Bullet 15, Player 16번 쓰고 있음.
        // Physics2D.IgnoreLayerCollision(15, 15);
        // edit -> project settings -> physics2D 에서 레이어별 충돌 무시 설정 가능, 거기서 일단 설정했음
        // PlayerBullet, Player 충돌 무시
        // EnemyBullet, Enemy 충돌 무시
    }

    private void FixedUpdate()
    {
        if (timeCount < updateDelayTime)
        {
            timeCount += Time.fixedDeltaTime;
            return;
        }
        for (int i = 0; i < info.updatePropertiesLength; i++)
        {
            info.updateProperties[i].Update();
        }
    }


    // 총알 전체 회수할 때 onDisable로 회수 처리 하려함.
    void OnDisable()
    {
        if(true == active)
        {
            CommonDelete();
        }
    }
    #endregion

    #region init
    /// <summary>일반(투사체) 총알 초기화 - position이랑 direction만 받음, DeleteAfterSummonBulletProperty 전용 초기화</summary>
    public void Init(BulletInfo bulletInfo, BuffManager ownerBuff, TransferBulletInfo transferBulletInfo, OwnerType ownerType, Vector3 pos, float direction = 0)
    {
        active = true;
        info = bulletInfo;
        this.transferBulletInfo = new TransferBulletInfo(transferBulletInfo);

        // 처음 위치 설정
        objTransform.position = pos;
        objTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        objTransform.localScale = new Vector3(info.scaleX, info.scaleY, 1f);

        SetBulletPresetType();
        SetBulletPresetInfo();

        UpdateTransferBulletInfo();
        ApplyWeaponBuff();
        // 투사체 총알 속성 초기화
        InitProjectileProperty();

        // Owner 정보 초기화
        InitOwnerInfo(ownerType);

        // 총알 속성들 초기화
        InitPropertyClass();

        dirDegree = 0f;
        dirVector = Vector3.right;
        SetDirection(direction);
    }

    // 일반(투사체) 총알 초기화
    public void Init(BulletInfo bulletInfo, BuffManager ownerBuff, OwnerType ownerType, Vector3 pos, float direction, TransferBulletInfo transferBulletInfo, float updateDelayTime)
    {
        active = true;
        info = bulletInfo;
        this.transferBulletInfo = new TransferBulletInfo(transferBulletInfo);
        this.ownerBuff = ownerBuff;
        this.updateDelayTime = updateDelayTime;
        // 처음 위치 설정
        objTransform.position = pos;
        objTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        objTransform.localScale = new Vector3(info.scaleX, info.scaleY, 1f);

        SetBulletPresetType();
        SetBulletPresetInfo();

        UpdateTransferBulletInfo();

        //Debug.Log(name + " , " + info.damage);

        // Owner 정보 초기화
        InitOwnerInfo(ownerType);
        // Owner buff 적용
        ApplyWeaponBuff();
        // 투사체 총알 속성 초기화
        InitProjectileProperty();
        // 총알 속성들 초기화
        InitPropertyClass();

        dirDegree = 0f;
        dirVector = Vector3.right;
        SetDirection(direction);
        if (0 < updateDelayTime)
        {
            objRigidbody.velocity = Vector2.zero;
        }
    }

    // 레이저 총알 초기화
    public void Init(BulletInfo bulletInfo, BuffManager ownerBuff, OwnerType ownerType, float addDirVecMagnitude, float additionalVerticalPos, DelGetPosition ownerPos, DelGetPosition ownerDirVec, DelGetDirDegree ownerDirDegree, TransferBulletInfo transferBulletInfo, float additionalDirDegree)
    {
        laserAdditionalDegree = additionalDirDegree;
        active = true;
        info = bulletInfo;
        this.transferBulletInfo = new TransferBulletInfo(transferBulletInfo);
        this.ownerBuff = ownerBuff;

        UpdateTransferBulletInfo();
        
        // component on/off
        SetColliderActive(false);
        lineRenderer.enabled = true;

        paticleObj.SetActive(false);
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.cyan;
        lineRenderer.startWidth = info.laserSize;
        lineRenderer.endWidth = info.laserSize;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Vector2.zero);
        lineRenderer.SetPosition(1, Vector2.zero);

        // Owner 정보 초기화
        InitOwnerInfo(ownerType);
        // Owner buff 적용, InitPropertyClass 이전에 실행 해야됨.
        ApplyWeaponBuff();

        spriteAnimatorObj.SetActive(false);
        spriteRenderer.sprite = null;
        laserViewObj.SetActive(true);

        // 0529 레이저 임시, 파란색 레이저
        laserStartPointAnimator.SetTrigger("BLUE_CIRCLE");
        laserEndPointAnimator.SetTrigger("BLUE_CIRCLE");
        objTransform.localScale = new Vector3(info.scaleX, info.scaleY, 1f);
        this.ownerPos = ownerPos;
        this.ownerDirVec = ownerDirVec;
        this.ownerDirDegree = ownerDirDegree;
        this.addDirVecMagnitude = addDirVecMagnitude;
        this.additionalVerticalPos = additionalVerticalPos;
        objTransform.position = ownerPos();

        InitPropertyClass();
        //bulletUpdate = StartCoroutine("BulletUpdate");
    }

    /// <summary>FixedOwenrPattern 전용 초기화</summary> 
    public void Init(BulletInfo bulletInfo, BuffManager ownerBuff, OwnerType ownerType, float addDirVecMagnitude, Vector3 pos, DelGetPosition ownerPos, DelGetPosition ownerDirVec, DelGetDirDegree ownerDirDegree, TransferBulletInfo transferBulletInfo)
    {
        active = true;
        info = bulletInfo;
        this.transferBulletInfo = new TransferBulletInfo(transferBulletInfo);
        this.ownerBuff = ownerBuff;

        SetBulletPresetType();
        SetBulletPresetInfo();

        UpdateTransferBulletInfo();

        // Owner 정보 초기화
        InitOwnerInfo(ownerType);

        // Owner buff 적용
        ApplyWeaponBuff();

        // 투사체 총알 속성 초기화
        InitProjectileProperty();

        // InitPropertyClass() 할 때 정보 넘겨야 되서 미리 선언
        this.ownerPos = ownerPos;
        this.ownerDirVec = ownerDirVec;
        this.ownerDirDegree = ownerDirDegree;
        this.addDirVecMagnitude = addDirVecMagnitude;

        // 총알 속성들 초기화
        InitPropertyClass();

        // 처음 위치 설정
        objTransform.position = pos;
        objTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        objTransform.localScale = new Vector3(info.scaleX, info.scaleY, 1f);
        dirDegree = 0f;
        dirVector = Vector3.right;        
        SetDirection(ownerDirDegree());
    }

    /// <summary>child bullet전용 초기화</summary>
    public void Init(BulletInfo bulletInfo, BuffManager ownerBuff, OwnerType ownerType, Transform parentBulletTransform, ChildBulletCommonProperty childBulletCommonProperty, TransferBulletInfo transferBulletInfo, InitVector initVector)
    {
        active = true;
        info = bulletInfo;
        this.transferBulletInfo = new TransferBulletInfo(transferBulletInfo);
        this.ownerBuff = ownerBuff;
        this.parentBulletTransform = parentBulletTransform;
        this.childBulletCommonProperty = childBulletCommonProperty;
        this.initVector = initVector;
        info.lifeTime = childBulletCommonProperty.childBulletLifeTime + childBulletCommonProperty.timeForOriginalShape;

        // 처음 위치 설정
        objTransform.position = parentBulletTransform.position;
        objTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        objTransform.localScale = new Vector3(info.scaleX, info.scaleY, 1f);

        SetBulletPresetType();
        SetBulletPresetInfo();

        UpdateTransferBulletInfo();
        // childBullet은 speed에 의해서 움직이지 않고 UpdateProperty에서 childProperty에 의해 좌표 값 설정해서 스피드 필요 없음.
        info.speed = 0;

        // Owner 정보 초기화
        InitOwnerInfo(ownerType);   
        // Owner buff 적용                            
        ApplyWeaponBuff();
        // 투사체 총알 속성 초기화
        InitProjectileProperty();
        // 총알 속성들 초기화
        InitPropertyClass();

        dirDegree = 0;
        dirVector = Vector3.right;
        SetDirection(initVector.dirDegree);
    }

    private void InitOwnerInfo(OwnerType ownerType)
    {
        this.ownerType = ownerType;
        // Enemy 13, Wall 14, Bullet 15, Player 16번
        switch (ownerType)
        {
            case OwnerType.ENEMY:
                colliderObj.layer = LayerMask.NameToLayer("EnemyBullet");
                break;
            case OwnerType.PLAYER:
                colliderObj.layer = LayerMask.NameToLayer("PlayerBullet");
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
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
        laserViewObj.SetActive(false);

        if(BulletParticleType.NONE != info.bulletParticleType)
        {
            bulletParticle = ParticleManager.Instance.PlayBulletParticle(info.bulletParticleType.ToString(), objTransform.position, objTransform);
        }

        ActivateTrailRenderer();
        ActivateColiider();
        // sprite 애니메이션 적용
        if (BulletAnimationType.NotPlaySpriteAnimation != info.spriteAnimation)
        {
            spriteAnimatorObj.SetActive(true);
            animator.SetTrigger(info.spriteAnimation.ToString());
            spriteRenderer.sprite = null;
            switch (info.colliderType)
            {
                case ColliderType.AUTO_SIZE_BOX:
                    boxCollider.size = new Vector2(0.1f, 0.1f);
                    break;
                case ColliderType.AUTO_SIZE_CIRCLE:
                    circleCollider.radius = 0.1f;
                    break;
                case ColliderType.MANUAL_SIZE_BOX:
                    colliderObj.transform.localScale = new Vector3(1, 1, 1f);
                    boxCollider.size = info.manualSize * info.autoSizeRatio;
                    boxCollider.offset = Vector2.zero;
                    break;
                case ColliderType.MANUAL_SIZE_CIRCLE:
                    colliderObj.transform.localScale = new Vector3(1 / info.scaleX, 1 / info.scaleY, 1f);
                    circleCollider.radius = info.circleManualRadius * info.autoSizeRatio * 0.5f;
                    circleCollider.offset = Vector2.zero;
                    break;
                default:
                    break;
            }
            setColliderSizeOfAniSprite = StartCoroutine(SetColliderSizeOfAniSprite());
        }
        // sprite 애니메이션 미 적용
        else
        {
            spriteAnimatorObj.SetActive(false);
            spriteRenderer.sprite = info.bulletSprite;
            SetColliderSize(spriteRenderer, info.autoSizeRatio);
        }

        if (info.isRandomSpeed)
        {
            info.speed = info.speed * Random.Range(info.speedRandomMinRatio, info.speedRandomMaxRatio);
        }

        // rotate 360도 계속 회전하는 애니메이션 적용
        if (info.showsRotationAnimation)
        {
            rotationAnimation = StartCoroutine(RotationAnimation());
        }
        // scale이 커지고 작아지는 애니메이션 적용
        if (info.showsScaleAnimation)
        {
            scaleAnimation = StartCoroutine(ScaleAnimation());
        }

        // lifeTime이 0 초과되는 값을 가지면 시간이 lifeTime이 지나면 delete 속성 실행
        if (info.lifeTime > 0)
        {
            if (info.isRandomLifeTime)
            {
                info.lifeTime = info.lifeTime * Random.Range(info.lifeTimeRandomMinRatio, info.lifeTimeRandomMaxRatio);
            }
            deleteOnlifeTime = StartCoroutine(DeleteOnLifeTime());
        }

        if (info.soundId >= 0)
        {
            AudioManager.Instance.PlaySound(info.soundId, SOUNDTYPE.WEAPON);
        }

        // 파티클이 포함되어있는 오브젝트 on/ off
        paticleObj.SetActive(info.showsParticle);

        // 튕기는 총알 테스트 용, 일단 컬라이더 임시로 박스만 쓰는 중
        if (true == info.bounceAble)
        {
            boxCollider.isTrigger = false;
            circleCollider.isTrigger = false;
            polygonCollider.isTrigger = false;
        }
        else
        {
            boxCollider.isTrigger = true;
            circleCollider.isTrigger = true;
            polygonCollider.isTrigger = true;
        }

        if (true == info.canBlockBullet)
        {
            if (OwnerType.PLAYER == ownerType)
                colliderObj.layer = LayerMask.NameToLayer("PlayerCanBlockBullet");
            if (OwnerType.ENEMY == ownerType)
                colliderObj.layer = LayerMask.NameToLayer("EnemyCanBlockBullet");
        }
        if (true == info.canReflectBullet)
        {
            if (OwnerType.PLAYER == ownerType)
                colliderObj.layer = LayerMask.NameToLayer("PlayerCanReflectBullet");
            if (OwnerType.ENEMY == ownerType)
                colliderObj.layer = LayerMask.NameToLayer("EnmeyCanReflectBullet");
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

    /// <summary>
    /// 적용할 bulletPresetInfo 있으면 설정
    /// </summary>
    private void SetBulletPresetType()
    {
        if (BulletPresetType.None != info.bulletPresetType)
        {
            this.bulletPresetInfo = BulletPresets.Instance.bulletPresetInfoList[(int)info.bulletPresetType - 1];
        }
        if (BulletParticleType.NONE != info.bulletParticleType)
        {
            this.bulletParticlePresetInfo = BulletPresets.Instance.bulletParticlePresetInfoList[(int)info.bulletParticleType - 1];
        }
    }

    /// <summary>
    /// original info로 설정한 내용 외의 초기 기본 값으로 되어있는 정보들은 프리셋 정보로 설정 
    /// </summary>
    private void SetBulletPresetInfo()
    {
        if (BulletPresetType.None != info.bulletPresetType)
        {
            if (-1 == info.scaleX && -1 == info.scaleY)
            {
                objTransform.localScale = new Vector3(bulletPresetInfo.scaleX, bulletPresetInfo.scaleY, 1f);
            }
            if (null == info.bulletSprite)
                info.bulletSprite = bulletPresetInfo.sprite;
            if (ColliderType.NONE == info.colliderType)
                info.colliderType = bulletPresetInfo.colliderType;
            if (1 == info.autoSizeRatio && 1 != bulletPresetInfo.autoSizeRatio)
                info.autoSizeRatio = bulletPresetInfo.autoSizeRatio;
            if (BulletAnimationType.NotPlaySpriteAnimation == info.spriteAnimation)
                info.spriteAnimation = bulletPresetInfo.spriteAnimation;

            if (-1 == info.lifeTime)
                info.lifeTime = bulletPresetInfo.lifeTime;

            if (-1 == info.manualSize.x && -1 == info.manualSize.y)
                info.manualSize = bulletPresetInfo.manualSize;
            if (-1 == info.circleManualRadius)
                info.circleManualRadius = bulletPresetInfo.circleManualRadius;
            if (-1 == info.colliderOffset.x && -1 == info.colliderOffset.y)
                info.colliderOffset = bulletPresetInfo.colliderOffset;

            info.bulletImpactType = bulletPresetInfo.bulletImpactType;

            if (bulletPresetInfo.isFixedAngle)
                info.isFixedAngle = true;
        }

        if(BulletParticleType.NONE != info.bulletParticleType)
        {
            if(info.appliesCollisionByParticlePresets)
            {
                info.bulletImpactType = bulletParticlePresetInfo.bulletImpactType;

                if (ColliderType.NONE == info.colliderType)
                    info.colliderType = bulletParticlePresetInfo.colliderType;
                if (1 == info.autoSizeRatio && 1 != bulletParticlePresetInfo.autoSizeRatio)
                    info.autoSizeRatio = bulletParticlePresetInfo.autoSizeRatio;
                if (-1 == info.manualSize.x && -1 == info.manualSize.y)
                    info.manualSize = bulletParticlePresetInfo.manualSize;
                if (-1 == info.circleManualRadius)
                    info.circleManualRadius = bulletParticlePresetInfo.circleManualRadius;
                if (-1 == info.colliderOffset.x && -1 == info.colliderOffset.y)
                    info.colliderOffset = bulletParticlePresetInfo.colliderOffset;
            }
        }
    }

    private void SetColliderActive(bool enable)
    {
        boxCollider.enabled = enable;
        circleCollider.enabled = enable;
        polygonCollider.enabled = enable;
    }

    public void ActivateColiider()
    {
        SetColliderActive(false);
        switch (info.colliderType)
        {
            case ColliderType.AUTO_SIZE_BOX:
            case ColliderType.MANUAL_SIZE_BOX:
                boxCollider.enabled = true;
                break;
            case ColliderType.Beam:
            case ColliderType.AUTO_SIZE_CIRCLE:
            case ColliderType.MANUAL_SIZE_CIRCLE:
                circleCollider.enabled = true;
                break;
            case ColliderType.MANUAL_SIZE_RHOMBUS:
            case ColliderType.MANUAL_SIZE_TRIANGLE:
                polygonCollider.enabled = true;
                break;
            default:
                break;
        }
    }

    public void SetColliderSize(SpriteRenderer renderer, float sizeRate)
    {
        float sizeX, sizeY, size;
        switch (info.colliderType)
        {
            case ColliderType.Beam:
                colliderObj.transform.localScale = new Vector3(1 / info.scaleX, 1 / info.scaleY, 1f);
                sizeX = renderer.sprite.bounds.size.x;
                sizeY = renderer.sprite.bounds.size.y;
                size = (sizeX > sizeY) ? sizeY : sizeX;
                circleCollider.radius = size * 0.3f;
                circleCollider.offset = Vector2.zero;
                break;
            case ColliderType.AUTO_SIZE_BOX:
                colliderObj.transform.localScale = new Vector3(1, 1, 1f);
                boxCollider.size = renderer.sprite.bounds.size * sizeRate;
                boxCollider.offset = renderer.sprite.bounds.center;
                break;
            case ColliderType.AUTO_SIZE_CIRCLE:
                colliderObj.transform.localScale = new Vector3(1 / info.scaleX, 1 / info.scaleY, 1f);
                sizeX = renderer.sprite.bounds.size.x;
                sizeY = renderer.sprite.bounds.size.y;
                size = (sizeX > sizeY) ? sizeY : sizeX;
                circleCollider.radius = size * sizeRate * 0.5f;
                circleCollider.offset = renderer.sprite.bounds.center;
                break;
            case ColliderType.MANUAL_SIZE_BOX:
                colliderObj.transform.localScale = new Vector3(1, 1, 1f);
                boxCollider.size = info.manualSize * sizeRate;
                boxCollider.offset = info.colliderOffset;
                break;
            case ColliderType.MANUAL_SIZE_CIRCLE:
                colliderObj.transform.localScale = new Vector3(1 / info.scaleX, 1 / info.scaleY, 1f);
                circleCollider.radius = info.circleManualRadius * sizeRate * 0.5f;
                circleCollider.offset = info.colliderOffset;
                break;
            case ColliderType.MANUAL_SIZE_RHOMBUS:
                colliderObj.transform.localScale = new Vector3(1, 1, 1f);
                Vector2[] rhombusColliderPoints = new Vector2[] { Vector2.up, Vector2.left, Vector2.down, Vector2.right };
                rhombusColliderPoints[0] *= info.manualSize.y * sizeRate;
                rhombusColliderPoints[1] *= info.manualSize.x * sizeRate;
                rhombusColliderPoints[2] *= info.manualSize.y * sizeRate;
                rhombusColliderPoints[3] *= info.manualSize.x * sizeRate;
                polygonCollider.points = rhombusColliderPoints;
                polygonCollider.offset = info.colliderOffset;
                break;
            case ColliderType.MANUAL_SIZE_TRIANGLE:
                colliderObj.transform.localScale = new Vector3(1, 1, 1f);
                Vector2[] triangleColliderPoints = new Vector2[] { Vector2.up, Vector2.left, Vector2.right };
                triangleColliderPoints[0] *= info.manualSize.y * sizeRate;
                triangleColliderPoints[1] *= info.manualSize.x * sizeRate;
                triangleColliderPoints[2] *= info.manualSize.x * sizeRate;
                polygonCollider.points = triangleColliderPoints;
                polygonCollider.offset = info.colliderOffset;
                break;
            default:
                break;
        }
        if (OwnerType.ENEMY == ownerType)
        {
            //boxCollider.size = boxCollider.size / objTransform.localScale.x;
            //circleCollider.radius = circleCollider.radius / objTransform.localScale.x;
        }
    }
    #endregion

    #region velocity, dir Func
    public void RotateSpriteEulerAngle(float rotationAngle)
    {
        eulerAngleZ += rotationAngle;
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
        UpdatePerpendicularVec();
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
        UpdatePerpendicularVec();
    }

    /// <summary> 현재 방향의 각도와 방향벡터에서 매개변수로 받은 각도만큼 회전 및 속도를 지정한다. </summary>
    public void RotateDirection(float dirDegree)
    {
        this.dirDegree += dirDegree;
        if(0 != dirDegree)
            dirVector = MathCalculator.VectorRotate(dirVector, dirDegree);
        if (info.isFixedAngle == false)
        {
            objTransform.rotation = Quaternion.Euler(0, 0, this.dirDegree);
        }
        objRigidbody.velocity = info.speed * dirVector;
        UpdatePerpendicularVec();
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

    private void UpdatePerpendicularVec()
    {
        perpendicularVec = MathCalculator.VectorRotate(dirVector, 90);
    }

    public void TranslatePerpendicular(float magnitude)
    {
        Vector3 translation = info.amplitude * magnitude * perpendicularVec;
        objTransform.Translate(translation, Space.World);
    }
    #endregion

    #region collision
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

    // TODO : collisionPropery안에서도 또 layer 체크하는데 봐서 간소화 시킬 수 있으면 간소화 시킬 예정
    /// <summary> 충돌 속성 실행 Collision </summary>
    public void CollisionBullet(Collision2D coll)
    {
        int length = info.collisionPropertiesLength;
        if (OwnerType.PLAYER == ownerType)
        {
            // TransparentFx 1, enemy 13, wall 14, EnemyCanBlockBullet 20, EnemyCanReflectBullet 21
            if (UtilityClass.CheckLayer(coll.gameObject.layer, 1, 13, 14, 20, 21))
            {
                for (int i = 0; i < length; i++)
                {
                    info.collisionProperties[i].Collision(ref coll);
                }
            }
        }
        else if(OwnerType.ENEMY == ownerType)
        {
            // TransparentFx 1, wall 14, player 16, PlayerCanBlockBullet 18, PlayerCanReflectBullet 19
            if (UtilityClass.CheckLayer(coll.gameObject.layer, 1, 14, 16, 18, 19))
            {
                for (int i = 0; i < length; i++)
                {
                    info.collisionProperties[i].Collision(ref coll);
                }
            }
        }
    }
    
    /// <summary> 충돌 속성 실행 Trigger </summary>
    public void CollisionBullet(Collider2D coll)
    {
        int length = info.collisionPropertiesLength;
        if (OwnerType.PLAYER == ownerType)
        {
            // TransparentFx 1, enemy 13, wall 14, EnemyCanBlockBullet 20, EnemyCanReflectBullet 21
            if (UtilityClass.CheckLayer(coll.gameObject.layer, 1, 13, 14, 20, 21))
            {
                for (int i = 0; i < length; i++)
                {
                    info.collisionProperties[i].Collision(ref coll);
                }
            }
        }
        else if (OwnerType.ENEMY == ownerType)
        {
            // TransparentFx 1, wall 14, player 16, PlayerCanBlockBullet 18, PlayerCanReflectBullet 19
            if (UtilityClass.CheckLayer(coll.gameObject.layer, 1, 14, 16, 18, 19))
            {
                for (int i = 0; i < length; i++)
                {
                    info.collisionProperties[i].Collision(ref coll);
                }
            }
        }
    }
    #endregion

    #region delete
    /// <summary> 삭제 속성 실행 </summary>
    public void DestroyBullet()
    {
        CommonDelete();
        // 삭제 속성 모두 실행
        for (int i = 0; i < info.deletePropertiesLength; i++)
        {
            info.deleteProperties[i].DestroyBullet();
        }
    }

    /// <summary> 충돌로 인한 삭제, 총알 전체 회수로 인한 삭제시 공통적인 삭제 내용 실행</summary>
    private void CommonDelete()
    {
        active = false;
        timeCount = 0;
        updateDelayTime = 0;

        RemoveTrailRenderer();

        // 실행 중인 코루틴이 있으면 코루틴 멈춤
        if (null != bulletUpdate)
        {
            StopCoroutine(bulletUpdate);
        }
        if (null != rotationAnimation)
        {
            StopCoroutine(rotationAnimation);
        }
        if (null != scaleAnimation)
        {
            StopCoroutine(scaleAnimation);
        }
        if (null != deleteOnlifeTime)
        {
            StopCoroutine(deleteOnlifeTime);
        }
        if (null != setColliderSizeOfAniSprite)
        {
            StopCoroutine(setColliderSizeOfAniSprite);
        }

        viewTransform.localRotation = Quaternion.Euler(0, 0, 0);
        viewTransform.localScale = new Vector3(1f, 1f, 1f);

        // 0813 모, 애니메이터로 인해서 sprite가 null이 아니게 되어서 초기화 하기 위해서 넣음
        if (BulletAnimationType.NotPlaySpriteAnimation != info.spriteAnimation)
        {
            animator.Rebind();
        }
    }
    #endregion

    #region func

    public void ActivateTrailRenderer()
    {
        if (info.canActivateTrailRenderer)
        {
            trailRenderer = TrailRendererManager.Instance.ActivateTrailRenderer(info.trailRendererInfo, transform);
        }
    }
    public void RemoveTrailRenderer()
    {
        if (info.canActivateTrailRenderer)
        {
            TrailRendererManager.Instance.RemoveTrailRenderer(trailRenderer.gameObject, trailRenderer, info.trailRendererInfo.remainTime);
            trailRenderer = null;
        }
    }

    /// <summary> weapon -> bulletPattern으로 넘어온 정보 최신화, 이후 enemy에 정보 넘김. </summary>
    private void UpdateTransferBulletInfo()
    {
        // pattern에서 넘어온 정보의 값이 존재 할 경우(0이 아닐 경우) bulletInfo 값에 덮어쓰고 0일 경우 기존 bulletInfo값 그대로 씀.
        if (0 != transferBulletInfo.bulletMoveSpeed)
            info.speed = transferBulletInfo.bulletMoveSpeed;
        if (0 != transferBulletInfo.range)
            info.range = transferBulletInfo.range;
        if (0 != transferBulletInfo.damage)
            info.damage = transferBulletInfo.damage;        
        if (0 != transferBulletInfo.criticalChance)
            info.criticalChance = transferBulletInfo.criticalChance;
    }

    /// <summary> 매개변수 weaponType와 동일 타입인지 확인 후 bool 반환</summary>
    /// <param name="weaponType">확인할 weapontype</param>
    public bool CheckEqualWeaponType(WeaponType weaponType)
    {
        if (weaponType == transferBulletInfo.weaponType)
            return true;
        else
            return false;
    }

    /// <summary>bullet waeponEffect 적용</summary>
    public void ApplyWeaponBuff()
    {
        int type = (int)transferBulletInfo.weaponType;
        // (중요함) 보스 무기 등 테스트 중에 무기 info 생성시 default값이 NULL인데 이러면 total+effect가 1.0이 아닌 1+1=>2가 나옴.
        // 실제로 bullet Speed 자꾸 2배가 되서 찾다가 여기서 버그 발생한 것.
        if (WeaponType.NULL == transferBulletInfo.weaponType) return;
        
        //Debug.Log(info.memo + ", " + (WeaponType)type + ", " + info.bulletType);
        //Debug.Log(name + " zzz : " + ownerBuff);

        if (BulletType.EXPLOSION == info.bulletType) return;
        
        WeaponTargetEffect totalInfo = ownerBuff.WeaponTargetEffectTotal[0];
        WeaponTargetEffect effectInfo = ownerBuff.WeaponTargetEffectTotal[type];

        // 원거리 무기 관통 횟수 +1 증가
        if (effectInfo.increasePierceCount)
        {
            info.pierceCount += 1;
        }

        // 샷건 총알 비유도 총알 방식에서 발사 n초 후 유도총알로 바뀌기
        if (CheckEqualWeaponType(WeaponType.SHOTGUN) && effectInfo.shotgunBulletCanHoming)
        {
            info.homingStartTime = 0.1f;
            info.homingEndTime = 1.5f;
            info.range += 15f;      // 기존의 샷건이 사정거리가 짧은데 유도 총알로 바뀌면서 사거리 증가 시켜야 될 것 같음. 수치는 봐서 조절

            //HomingProperty 중복 생성 방지
            if (false == HasIncludedUpdateProperty(BulletPropertyType.Update, typeof(HomingProperty)))
            {
                info.updateProperties.Add(new HomingProperty());
                info.updatePropertiesLength += 1;
            }
        }

        // 투사체 유도 기능
        if(effectInfo.canHoming)
        {
            info.homingEndTime = 1.5f;
            //HomingProperty 중복 생성 방지
            if (false == HasIncludedUpdateProperty(BulletPropertyType.Update, typeof(HomingProperty)))
            {
                info.updateProperties.Add(new HomingProperty());
                info.updatePropertiesLength += 1;
            }
        }
        
        // 모든 근거리 무기 상대 총알 막기
        if (effectInfo.meleeWeaponsCanBlockBullet)
        {
            info.canBlockBullet = true;
            if (OwnerType.PLAYER == ownerType)
            {
                colliderObj.layer = LayerMask.NameToLayer("PlayerCanBlockBullet");
            }
            if (OwnerType.ENEMY == ownerType)
            {
                colliderObj.layer = LayerMask.NameToLayer("EnemyCanBlockBullet");
            }
        }

        // 모든 근거리 무기 상대 총알 반사
        if (effectInfo.meleeWeaponsCanReflectBullet)
        {
            info.canReflectBullet = true;
            if (OwnerType.PLAYER == ownerType)
            {
                colliderObj.layer = LayerMask.NameToLayer("PlayerCanReflectBullet");
            }
            if (OwnerType.ENEMY == ownerType)
            {
                colliderObj.layer = LayerMask.NameToLayer("EnmeyCanReflectBullet");
            }
        }

        // 총알이 벽에 1회 튕겨집니다. 
        if (effectInfo.bounceAble)
        {
            info.bounceAble = true;
            info.bounceCount = 1;
        }

        // bulletInfo 수치들 공식 최종 적용
        
        // 모든 무기 공격력 n % 증가, n 미정
        info.damage = info.damage * (totalInfo.damageIncrement + effectInfo.damageIncrement + transferBulletInfo.chargedDamageIncrement);
        
        // 모든 무기 치명타 확률 n% 증가, 합 옵션
        info.criticalChance = info.criticalChance + totalInfo.criticalChanceIncrement + effectInfo.damageIncrement;

        // 크리티컬 데미지 상승, 합 옵션
        transferBulletInfo.criticalDamageIncrement = totalInfo.criticalDamageIncrement + effectInfo.criticalDamageIncrement;

        info.speed = info.speed * (totalInfo.bulletSpeedIncrement + effectInfo.bulletSpeedIncrement);    // 총알 이동속도 변화

        if (OwnerType.PLAYER == ownerType)
        {
            DecreaseDamageAfterPierce = 0.3f * (totalInfo.decreaseDamageAfterPierceReduction * effectInfo.decreaseDamageAfterPierceReduction);
        }
    }

    /// <summary> bullet Properties 안에 해당 property가 포함 되어 있는지 여부 확인</summary>
    public bool HasIncludedUpdateProperty(BulletPropertyType bulletPropertyType, System.Type type)
    {
        switch(bulletPropertyType)
        {
            case BulletPropertyType.Collision:
                for (int i = 0; i < info.collisionPropertiesLength; i++)
                {
                    if (type == info.collisionProperties[i].GetType())
                    {
                        return true;
                    }
                }
                break;
            case BulletPropertyType.Update:
                for (int i = 0; i < info.updatePropertiesLength; i++)
                {
                    if (type == info.updateProperties[i].GetType())
                    {
                        return true;
                    }
                }
                break;
            case BulletPropertyType.Delete:
                for (int i = 0; i < info.deletePropertiesLength; i++)
                {
                    if (type == info.deleteProperties[i].GetType())
                    {
                        return true;
                    }
                }
                break;                
            default:
                break;
        }
        return false;
    }
    #endregion

    #region coroutine
    // 애니메이션 트리거 작동 특성 상 setTrigger 이후 한 프레임 뒤에 바뀌나봄. 그래서 코루틴으로 함. 
    /// <summary> Animation Sprite용 collider size 설정</summary>
    private IEnumerator SetColliderSizeOfAniSprite()
    {
        while(true)
        {
            if (null != spriteAniRenderer.sprite)
                break;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }        
        float sizeX, sizeY, size;
        switch (info.colliderType)
        {
            case ColliderType.Beam:
                colliderObj.transform.localScale = new Vector3(1 / info.scaleX, 1 / info.scaleY, 1f);
                sizeX = spriteAniRenderer.sprite.bounds.size.x;
                sizeY = spriteAniRenderer.sprite.bounds.size.y;
                size = (sizeX > sizeY) ? sizeY : sizeX;
                circleCollider.radius = size * 0.3f;
                circleCollider.offset = Vector2.zero;
                break;
            case ColliderType.AUTO_SIZE_BOX:
                colliderObj.transform.localScale = new Vector3(1, 1, 1f);
                boxCollider.size = spriteAniRenderer.sprite.bounds.size * info.autoSizeRatio;
                boxCollider.offset = spriteAniRenderer.sprite.bounds.center;
                break;
            case ColliderType.AUTO_SIZE_CIRCLE:
                colliderObj.transform.localScale = new Vector3(1 / info.scaleX, 1 / info.scaleY, 1f);
                sizeX = spriteAniRenderer.sprite.bounds.size.x;
                sizeY = spriteAniRenderer.sprite.bounds.size.y;
                size = (sizeX > sizeY) ? sizeY : sizeX;
                circleCollider.radius = size * info.autoSizeRatio * 0.5f;
                circleCollider.offset = spriteAniRenderer.sprite.bounds.center;
                break;
            default:
                break;
        }
        //Debug.Log("t : " + Time.time + ", " + spriteAniRenderer.sprite.bounds.size +", colider Size : " + boxCollider.size);
    }


    /// <summary>rotation Z 360도 회전하는 코루틴</summary>
    private IEnumerator RotationAnimation()
    {
        eulerAngleZ = 0;
        float totalRotationAngle = 0;
        while (true)
        {
            if (info.angleOfSpriteRotationMax != -1 && info.angleOfSpriteRotationMax < totalRotationAngle+1)
                break;
            eulerAngleZ += info.angleOfSpriteRotationPerSecond * Time.fixedDeltaTime;
            totalRotationAngle += info.angleOfSpriteRotationPerSecond * Time.fixedDeltaTime;
            viewTransform.localRotation = Quaternion.Euler(0f, 0f, eulerAngleZ);
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
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
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);  // 일단은 약 60 fps 정도로 실행
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
            time += Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }
    }
    #endregion

}
