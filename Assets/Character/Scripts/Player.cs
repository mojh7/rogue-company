using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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



public abstract class Character : MonoBehaviour
{
    protected enum State { NOTSPAWNED, DIE, ALIVE }
    protected Sprite sprite;
    protected Animator animator;
    protected float hp;
    protected State pState;
    public CircleCollider2D interactiveCollider2D;
    public float moveSpeed;     // Character move Speed

    protected bool isAutoAiming;    // 오토에임 적용 유무

    protected Vector3 directionVector;
    protected float directionDegree;  // 바라보는 각도(총구 방향)

    public abstract void Die();

    public abstract Vector3 GetDirVector();
    public abstract float GetDirDegree();
    /**/

}


/// <summary>
/// Player Class
/// </summary>
public class Player : Character
{
    #region variables

    public enum PlayerState { IDLE, DASH, KNOCKBACK, DEAD }
    //public Joystick joystick;


    [SerializeField]
    private PlayerController controller;    // 플레이어 컨트롤 관련 클래스
    private BuffManager buffManager;
    private Transform objTransform;
    private PlayerState state;
    /// <summary> player 크기 </summary>
    private float playerScale;
    private Vector3 scaleVector;    // player scale 우측 (1, 1, 1), 좌측 : (-1, 1, 1) 
    private bool isRightDirection;    // player 방향이 우측이냐(true) 아니냐(flase = 좌측)

    public WeaponManager weaponManager;
    
    
    struct RaycasthitEnemy
    {
        public int index;
        public float distance;
    }

    private RaycastHit2D hit;
    private List<RaycasthitEnemy> raycastHitEnemies;
    private RaycasthitEnemy raycasthitEnemyInfo;
    private int layerMask;

    #endregion

    #region getter
    public PlayerController PlayerController { get { return controller; } }
    public PlayerState State { get { return state; } }
    public bool GetRightDirection(){ return isRightDirection; }
    public Vector3 GetInputVector () { return controller.GetInputVector(); }
    public override Vector3 GetDirVector()
    {
        return directionVector;
    }
    public override float GetDirDegree()
    {
        return directionDegree;
    }
    public Vector3 GetPosition() { return objTransform.position; }
    public BuffManager GetBuffManager() { return buffManager; }
    public WeaponManager GetWeaponManager() { return weaponManager; }
    #endregion
    #region setter
    #endregion

    #region UnityFunction
    void Awake()
    {
        state = PlayerState.IDLE;
        objTransform = GetComponent<Transform>();
        playerScale = 1f;
        scaleVector = new Vector3(playerScale, playerScale, 1);
        buffManager = new BuffManager();
        isRightDirection = true;
        raycastHitEnemies = new List<RaycasthitEnemy>();
        raycasthitEnemyInfo = new RaycasthitEnemy();
        layerMask = 1 << LayerMask.NameToLayer("Wall");

        Init();
    }

    //for debug
    bool canAutoAim = false;
    bool updateAutoAim = false;

    // Update is called once per frame
    void Update()
    {
        // for debug
        if (canAutoAim == false)
        {
            directionVector = controller.GetRecenteNormalInputVector();
            directionDegree = directionVector.GetDegFromVector();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            canAutoAim = !canAutoAim;
            SetAim();
            Debug.Log("autoAim : " + canAutoAim);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            updateAutoAim = !updateAutoAim;
            Debug.Log("updateAutoAim : " + updateAutoAim);
        }
        if (updateAutoAim)
        {
            SetAim();
        }

        // 각도에 따른 player 우측, 좌측 바라보기
        if (-90 <= directionDegree && directionDegree < 90)
        {
            isRightDirection = true;
            scaleVector.x = playerScale;
            objTransform.localScale = scaleVector;
        }
        else
        {
            isRightDirection = false;
            scaleVector.x = -playerScale;
            objTransform.localScale = scaleVector;
        }   
    }

