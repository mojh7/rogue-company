using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;

/*
#region variables
#endregion
#region getter
#endregion
#region setter
#endregion
#region UnityFunction
#endregion
#region Function
#endregion
*/

/// <summary>
/// Player Class
/// </summary>
public class Player : Character
{
    #region variables
    public enum PlayerType { SOCCER, MUSIC, FISH, ARMY }
    //public Joystick joystick;

    [SerializeField]
    private PlayerController controller;    // 플레이어 컨트롤 관련 클래스
    
    private Transform objTransform;

    private RaycastHit2D hit;
    private List<RaycasthitEnemy> raycastHitEnemies;
    private RaycasthitEnemy raycasthitEnemyInfo;
    private int layerMask;  // autoAim을 위한 layerMask
    private int killedEnemyCount;
    


    [SerializeField] private PlayerHpbarUI PlayerHPUi;
    [SerializeField] private WeaponSwitchButton weaponSwitchButton;
    private PlayerData playerData;
    private PlayerData originPlayerData;    // 아이템 효과 적용시 기준이 되는 정보

    // 윤아 0802
    [SerializeField] private Stamina stamina;
    // 윤아 0802
    [SerializeField] private float playTime;


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
    public PlayerController PlayerController { get { return controller; } }
    public Vector3 GetInputVector () { return controller.GetInputVector(); }

    public WeaponSwitchButton GetWeaponSwitchButton() { return weaponSwitchButton; }

    public int GetStamina() { return playerData.Stamina; }
    public int GetSkillGauage() { return playerData.SkillGauge; }

