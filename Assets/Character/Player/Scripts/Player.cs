using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

public class Player : Character
{
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

    // 윤아 0802
    private Stamina stamina;
    // 윤아 0802
    private float playTime;

    private float floorSpeed;
    private int shieldCount;
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
    #endregion

    #region getter
    public PlayerController PlayerController
    {
        get
        {
            return controller;
        }
    }
    public int GetStamina()
    {
        return playerData.Stamina;
    }
    public int GetSkillGauage()
    {
        return playerData.SkillGauge;
    }
    public void SetStamina(int stamina)
    {
        playerData.Stamina = stamina;
    }
    #endregion

    #region setter
    public void SetInFloor()
    {
        floorSpeed = 2;
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
        playTime = 0;
        scaleVector = new Vector3(1f, 1f, 1f);
        isRightDirection = true;
        raycastHitEnemies = new List<RaycasthitEnemy>();
        raycasthitEnemyInfo = new RaycasthitEnemy();
        layerMask = 1 << LayerMask.NameToLayer("TransparentFX");
        floorSpeed = 0;
    }

    // bool e = false;
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
        playTime += Time.deltaTime;
        SetAim();

        // 총구 방향(각도)에 따른 player 우측 혹은 좌측 바라 볼 때 반전되어야 할 object(sprite는 여기서, weaponManager는 스스로 함) scale 조정
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
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
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
        pState = CharacterInfo.State.ALIVE;
        ownerType = CharacterInfo.OwnerType.Player;
        immune = CharacterInfo.Immune.NONE;

        animationHandler.Init(this, PlayerManager.Instance.runtimeAnimator);

        shieldCount = 0;
        InitilizeController();

        PlayerHPUi = GameObject.Find("HPbar").GetComponent<PlayerHpbarUI>();
        buffManager = PlayerBuffManager.Instance.BuffManager;
        buffManager.SetOwner(this);
        stamina = Stamina.Instance;
        stamina.SetPlayer(this);

        // weaponManager 초기화, 바라보는 방향 각도, 방향 벡터함수 넘기기 위해서 해줘야됨
        weaponManager.Init(this, CharacterInfo.OwnerType.Player);
    }

    public void InitPlayerData(PlayerData playerData)
    {
        Debug.Log("InitPlayerData hp : " + playerData.Hp);
        this.playerData = playerData;
        originPlayerData = playerData.Clone();
        UpdatePlayerData();
        PlayerHPUi.SetHpBar(playerData.Hp);
        stamina.SetStaminaBar(playerData.StaminaMax);
    }
    #endregion

    #region function

    protected override void Die()
    {
        GameDataManager.Instance.SetTime(playTime);
        GameStateManager.Instance.GameOver();
        UIManager.Instance.GameOverUI();
    }

    /// <summary>
    /// 쉴드가 있을시 데미지 상관 없이 공격(공격 타입 상관 X) 방어
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

        playerData.Hp -= transferredBulletInfo.damage;
        PlayerHPUi.DecreaseHp(playerData.Hp);
        AttackedEffect();
        if (playerData.Hp <= 0) Die();
        return transferredBulletInfo.damage;
    }

    public override float Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalChance = 0, bool positionBasedKnockBack = false)
    {
        if (CharacterInfo.State.ALIVE != pState)
            return 0;
        float criticalCheck = Random.Range(0f, 1f);
        // 크리티컬 공격
        playerData.Hp -= damage;

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
            Debug.Log("Player 스킬 활성화");
            //skillGauge = 0;
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

    private void Move()
    {
        // 조이스틱 방향으로 이동하되 입력 거리에 따른 이동속도 차이가 생김.
        objTransform.Translate(controller.GetMoveInputVector() * (playerData.MoveSpeed + floorSpeed) * Time.fixedDeltaTime);
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

    public bool AttackAble()
    {
        if (pState == CharacterInfo.State.ALIVE)
            return true;
        else return false;
    }

    public void AddKilledEnemyCount()
    {
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
            playerData.Hp += recoveryHp;
            return true;
        }
        else
            return false;
    }

    protected override void SetAim()
    {
        switch (autoAimType)
        {
            default:
            case CharacterInfo.AutoAimType.RANDOM:
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
            List<CircleCollider2D> enemyColliderList = EnemyManager.Instance.GetEnemyColliderList;
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
                directionVector = controller.GetMoveRecentNormalInputVector();
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

            CircleCollider2D enemyColider = enemyColliderList[raycastHitEnemies[proximateEnemyIndex].index];
            enemyPos = enemyColider.transform.position + new Vector3(enemyColider.offset.x + enemyColider.offset.y, 0);
            directionVector = (enemyPos - objTransform.position);
            directionVector.z = 0;
            directionVector.Normalize();
            directionDegree = directionVector.GetDegFromVector();
        }
    }
    private void SemiAutoAim()
    {
        Vector2 enemyVector;

        int enemyTotal = EnemyManager.Instance.GetAliveEnemyTotal();

        if (0 == enemyTotal)
        {
            directionVector = controller.GetMoveAttackInputVector();
            directionDegree = directionVector.GetDegFromVector();
            return;
        }
        else
        {
            directionVector = controller.GetAttackRecentNormalInputVector();
            directionVector.Normalize();
            List<Enemy> enemyList = EnemyManager.Instance.GetEnemyList;
            List<CircleCollider2D> enemyColliderList = EnemyManager.Instance.GetEnemyColliderList;
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
            CircleCollider2D enemyColider = enemyColliderList[raycastHitEnemies[proximateEnemyIndex].index];
            enemyPos = enemyColider.transform.position + new Vector3(enemyColider.offset.x + enemyColider.offset.y, 0);
            directionVector = (enemyPos - objTransform.position);
            directionDegree = directionVector.GetDegFromVector();
        }
    }
    private void ManualAim()
    {
        directionVector = controller.GetMoveAttackInputVector();
        directionVector.Normalize();
        directionDegree = directionVector.GetDegFromVector();
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

    // item Player 대상 효과 중 버프, 패시브 적용
    public override void ApplyItemEffect(CharacterTargetEffect itemUseEffect)
    {
        playerData.StaminaMax = (int)(originPlayerData.StaminaMax * itemUseEffect.staminaMaxIncrement);
        playerData.MoveSpeed = originPlayerData.MoveSpeed * itemUseEffect.moveSpeedIncrement;
    }

    public override void ApplyStatusEffect(StatusEffectInfo statusEffectInfo)
    {
    }

    protected override bool IsAbnormal()
    {
        return false;
    }
    // 안 쓸거 같음.
    public void UpdatePlayerData()
    {
        // playerData. = originPlayerData. * buffManager.PlayerTargetEffectTotal.
        playerData.StaminaMax = (int)(originPlayerData.StaminaMax * buffManager.CharacterTargetEffectTotal.staminaMaxIncrement);
        playerData.MoveSpeed = originPlayerData.MoveSpeed * buffManager.CharacterTargetEffectTotal.moveSpeedIncrement;
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
    #endregion
}

public class PlayerController
{
    #region components
    private Joystick moveJoyStick;
    private AttackJoyStick attackJoyStick;
    #endregion

    public PlayerController(Joystick moveJoyStick, AttackJoyStick attackJoyStick)
    {
        this.moveJoyStick = moveJoyStick;
        this.attackJoyStick = attackJoyStick;
    }
    public Vector2 GetMoveAttackInputVector()
    {
        if(attackJoyStick.GetAttackDown())
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
    #endregion
}