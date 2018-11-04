using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

public class Player : Character
{
    #region components
    [SerializeField]
    protected SpriteRenderer headRenderer;
    #endregion
    #region variables
    public enum PlayerType { SOCCER, MUSIC, FISH, ARMY }

    [SerializeField]
    private PlayerController controller;    // 플레이어 컨트롤 관련 클래스

    private Transform objTransform;

    private RaycastHit2D hit;
    private List<RaycasthitEnemy> raycastHitEnemies;
    private RaycasthitEnemy raycasthitEnemyInfo;
    private int layerMask;  // autoAim을 위한 layerMask
    private int killedEnemyCount;

    [SerializeField] private PlayerHpbarUI PlayerHPUi;
    private PlayerData playerData;
    private PlayerData originPlayerData;    // 아이템 효과 적용시 기준이 되는 정보

    private float skillGageMultiple;
    private float steminaGageMultiple;

    // 윤아 0802
    private Stamina stamina;

    private float floorSpeed;
    private int shieldCount;

    [SerializeField]
    private GameObject muzzlePosObj;
    [SerializeField]
    private Transform muzzlePosTransform;

    private SkillData skillData; 
    #endregion

    #region property
    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
        set
        {
            playerData = value;
        }
    }
    public int KilledEnemyCount
    {
        get
        {
            return killedEnemyCount;
        }
    }
    public int ShieldCount
    {
        private set { shieldCount = value; }
        get { return shieldCount; }
    }

    public bool IsNotConsumeStamina
    {
        get; private set;
    }
    public bool IsNotConsumeAmmo
    {
        get; private set;
    }

    #endregion

    #region getter
    public PlayerController PlayerController
    {
        get
        {
            return controller;
        }
    }
    #endregion

    #region setter
    public void SetInFloor()
    {
        floorSpeed = .5f;
    }
    public void SetInRoom()
    {
        floorSpeed = 0;
    }
    #endregion

    #region UnityFunc
    void Awake()
    {
        objTransform = GetComponent<Transform>();
        scaleVector = new Vector3(1f, 1f, 1f);
        isRightDirection = true;
        raycastHitEnemies = new List<RaycasthitEnemy>();
        raycasthitEnemyInfo = new RaycasthitEnemy();
        layerMask = 1 << LayerMask.NameToLayer("TransparentFX");
        floorSpeed = 0;

        isAbnormalStatuses = new bool[(int)AbnormalStatusType.END];
        abnormalStatusCounts = new int[(int)AbnormalStatusType.END];
        overlappingCounts = new int[(int)AbnormalStatusType.END];
        abnormalStatusCoroutines = new Coroutine[(int)AbnormalStatusType.END];
        abnormalStatusTime = new float[(int)AbnormalStatusType.END];
        abnormalStatusDurationTime = new float[(int)AbnormalStatusType.END];
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(false == e && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("무기 장착");
            e = true;
            // weaponManager 초기화, 바라보는 방향 각도, 방향 벡터함수 넘기기 위해서 해줘야됨
            weaponManager.Init(this, OwnerType.Player);
        }
        */

        // 총구 방향(각도)에 따른 player 우측 혹은 좌측 바라 볼 때 반전되어야 할 object(sprite는 여기서, weaponManager는 스스로 함) scale 조정
        if (Input.GetKeyDown(KeyCode.Q))
            ActiveSkill();
        if (Input.GetKeyDown(KeyCode.LeftShift))
            Evade();
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
        headRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
        if (isEvade)
            return;
        if (-90 <= directionDegree && directionDegree < 90)
        {
            isRightDirection = true;
            scaleVector.x = 1f;
            spriteTransform.localScale = scaleVector;
        }
        else
        {
            isRightDirection = false;
            scaleVector.x = -1f;
            spriteTransform.localScale = scaleVector;
        }
    }

    void FixedUpdate()
    {
        Move();
    }
    #endregion

    #region initialzation

    void InitilizeController()
    {
        ControllerUI.Instance.SetPlayer(this, ref controller);
    }

    public override void Init()
    {
        base.Init();
        baseColor = Color.white;
        pState = CharacterInfo.State.ALIVE;
        ownerType = CharacterInfo.OwnerType.Player;
        damageImmune = CharacterInfo.DamageImmune.NONE;
        abnormalImmune = CharacterInfo.AbnormalImmune.NONE;

        animationHandler.Init(this, PlayerManager.Instance.runtimeAnimator);

        IsNotConsumeStamina = false;
        IsNotConsumeAmmo = false;

        DeactivateAbnormalComponents();
        directionVector = new Vector3(1, 0, 0);
        shieldCount = 0;
        evadeCoolTime = 0.05f;
        battleSpeed = 0.5f;
        InitilizeController();

        PlayerHPUi = GameObject.Find("HPbar").GetComponent<PlayerHpbarUI>();
        buffManager = PlayerBuffManager.Instance.BuffManager;
        buffManager.SetOwner(this);
        stamina = Stamina.Instance;
        stamina.SetPlayer(this);

        // weaponManager 초기화, 바라보는 방향 각도, 방향 벡터함수 넘기기 위해서 해줘야됨
        weaponManager.Init(this, CharacterInfo.OwnerType.Player);

        animationHandler.SetEndAction(EndEvade);
        TimeController.Instance.PlayStart();

        DeactivateAbnormalComponents();
        InitStatusEffects();
    }

    public void InitPlayerData(PlayerData playerData)
    {
        skillGageMultiple = 1;
        steminaGageMultiple = 1;
        Debug.Log("InitPlayerData hp : " + playerData.Hp);
        this.playerData = playerData;
        originPlayerData = playerData.Clone();
        ApplyItemEffect();
        PlayerHPUi.SetHpBar(playerData.Hp);
        stamina.SetStaminaBar(playerData.StaminaMax);
        playerData.SkillGauge = 100;
    }
    #endregion

    #region function
    private void ScaleChange(float scale)
    {
        if(scale != this.transform.localScale.x)
        {
            ParticleManager.Instance.PlayParticle("Smoke", this.transform.position);
        }
        this.transform.localScale = Vector3.one * scale;
        this.GetComponentInChildren<Camera>().transform.localScale = Vector3.one / scale;
    }
    public void UpdateMuzzlePosition(Vector3 pos, bool visible)
    {
        muzzlePosObj.SetActive(visible);
        muzzlePosTransform.position = pos;
    }

    public override bool Evade()
    {
        if (isEvade || !canEvade)
            return false;
        controller.AttackJoyStickUp();
        canEvade = false;
        isEvade = true;
        gameObject.layer = 0;
        directionVector = controller.GetMoveRecentNormalInputVector();
        directionVector.Normalize();
        directionDegree = directionVector.GetDegFromVector();
        if (-90 <= directionDegree && directionDegree < 90)
        {
            isRightDirection = true;
            scaleVector.x = 1f;
            spriteTransform.localScale = scaleVector;
        }
        else
        {
            isRightDirection = false;
            scaleVector.x = -1f;
            spriteTransform.localScale = scaleVector;
        }
        ParticleManager.Instance.PlayParticle("WalkSmoke", transform.position, spriteRenderer.transform.localScale);
        animationHandler.Skill(0);
        weaponManager.HideWeapon();
        StartCoroutine(Roll(directionVector));
        return true;
    }

    protected override void Die()
    {
        GameDataManager.Instance.SetTime(TimeController.Instance.GetPlayTime);
        UIManager.Instance.GameOverUI();
        GameStateManager.Instance.GameOver();
    }

    /// <summary>
    /// 쉴드가 있을시 데미지 상관 없이 공격(공격 타입 상관 X) 방어, 아직 미사용
    /// </summary>
    public bool DefendAttack()
    {
        if (0 >= shieldCount)
            return false;

        shieldCount -= 1;
        // 버프매니저 쪽으로 쉴드 버프 없애는 명령 보내기
        return true;
    }

    public override float Attacked(TransferBulletInfo transferredBulletInfo)
    {
        // if (DefendAttack()) return 0;
        if(damageImmune == CharacterInfo.DamageImmune.ALL)
        {
            ParticleManager.Instance.PlayParticle("Guard", interactiveCollider2D.transform.position);
            return 0;
        }
        if (CharacterInfo.State.ALIVE != pState || isEvade)
            return 0;
        playerData.Hp -= transferredBulletInfo.damage;
        AttackedAction(1);
        PlayerHPUi.DecreaseHp(playerData.Hp);
        AttackedEffect();
        if (playerData.Hp <= 0) Die();
        return transferredBulletInfo.damage;
    }

    public override float Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalChance = 0, bool positionBasedKnockBack = false)
    {
        if (damageImmune == CharacterInfo.DamageImmune.ALL)
        {
            ParticleManager.Instance.PlayParticle("Guard", interactiveCollider2D.transform.position);
            return 0;
        }
        if (CharacterInfo.State.ALIVE != pState || isEvade)
            return 0;
        float criticalCheck = Random.Range(0f, 1f);
        // 크리티컬 공격
        playerData.Hp -= damage;
        AttackedAction(damage);
        if (knockBack > 0)
            isKnockBack = true;

        // 넉백 총알 방향 : 총알 이동 방향 or 몬스터-총알 방향 벡터
        rgbody.velocity = Vector3.zero;

        // bullet과 충돌 Object 위치 차이 기반의 넉백  
        if (positionBasedKnockBack)
        {
            rgbody.AddForce(knockBack * ((Vector2)transform.position - bulletPos).normalized);
        }
        // bullet 방향 기반의 넉백
        else
        {
            rgbody.AddForce(knockBack * _dir);
        }
        PlayerHPUi.DecreaseHp(playerData.Hp);
        AttackedEffect();
        StopCoroutine(KnockBackCheck());
        StartCoroutine(KnockBackCheck());

        if (playerData.Hp <= 0) Die();

        return damage;
    }

    public override void ActiveSkill()
    {
        if (100 == playerData.SkillGauge)
        {
            playerData.SkillData.Run(this, null, 1);
            playerData.SkillGauge = 0;
            controller.ActiveSkill(playerData.SkillGauge, 100);
        }
    }

    public override void SetAim()
    {
        switch (autoAimType)
        {
            default:
            case CharacterInfo.AutoAimType.REACTANCE:
            case CharacterInfo.AutoAimType.AUTO:
                AutoAim();
                break;
            case CharacterInfo.AutoAimType.SEMIAUTO:
                SemiAutoAim();
                break;
            case CharacterInfo.AutoAimType.MANUAL:
                ManualAim();
                break;
        }
    }

    public override CustomObject Interact()
    {
        float bestDistance = interactiveCollider2D.radius * 10;
        Collider2D bestCollider = null;

        Collider2D[] collider2D = Physics2D.OverlapCircleAll(transform.position, interactiveCollider2D.radius, (1 << 1) | (1 << 9));

        for (int i = 0; i < collider2D.Length; i++)
        {
            if (!collider2D[i].GetComponent<CustomObject>().GetAvailable())
                continue;
            float distance = Vector2.Distance(transform.position, collider2D[i].transform.position);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestCollider = collider2D[i];
            }
        }

        if (null == bestCollider)
            return null;

        return bestCollider.GetComponent<CustomObject>();
    }

    public bool AttackAble()
    {
        if (pState == CharacterInfo.State.ALIVE && !isEvade)
            return true;
        else return false;
    }

    public void AddKilledEnemyCount()
    {
        
        playerData.SkillGauge += (int)(10 * skillGageMultiple);
        if (playerData.SkillGauge >= 100)
            playerData.SkillGauge = 100;
        controller.ActiveSkill(playerData.SkillGauge, 100);
        if (false == buffManager.CharacterTargetEffectTotal.canDrainHp) return;
        killedEnemyCount += 1;
        if (killedEnemyCount == 7)
        {
            Debug.Log("몬스터 7마리 처치 후 피 회복");
            RecoverHp(1f);
            killedEnemyCount = 0;
        }
    }

    public bool ConsumeStamina(int staminaConsumption)
    {
        if (0 <= playerData.Stamina)
            return false;
        playerData.Stamina -= staminaConsumption;
        if (0 <= playerData.Stamina)
            playerData.Stamina = 0;

        return true;
    }

    public bool RecoverHp(float recoveryHp)
    {
        if (playerData.Hp + recoveryHp <= playerData.HpMax)
        {
            ParticleManager.Instance.PlayParticle("Hp", this.transform.position);
            playerData.Hp += recoveryHp;
            return true;
        }
        else
            return false;
    }

    private void AttackedAction(float power)
    {
        CameraController.Instance.Shake(0.2f, 0.2f);
        LayerController.Instance.FlashAttackedLayer(0.2f);
    }
    private void AutoAim()
    {
        int enemyTotal = EnemyManager.Instance.GetAliveEnemyTotal();

        if (0 == enemyTotal)
        {
            directionVector = controller.GetMoveAttackInputVector();
            directionVector.Normalize();
            directionDegree = directionVector.GetDegFromVector();
            return;
        }
        else
        {
            List<Enemy> enemyList = EnemyManager.Instance.GetEnemyList;
            List<BoxCollider2D> enemyColliderList = EnemyManager.Instance.GetEnemyColliderList;
            raycastHitEnemies.Clear();
            int raycasthitEnemyNum = 0;
            float minDistance = 10000f;
            int proximateEnemyIndex = -1;

            Vector3 enemyPos = new Vector3(0, 0, 0);
            for (int i = 0; i < enemyTotal; i++)
            {
                raycasthitEnemyInfo.index = i;
                enemyPos = enemyColliderList[i].transform.position + new Vector3(enemyColliderList[i].offset.x + enemyColliderList[i].offset.y, 0);
                raycasthitEnemyInfo.distance = Vector2.Distance(enemyPos, objTransform.position);
                hit = Physics2D.Raycast(objTransform.position, enemyPos - objTransform.position, raycasthitEnemyInfo.distance, layerMask);
                if (hit.collider == null)
                {
                    raycastHitEnemies.Add(raycasthitEnemyInfo);
                    raycasthitEnemyNum += 1;
                }
            }

            if (raycasthitEnemyNum == 0)
            {
                isBattle = false;
                directionVector = controller.GetMoveAttackInputVector();
                directionDegree = directionVector.GetDegFromVector();
                return;
            }

            for (int j = 0; j < raycasthitEnemyNum; j++)
            {
                if (raycastHitEnemies[j].distance <= minDistance)
                {
                    minDistance = raycastHitEnemies[j].distance;
                    proximateEnemyIndex = j;
                }
            }

            BoxCollider2D enemyColider = enemyColliderList[raycastHitEnemies[proximateEnemyIndex].index];
            enemyPos = enemyColider.transform.position + new Vector3(enemyColider.offset.x + enemyColider.offset.y, 0);
            directionVector = (enemyPos - objTransform.position);
            directionVector.z = 0;
            directionVector.Normalize();
            directionDegree = directionVector.GetDegFromVector();
            isBattle = true;
        }
    }
    private void SemiAutoAim()
    {
        Vector2 enemyVector;

        int enemyTotal = EnemyManager.Instance.GetAliveEnemyTotal();

        if (0 == enemyTotal)
        {
            directionVector = controller.GetMoveAttackInputVector();
            directionVector.Normalize();
            directionDegree = directionVector.GetDegFromVector();
            isBattle = false;
            return;
        }
        else
        {
            directionVector = controller.GetAttackRecentNormalInputVector();
            directionVector.Normalize();
            List<Enemy> enemyList = EnemyManager.Instance.GetEnemyList;
            List<BoxCollider2D> enemyColliderList = EnemyManager.Instance.GetEnemyColliderList;
            raycastHitEnemies.Clear();
            int raycasthitEnemyNum = 0;
            float minDistance = 10000f;
            int proximateEnemyIndex = -1;

            Vector3 enemyPos;
            for (int i = 0; i < enemyTotal; i++)
            {
                raycasthitEnemyInfo.index = i;
                enemyPos = enemyColliderList[i].transform.position;
                raycasthitEnemyInfo.distance = Vector2.Distance(enemyPos, objTransform.position);
                enemyVector = enemyPos - objTransform.position;
                hit = Physics2D.Raycast(objTransform.position, enemyVector, raycasthitEnemyInfo.distance, layerMask);
                if (hit.collider == null && Vector2.Dot(directionVector, enemyVector.normalized) >.25f)
                {
                    raycastHitEnemies.Add(raycasthitEnemyInfo);
                    raycasthitEnemyNum += 1;
                }
            }

            if (raycasthitEnemyNum == 0)
            {
                directionDegree = directionVector.GetDegFromVector();
                return;
            }

            for (int j = 0; j < raycasthitEnemyNum; j++)
            {
                if (raycastHitEnemies[j].distance <= minDistance)
                {
                    minDistance = raycastHitEnemies[j].distance;
                    proximateEnemyIndex = j;
                }
            }
            BoxCollider2D enemyColider = enemyColliderList[raycastHitEnemies[proximateEnemyIndex].index];
            enemyPos = enemyColider.transform.position + new Vector3(enemyColider.offset.x + enemyColider.offset.y, 0);
            directionVector = (enemyPos - objTransform.position);
            directionVector.Normalize();
            directionDegree = directionVector.GetDegFromVector();
        }
        isBattle = true;
    }
    private void ManualAim()
    {
        directionVector = controller.GetMoveAttackInputVector();
        directionVector.Normalize();
        directionDegree = directionVector.GetDegFromVector();
        isBattle = true;
    }
    private void Move()
    {
        if (isEvade)
            return;
        if(isBattle)
        {
            totalSpeed = playerData.MoveSpeed + floorSpeed - battleSpeed;
        }
        else
        {
            totalSpeed = playerData.MoveSpeed + floorSpeed;
        }
        rgbody.MovePosition(objTransform.position 
            + controller.GetMoveInputVector() * (totalSpeed) * Time.fixedDeltaTime);
        // 조이스틱 방향으로 이동하되 입력 거리에 따른 이동속도 차이가 생김.
        //objTransform.Translate(controller.GetMoveInputVector() * (playerData.MoveSpeed + floorSpeed) * Time.fixedDeltaTime);
        if (controller.GetMoveInputVector().sqrMagnitude > 0.1f)
        {
            animationHandler.Walk();
        }
        else
        {
            animationHandler.Idle();
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector2.up * playerData.MoveSpeed * Time.fixedDeltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.down * playerData.MoveSpeed * Time.fixedDeltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * playerData.MoveSpeed * Time.fixedDeltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * playerData.MoveSpeed * Time.fixedDeltaTime);
        }
    }
    private void EndEvade()
    {
        isEvade = false;
        gameObject.layer = 16;
        weaponManager.RevealWeapon();
    }

    // total을 안 거치고 바로 효과 적용하기 위해 구분함, 소모형 아이템 용 함수
    public void ApplyConsumableItem(CharacterTargetEffect itemUseEffect)
    {
        Debug.Log("소모품 아이템 플레이어 대상 효과 적용");
        if (0 != itemUseEffect.recoveryHp)
        {
            playerData.Hp += itemUseEffect.recoveryHp;
        }
        if (0 != itemUseEffect.recoveryStamina)
        {
            playerData.Stamina += itemUseEffect.recoveryStamina;
        }
    }

    public override void ApplyItemEffect()
    {
        CharacterTargetEffect itemUseEffect = buffManager.CharacterTargetEffectTotal;
        playerData.StaminaMax = (int)(originPlayerData.StaminaMax * itemUseEffect.staminaMaxIncrement);
        if(playerData.MoveSpeed != originPlayerData.MoveSpeed * itemUseEffect.moveSpeedIncrement)
        {
            if(playerData.MoveSpeed > originPlayerData.MoveSpeed * itemUseEffect.moveSpeedIncrement)
            {
                ParticleManager.Instance.PlayParticle("SpeedDown", this.transform.position  );
            }
            else
            {
                ParticleManager.Instance.PlayParticle("SpeedUp", this.transform.position);
            }
        }
        playerData.MoveSpeed = originPlayerData.MoveSpeed * itemUseEffect.moveSpeedIncrement;
        IsNotConsumeStamina = itemUseEffect.isNotConsumeStamina;
        IsNotConsumeAmmo = itemUseEffect.isNotConsumeAmmo;
        damageImmune = itemUseEffect.isImmuneDamage;
        abnormalImmune = itemUseEffect.isImmuneAbnormal;

        if (itemUseEffect.hpRatio > 0)
            playerData.Hp = originPlayerData.Hp * itemUseEffect.hpRatio;
        if (itemUseEffect.hpMaxRatio > 0)
            playerData.HpMax = originPlayerData.HpMax * itemUseEffect.hpMaxRatio;
        if (itemUseEffect.skillGage > 0)
            skillGageMultiple = itemUseEffect.skillGage;
        if (itemUseEffect.steminaGage > 0)
            steminaGageMultiple = itemUseEffect.steminaGage;

        if (itemUseEffect.charScale > 0 && itemUseEffect.charScale <= 2f)
            ScaleChange(itemUseEffect.charScale);
    }

    protected override bool IsAbnormal()
    {
        return false;
    }
    #endregion

    #region abnormalStatusFunc
    private bool AbnormalChance(float appliedChance)
    {
        float chance = Random.Range(0, 1f);
        if (chance < appliedChance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void ApplyStatusEffect(StatusEffectInfo statusEffectInfo)
    {
        if (CharacterInfo.State.ALIVE != pState || null == statusEffectInfo || isEvade || abnormalImmune == CharacterInfo.AbnormalImmune.ALL)
            return;

        //if (0 != statusEffectInfo.knockBack)
        //    KnockBack(statusEffectInfo.knockBack, statusEffectInfo.BulletDir, statusEffectInfo.BulletPos, statusEffectInfo.positionBasedKnockBack);

        //if (true == statusEffectInfo.canPoison)
        //    Poison(statusEffectInfo.posionChance);
        if (true == statusEffectInfo.canBurn)
            Burn(statusEffectInfo.burnChance);
        //if (true == statusEffectInfo.canDelayState)
        //    DelayState(statusEffectInfo.delayStateChance);

        //if (true == statusEffectInfo.canNag)
        //    Nag(statusEffectInfo.nagChance);
        //if (true == statusEffectInfo.canClimb)
        //    Climbing(statusEffectInfo.climbChance);
        //if (true == statusEffectInfo.graveyardShift)
        //    GraveyardShift(statusEffectInfo.graveyardShiftChance);
        //if (true == statusEffectInfo.canFreeze)
        //    Freeze(statusEffectInfo.freezeChance);
        //if (true == statusEffectInfo.reactance)
        //    Reactance(statusEffectInfo.reactanceChance);

        //if (0 != statusEffectInfo.stun)
        //    Stun(statusEffectInfo.stun, statusEffectInfo.stunChance);
        //if (0 != statusEffectInfo.charm)
        //    Charm(statusEffectInfo.charm, statusEffectInfo.charmChance);
    }

    // 불 처리랑 데미지 따로 있는데 player 일단 임시로 해놓음.
    private void Burn(float chance)
    {
        if (false == AbnormalChance(chance))
            return;

        if (false == isBurning)
        {
            burnCoroutine = StartCoroutine(BurnCoroutine());
        }
    }


    #endregion

    #region abnormalStatusCoroutine
    IEnumerator BurnCoroutine()
    {
        isBurning = true;
        abnormalComponents.BurnEffect.SetActive(true);
        // 불 처리랑 데미지 enemy는 따로 뭐가 있는데 player는 일단 임시로 이렇게함.
        int count = 6;
        //float damage = 1;
        while (count > 0)
        {
            yield return YieldInstructionCache.WaitForSeconds(0.5f);
            if (abnormalImmune == CharacterInfo.AbnormalImmune.NONE)
            {
                playerData.Hp -= 1;
                PlayerHPUi.DecreaseHp(playerData.Hp);
            }
            count -= 1;
        }
        abnormalComponents.BurnEffect.SetActive(false);
        isBurning = false;
    }
    #endregion
    #region coroutine
    private IEnumerator KnockBackCheck()
    {
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
            if (Vector2.zero != rgbody.velocity && rgbody.velocity.magnitude < 1f)
            {
                //isActiveAI = true;
                //aiController.PlayMove();
            }
        }
    }
    private IEnumerator Roll(Vector3 dir)
    {
        float doubling = 2f;
        totalSpeed = playerData.MoveSpeed + floorSpeed;
        while (isEvade)
        {
            doubling -= Time.fixedDeltaTime * 3;
            if (doubling <= 1f)
                doubling = 1f;
            rgbody.MovePosition(objTransform.position + dir * (totalSpeed) * Time.fixedDeltaTime * doubling);
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }
        yield return YieldInstructionCache.WaitForSeconds(0.05f);
        yield return YieldInstructionCache.WaitForSeconds(evadeCoolTime);
        canEvade = true;
    }
    #endregion
}