    public void SetStamina(int stamina) { playerData.Stamina = stamina; }
        //Debug.Log(playerData.Stamina); 
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
        layerMask = 1 << LayerMask.NameToLayer("Wall");
    }

    //for debug
    bool canAutoAim = false;
    bool updateAutoAim = true;

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
        if (Input.GetKeyDown(KeyCode.B))
        {
            updateAutoAim = !updateAutoAim;
            Debug.Log("updateAutoAim : " + updateAutoAim);
        }
        if (updateAutoAim)
        {
            SetAim();
        }
        else
        {
            directionVector = controller.GetRecentNormalInputVector();
            directionDegree = directionVector.GetDegFromVector();
        }

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

    public override void Init()
    {
        base.Init();
        pState = CharacterInfo.State.ALIVE;
        ownerType = CharacterInfo.OwnerType.Player;
        immune = CharacterInfo.Immune.NONE;

        animationHandler.Init(this, PlayerManager.Instance.runtimeAnimator);

        shieldCount = 0;
        // Player class 정보가 필요한 UI class에게 Player class 넘기거나, Player에게 필요한 UI 찾기
        GameObject.Find("AttackButton").GetComponent<AttackButton>().SetPlayer(this);
        GameObject.Find("ActiveSkillButton").GetComponent<ActiveSkillButton>().SetPlayer(this);
        weaponSwitchButton = GameObject.Find("WeaponSwitchButton").GetComponent<WeaponSwitchButton>();
        weaponSwitchButton.SetPlayer(this);
        controller = new PlayerController(GameObject.Find("VirtualJoystick").GetComponent<Joystick>());
        PlayerHPUi = GameObject.Find("HPbar").GetComponent<PlayerHpbarUI>();
        buffManager = PlayerBuffManager.Instance.BuffManager;
        buffManager.SetOwner(this);
        stamina = GameObject.Find("Image_stamina").GetComponent<Stamina>();
        stamina.SetPlayer(this);
        //Stamina.Instance.setTotalStamina(100);

        // weaponManager 초기화, 바라보는 방향 각도, 방향 벡터함수 넘기기 위해서 해줘야됨
        weaponManager.Init(this, CharacterInfo.OwnerType.Player);
    }

    public void InitPlayerData(PlayerData playerData)
    {
        Debug.Log("InitPlayerData hp : " + playerData.Hp);
        this.playerData = playerData;
        originPlayerData = playerData;
        UpdatePlayerData();
        PlayerHPUi.SetHpBar(playerData.Hp);
        stamina.setTotalStamina(playerData.StaminaMax);

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

        StopCoroutine(KnockBackCheck());
        StartCoroutine(KnockBackCheck());

        if (playerData.Hp <= 0) Die();

        return damage;
    }

    public void ActiveSkill()
    {
        if(100 == playerData.SkillGauge)
        {
            Debug.Log("Player 스킬 활성화");
            //skillGauge = 0;
        }
    }
    public CustomObject Interact()
    {
        float bestDistance = interactiveCollider2D.radius * 10;
        Collider2D bestCollider = null;

        Collider2D[] collider2D = Physics2D.OverlapCircleAll(transform.position,interactiveCollider2D.radius, 1 << 1);
        
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
    /// <summary>
    /// 캐릭터 이동, 디버그 용으로 WASD Key로 이동 가능  
    /// </summary>
    private void Move()
    {
        // 조이스틱 방향으로 이동하되 입력 거리에 따른 이동속도 차이가 생김.
        objTransform.Translate(controller.GetInputVector() * playerData.MoveSpeed * Time.fixedDeltaTime);
        if(controller.GetInputVector().sqrMagnitude > 0.1f)
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
    /// <summary> 공격 가능 여부 리턴 </summary>
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
        if(killedEnemyCount == 7)
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

    /// <summary>
    /// player 에임 조정, 몬스터 자동 조준 or 조이스틱 방향 
    /// </summary>
    private void SetAim()
    {       
        int enemyTotal = EnemyManager.Instance.GetAliveEnemyTotal();
        
        if (0 == enemyTotal)
        {
            directionVector = controller.GetRecentNormalInputVector();
            directionDegree = directionVector.GetDegFromVector();
            //Debug.Log("enemyTotal = 0, 오토 에임 풀림");
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
            // raycast로 player와 enemy 사이에 장애물이 없는 enmey 방향만 찾아낸다.
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
                directionVector = controller.GetRecentNormalInputVector();
                directionDegree = directionVector.GetDegFromVector();
                //Debug.Log("raycasthitEnemyNum = 0, player와 enemy사이에 장애물 존재, 오토 에임 풀림");
                return;
            }

            // 위에서 찾은 enmey들 중 distance가 가장 작은 값인 enemy쪽 방향으로 조준한다.
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


    // item Player 대상 효과 적용
    public override void ApplyItemEffect(CharacterTargetEffect itemUseEffect)
    {
        // 주로 즉시 효과 볼 내용들이 적용되서 체력, 허기 회복 두개만 쓸 것 같음.

        Debug.Log("소모품 아이템 플레이어 대상 효과 적용");
        if (itemUseEffect.recoveryHp != 0)
        {
            playerData.Hp += itemUseEffect.recoveryHp;
        }
        if (itemUseEffect.recoveryStamina != 0)
        {
            playerData.Stamina += itemUseEffect.recoveryStamina;
        }
        /*
        이런건 버프, 패시브 효과 쪽이 어울림
        moveSpeedIncrement, staminaMaxIncrement, armorIncrement, criticalChanceIncrement
        */
        playerData.StaminaMax = (int)(originPlayerData.StaminaMax * itemUseEffect.staminaMaxIncrement);
        playerData.MoveSpeed = originPlayerData.MoveSpeed * itemUseEffect.moveSpeedIncrement;
    }

    public override void ApplyStatusEffect(StatusEffectInfo statusEffectInfo)
    {
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

/// <summary> Player 조작 관련 Class </summary>
[System.Serializable]
public class PlayerController
{
    [SerializeField]
    private Joystick joystick; // 조이스틱 스크립트

    // 조이스틱 방향
    private Vector3 inputVector;

    public PlayerController(Joystick joystick)
    {
        this.joystick = joystick;
    }

    /// <summary>
    /// 조이스틱이 현재 바라보는 방향의 벡터  
    /// </summary> 
    public Vector3 GetInputVector()
    {    
        float h = joystick.GetHorizontalValue();
        float v = joystick.GetVerticalValue();

        // 조이스틱 일정 거리 이상 움직였을 때 부터 조작 적용. => 적용 미적용 미정
        //if (h * h + v * v > 0.01)
        //{
        inputVector = new Vector3(h, v, 0).normalized;
        //}
        //else inputVector = Vector3.zero;

        return inputVector;
    }

    /// <summary>
    /// 입력한 조이스틱의 가장 최근 Input vector의 normal vector 반환 
    /// </summary>
    public Vector3 GetRecentNormalInputVector()
    {
        //Debug.Log(joystick.GetRecenteNormalInputVector().magnitude);
        return joystick.GetRecentNormalInputVector();
    }
    
}