    void FixedUpdate()
    {
        Move();
    }
    #endregion

    #region function
    public void Init()
    {
        // weaponManager 초기화, 바라보는 방향 각도, 방향 벡터함수 넘기기 위해서 해줘야됨
        weaponManager.Init(this);
        // Player class 정보가 필요한 UI class에게 Player class 넘기기
        GameObject.Find("AttackButton").GetComponent<AttackButton>().SetPlayer(this);
        GameObject.Find("WeaponSwitchButton").GetComponent<WeaponSwitchButton>().SetPlayer(this);
        controller = new PlayerController(GameObject.Find("VirtualJoystick").GetComponent<Joystick>());
    }
    public override void Die()
    {
        throw new System.NotImplementedException();
    }
    public bool Interact()
    {
        float bestDistance = interactiveCollider2D.radius * 10;
        Collider2D bestCollider = null;

        Collider2D[] collider2D = Physics2D.OverlapCircleAll(transform.position, interactiveCollider2D.radius);

        for (int i = 0; i < collider2D.Length; i++)
        {
            if (null == collider2D[i].GetComponent<CustomObject>())
                continue;
            if (!collider2D[i].GetComponent<CustomObject>().isAvailable)
                continue;
            float distance = Vector2.Distance(transform.position, collider2D[i].transform.position);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestCollider = collider2D[i];
            }
        }

        if (null == bestCollider)
            return false;
        bestCollider.GetComponent<CustomObject>().Active();
        return true;
    }
    /// <summary>
    /// 캐릭터 이동, 디버그 용으로 WASD Key로 이동 가능  
    /// </summary>
    private void Move()
    {
        // 조이스틱 방향으로 이동하되 입력 거리에 따른 이동속도 차이가 생김.
        objTransform.Translate(controller.GetInputVector() * moveSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }
    }
    /// <summary> 공격 가능 여부 리턴 </summary>
    public bool AttackAble()
    {
        if (state == PlayerState.IDLE)
            return true;
        else return false;
    }

    public void SetAim()
    {       
        int enemyTotal = EnemyGenerator.Instance.GetAliveEnemyTotal();
        
        if (enemyTotal == 0)
        {
            directionVector = controller.GetRecenteNormalInputVector();
            directionDegree = directionVector.GetDegFromVector();
        }
        else
        {
            List<Enemy> enemyList = EnemyGenerator.Instance.GetEnemyList();
            
            raycastHitEnemies.Clear();
            int raycasthitEnemyNum = 0;
            float minDistance = 1000f;
            int proximateEnemyIndex = -1;

            // Debug.Log("Total : " + enemyTotal);

            // raycast로 player와 enemy 사이에 장애물이 없는 enmey 방향만 찾아낸다.
            for (int i = 0; i < enemyTotal; i++)
            {
                raycasthitEnemyInfo.index = i;
                raycasthitEnemyInfo.distance = Vector2.Distance(enemyList[i].transform.position, objTransform.position);
                hit = Physics2D.Raycast(objTransform.position, enemyList[i].transform.position - objTransform.position, raycasthitEnemyInfo.distance, layerMask);
                if(hit.collider == null)
                {
                    raycastHitEnemies.Add(raycasthitEnemyInfo);
                    raycasthitEnemyNum += 1;
                }
            }

            if (raycasthitEnemyNum == 0)
            {
                directionVector = controller.GetRecenteNormalInputVector();
                directionDegree = directionVector.GetDegFromVector();
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

            directionVector = (enemyList[raycastHitEnemies[proximateEnemyIndex].index].transform.position - objTransform.position).normalized;
            directionDegree = directionVector.GetDegFromVector();
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

    // 조이스틱 터치 유무에 상관없이 가장 최근 Input vector
    public Vector3 GetRecenteNormalInputVector()
    {
        return joystick.GetRecenteNormalInputVector();
    }
    
}