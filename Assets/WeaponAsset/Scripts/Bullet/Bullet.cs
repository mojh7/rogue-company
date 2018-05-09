using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletData;
using DelegateCollection;

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
    public Rigidbody2D objRigidbody;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    private Coroutine bulletUpdate;

    private Vector3 dirVector; // 총알 방향 벡터

    private DelGetPosition ownerDirVec;
    private DelGetPosition ownerPos;
    private float addDirVecMagnitude;

    [SerializeField]
    // 레이저용 lineRenderer
    private LineRenderer lineRenderer;
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
        //Debug.Log(this + "Awake");
        objTransform = GetComponent<Transform>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        objRigidbody = GetComponent<Rigidbody2D>();

        // 총알 끼리 무시, 총알 레이어 무시, 현재 임시로 Bullet layer 9번, Wall layer 10번 쓰고 있음.
        Physics2D.IgnoreLayerCollision(9, 9);
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
    public void Init(int bulletId, Sprite bulletSprite, float speed, float range, Vector3 pos, float direction)
    {
        info = DataStore.Instance.GetBulletInfo(bulletId);
        if (speed != 0)
        {
            info.speed = speed;
        }
        if (range != 0)
        {
            info.range = range;
        }

        // component on/off
        //boxCollider.enabled = true;
        lineRenderer.enabled = false;
        // sprite 설정
        spriteRenderer.sprite = bulletSprite;
        // 처음 위치랑, 각도 설정.
        objTransform.position = pos;
        objTransform.rotation = Quaternion.Euler(0f, 0f, direction);

        //Debug.Log("각도 : " + direction);
        InitProperty();
        ///bulletUpdate = StartCoroutine("BulletUpdate");

        UpdateDirection(direction);
    }

    // 레이저 총알 초기화
    // 레이저 나중에 빔 모양 말고 처음 시작 지점, raycast hit된 지점에 동그란 원 추가 생성 할 수도 있음.
    public void Init(int bulletId, float addDirVecMagnitude, DelGetPosition ownerPos, DelGetPosition ownerDirVec)
    {
        info = DataStore.Instance.GetBulletInfo(bulletId);
        // component on/off
        boxCollider.enabled = false;
        lineRenderer.enabled = true;
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.cyan;
        lineRenderer.startWidth = 0.4f;
        lineRenderer.endWidth = 0.4f;

        this.ownerPos = ownerPos;
        this.ownerDirVec = ownerDirVec;
        this.addDirVecMagnitude = addDirVecMagnitude;
        // sprite 설정
        spriteRenderer.sprite = null;
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
        objTransform.rotation = Quaternion.Euler(0, 0, dirVector.GetDegFromVector());
        objRigidbody.velocity = info.speed * dirVector;
    }
    public void UpdateDirection(float degree)
    {
        dirVector = MathCalculator.VectorRotate(Vector3.right, degree);
        objTransform.rotation = Quaternion.Euler(0, 0, degree);
        objRigidbody.velocity = info.speed * dirVector;
    }


    // 안쓸 듯
    // 총알 Update 코루틴
    private IEnumerator BulletUpdate()
    {
        while(true)
        {
            // 총알 update 속성 실행
            for (int i = 0; i < info.updatePropertiesLength; i++)
            {
                info.updateProperties[i].Update();
            }
            yield return YieldInstructionCache.WaitForSeconds(0.016f);  // 일단은 약 60 fps 정도로 실행
        }
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
    // 충돌 속성 실행 Collider
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

    // 삭제 속성 실행
    public void DestroyBullet()
    {
        //Debug.Log(this + "Destroy Bullet");
        // update 코루틴 멈춤
        if(bulletUpdate != null)
        {
            StopCoroutine(bulletUpdate);
        }
        // 삭제 속성 모두 실행
        for (int i = 0; i < info.deletePropertiesLength; i++)
        {
            info.deleteProperties[i].DestroyBullet();
        }
    }
    #endregion


}

/*
// 기본 총알
public class BaseBullet : Bullet
{

}*/