public class PlayerController
{
    #region components
    private Joystick moveJoyStick;
    private AttackJoyStick attackJoyStick;
    private ActiveSkillButton activeSkillButton;
    #endregion

    public PlayerController(Joystick moveJoyStick, AttackJoyStick attackJoyStick,ActiveSkillButton activeSkillButton)
    {
        this.moveJoyStick = moveJoyStick;
        this.attackJoyStick = attackJoyStick;
        this.activeSkillButton = activeSkillButton;
    }
    public Vector2 GetMoveAttackInputVector()
    {
        if(attackJoyStick.GetButtonDown())
        {
            return attackJoyStick.GetRecentNormalInputVector();
        }
        return moveJoyStick.GetRecentNormalInputVector();
    }
    #region move
    /// <summary>
    /// 조이스틱이 현재 바라보는 방향의 벡터  
    /// </summary> 
    public Vector3 GetMoveInputVector()
    {
        float h = moveJoyStick.GetHorizontalValue();
        float v = moveJoyStick.GetVerticalValue();

        return new Vector3(h, v, 0).normalized;
    }

    /// <summary>
    /// 입력한 조이스틱의 가장 최근 Input vector의 normal vector 반환 
    /// </summary>
    public Vector3 GetMoveRecentNormalInputVector()
    {
        return moveJoyStick.GetRecentNormalInputVector();
    }
    #endregion
    #region attack
    /// <summary>
    /// 조이스틱이 현재 바라보는 방향의 벡터  
    /// </summary> 
    public Vector3 GetAttackInputVector()
    {
        float h = attackJoyStick.GetHorizontalValue();
        float v = attackJoyStick.GetVerticalValue();

        return new Vector3(h, v, 0).normalized; 
    }

    /// <summary>
    /// 입력한 조이스틱의 가장 최근 Input vector의 normal vector 반환 
    /// </summary>
    public Vector3 GetAttackRecentNormalInputVector()
    {
        return attackJoyStick.GetRecentNormalInputVector();
    }

    public void AttackJoyStickUp()
    {
        attackJoyStick.OnPointerUp(null);
    }
    #endregion
    #region skill
    public void ActiveSkill(int sum, int total)
    {
        activeSkillButton.ChargeFill((float)sum / total);
    }
    #endregion
}