using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterInfo;


/* CollisionProperty class
 * 총알 충돌에 관련된 클래스
 * [현재]
 *  # 충돌 클래스와 삭제 클래스 분리
 *   - 충돌 후 총알이 삭제 되었을 때 내용이 충돌 클래스에 포함되어 있었으나
 *     따로 DeleteProperty를 만들어서 분리 시킴. (DelDestroyBullet)델리게이트로 bullet class에 삭제 함수를 받아서 실행.
 * 
 * 1. BaseNormalCollisionProperty
 *  - 투사체 총알 전용 충돌 속성(공격, 관통, 바운스 처리 한 번에 함)
 * 2. LaserCollisionProperty
 *  - 레이저 전용 충돌 속성, 레이저 update 속성에서 raycast 검사를 해서 나온 collider에 대해서 공격 처리 같은 충돌 처리를 담당함.
 * -------------------
 * [예정]
 * 굳이 Properties 안하고 Collision속성은 위에 속성 N개 중 1개 택하여 1개만 써도 될듯. 
 * 
 * [미정]
 * 1.
 */


public abstract class CollisionProperty : BulletProperty
{
    // 관통 가능한 오브젝트에 대한 관통 처리 가능 횟수
    protected int pierceCount = 1;    // default 1
    // 바운스 가능한 오브젝트에 대해서 총알이 반사각으로 튕겨나올 수 있는 횟수
    protected int bounceCount = 0;    // default 0

    protected Collider2D bulletCollider2D;
    public abstract CollisionProperty Clone();
    public abstract void Collision(ref Collision2D coll);
    public abstract void Collision(ref Collider2D coll);

    // 충돌, 반사, 공격 일반 투사체 총알이랑 레이저 따로 처리 해야 되니 함수 구조나 내용 바뀔 수 있음.
    protected virtual void Bounce(ref Collision2D coll) { }
    protected void Ignore(ref Collision2D coll)
    {
        //Physics2D.IgnoreCollision(coll.gameObject.GetComponent<Collider2D>(), bulletCollider2D);
    }
    protected void Ignore(ref Collider2D coll)
    {
        //Physics2D.IgnoreCollision(coll, bulletCollider2D);
    }
    protected virtual void Attack(ref Collision2D coll)
    {
        coll.gameObject.GetComponent<Character>().Attacked(transferBulletInfo);
    }
    protected virtual void Attack(ref Collider2D coll)
    {
        coll.gameObject.GetComponent<Character>().Attacked(transferBulletInfo);
    }
    protected virtual void AffectStatusEffect(ref Collision2D coll)
    {
        statusEffectInfo.BulletPos = bullet.transform.position;
        statusEffectInfo.BulletDir = bullet.GetDirVector();
        coll.gameObject.GetComponent<Character>().ApplyStatusEffect(statusEffectInfo);
    }
    protected virtual void AffectStatusEffect(ref Collider2D coll)
    {
        statusEffectInfo.BulletPos = bullet.transform.position;
        statusEffectInfo.BulletDir = bullet.GetDirVector();
        coll.gameObject.GetComponent<Character>().ApplyStatusEffect(statusEffectInfo);
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        transferBulletInfo.damage = bullet.info.damage;
        transferBulletInfo.criticalChance = bullet.info.criticalChance;
        bulletCollider2D = bullet.boxCollider;
    }
}

// 이름 바꿔야 될 것 같긴 한데, 사실상 투사체 총알 충돌 처리 = BaseNormalCollsion, 레이저 총알 처리 = LaserCollision


// 클래스 내부 내용 더 바꿀 수 있을 듯
// 총알 에서 판단할 여부
// 관통 o/x, 바운스 o/x, 사라지는 여부 공격 횟수 or 시간
// 대상에 따른 구분
// 관통 o/x, 공격 o/x, 바운스 o/x

// Player, 몬스터 - 관통 처리, 공격 o
// 벽 - 바운스 처리, 공격 x
// 내구도 있는 맵 내 오브젝트 - 바운스 처리, 공격 o


// 일반 충돌(투사체 총알)
class BaseNormalCollisionProperty : CollisionProperty
{
    private Vector3 reflectVector;
    private float decreaseDamageAfterPierce;
    private bool canDecreaseDamageAfterPierce;

    public override CollisionProperty Clone()
    {
        return new BaseNormalCollisionProperty();
    }

    private void DecreaseDamageAfterPierce()
    {
        if (false == canDecreaseDamageAfterPierce || OwnerType.Player != bullet.GetOwnerType()) return;
        transferBulletInfo.damage = transferBulletInfo.damage * (1.0f - decreaseDamageAfterPierce);
        canDecreaseDamageAfterPierce = false;
    }

    // collision
    public override void Collision(ref Collision2D coll)
    {
        if (OwnerType.Enemy == bullet.GetOwnerType())
        {
            // PlayerCanReflectBullet = 19, PlayerCanBlockBullet = 18
            // owenr = player bullet되고 왔던 방향과 반대 방향(원점 대칭)으로 반사
            if (UtilityClass.CheckLayer(coll.gameObject.layer, 19))
            {
                colliderObj.layer = LayerMask.NameToLayer("PlayerBullet");
                bullet.SetOwnerType(OwnerType.Player);
                bullet.RotateDirection(180);
            }
            else if (UtilityClass.CheckLayer(coll.gameObject.layer, 18))
            {
                delDestroyBullet();
            }
            
            // Enemy가 Player(16) 공격 : 관통 처리 o, 공격 o
            else if (UtilityClass.CheckLayer(coll.gameObject.layer, 16))
            {
                if (pierceCount > 0)
                {
                    AffectStatusEffect(ref coll);
                    Attack(ref coll);
                    Ignore(ref coll);
                    pierceCount -= 1;
                    if (pierceCount == 0)
                        delDestroyBullet();
                    DecreaseDamageAfterPierce();
                }
                return;
            }
        }
        else if (OwnerType.Player == bullet.GetOwnerType())
        {
            // EnemyCanReflectBullet = 21, EnemyCanBlockBullet = 20
            // owenr = enemy bullet되고 왔던 방향과 반대 방향(원점 대칭)으로 반사
            if (UtilityClass.CheckLayer(coll.gameObject.layer, 21))
            {
                colliderObj.layer = LayerMask.NameToLayer("EnemyBullet");
                bullet.SetOwnerType(OwnerType.Enemy);
                bullet.RotateDirection(180);
            }
            else if (UtilityClass.CheckLayer(coll.gameObject.layer, 20))
            {
                delDestroyBullet();
            }
            // Player(16)가 Enemy(13) 공격 : 관통 처리 o, 공격 o
            else if (UtilityClass.CheckLayer(coll.gameObject.layer, 13))
            {
                if (pierceCount > 0)
                {
                    AffectStatusEffect(ref coll);
                    Attack(ref coll);
                    Ignore(ref coll);
                    pierceCount -= 1;
                    if (pierceCount == 0)
                        delDestroyBullet();
                    DecreaseDamageAfterPierce();
                }
                return;
            }
        }
        
        // owner 상관 없는 처리이고 바운스 처리 o, 공격 x (ex : 벽 14, TransparentFX 1)
        if (UtilityClass.CheckLayer(coll.gameObject.layer, 1, 14))
        {
            Bounce(ref coll);
        }
    }

    // trigger
    public override void Collision(ref Collider2D coll)
    {
        if (OwnerType.Enemy == bullet.GetOwnerType())
        {
            // PlayerCanReflectBullet = 19, PlayerCanBlockBullet = 18
            // owenr = player bullet되고 왔던 방향과 반대 방향(원점 대칭)으로 반사
            if (UtilityClass.CheckLayer(coll.gameObject.layer, 19))
            {
                colliderObj.layer = LayerMask.NameToLayer("PlayerBullet");
                bullet.SetOwnerType(OwnerType.Player);
                bullet.RotateDirection(180);
            }
            else if (UtilityClass.CheckLayer(coll.gameObject.layer, 18))
            {
                delDestroyBullet();
            }
            // Enemy가 Player공격 : 관통 처리 o, 공격 o
            else if (UtilityClass.CheckLayer(coll.gameObject.layer, 16))
            {
                if (pierceCount > 0)
                {
                    AffectStatusEffect(ref coll);
                    Attack(ref coll);
                    Ignore(ref coll);
                    pierceCount -= 1;
                    if (pierceCount == 0)
                        delDestroyBullet();
                    DecreaseDamageAfterPierce();
                }
                return;
            }
        }
        else if (OwnerType.Player == bullet.GetOwnerType())
        {
            // EnemyCanReflectBullet = 21, EnemyCanBlockBullet = 20
            // owenr = enemy bullet되고 왔던 방향과 반대 방향(원점 대칭)으로 반사
            if (UtilityClass.CheckLayer(coll.gameObject.layer, 21))
            {
                colliderObj.layer = LayerMask.NameToLayer("EnemyBullet");
                bullet.SetOwnerType(OwnerType.Enemy);
                bullet.RotateDirection(180);
            }
            else if (UtilityClass.CheckLayer(coll.gameObject.layer, 20))
            {
                delDestroyBullet();
            }
            // Player(16)가 Enemy(13) 공격 : 관통 처리 o, 공격 o
            else if (UtilityClass.CheckLayer(coll.gameObject.layer, 13))
            {
                if(pierceCount > 0)
                {
                    AffectStatusEffect(ref coll);
                    Attack(ref coll);
                    Ignore(ref coll);
                    pierceCount -= 1;
                    if (pierceCount == 0)
                        delDestroyBullet();
                    DecreaseDamageAfterPierce();
                }
                return;
            }
        }
        
        // owner 상관 없는 처리이고 바운스 처리 o, 공격 x (TransparentFx 1, wall 14)
        if (UtilityClass.CheckLayer(coll.gameObject.layer, 1, 14))
        {
            delDestroyBullet();
        }
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        reflectVector = new Vector3();

        canDecreaseDamageAfterPierce = true;
        decreaseDamageAfterPierce = bullet.DecreaseDamageAfterPierce;
        pierceCount = bullet.info.pierceCount;
        bounceCount = bullet.info.bounceCount;
    }

    protected override void Bounce(ref Collision2D coll)
    {
        // bounce 가능 횟수가 남아있으면 총알을 반사각으로 튕겨내고 없으면 delete 처리
        if (bullet.info.bounceAble && bounceCount > 0)
        {
            reflectVector = Vector3.Reflect(MathCalculator.VectorRotate(Vector3.right, bulletTransform.rotation.eulerAngles.z), coll.contacts[0].normal);
            bullet.SetDirection(reflectVector);
            bounceCount -= 1;
            ///TestScript.Instance.CreateContactObj(coll.contacts[0].point); // 디버그용 contact 위치 표시
        }
        else
        {
            // 총알 회수
            delDestroyBullet();
        }
    }
}

// 레이저 전용 충돌
class LaserCollisionProperty : CollisionProperty
{
    public override CollisionProperty Clone()
    {
        return new LaserCollisionProperty();
    }

    public override void Collision(ref Collision2D coll)
    {
        // coll.attack
    }

    public override void Collision(ref Collider2D coll)
    {
        //enemy layer 13
        if (OwnerType.Player == bullet.GetOwnerType() && UtilityClass.CheckLayer(coll.gameObject.layer, 13))
        {
            AffectStatusEffect(ref coll);
            Attack(ref coll);
        }
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
        transferBulletInfo.damage = bullet.info.damage * Time.fixedDeltaTime;
    }

    protected override void Bounce(ref Collision2D coll)
    {
        throw new System.NotImplementedException();
    }
}

// 삭제되지 않은 충돌 속성, lifeTime에 의한 삭제만 이루어짐
class UndeletedCollisionProperty : CollisionProperty
{
    public override CollisionProperty Clone()
    {
        return new UndeletedCollisionProperty();
    }

    // collision
    public override void Collision(ref Collision2D coll)
    {
        // player layer 16
        if (OwnerType.Enemy == bullet.GetOwnerType() && UtilityClass.CheckLayer(coll.gameObject.layer, 16))
        {
            // Enemy가 Player 공격 : 관통 처리 o, 공격 o
            AffectStatusEffect(ref coll);
            Attack(ref coll);
            Ignore(ref coll);
        }
        // enemy layer 13
        else if (OwnerType.Player == bullet.GetOwnerType() && UtilityClass.CheckLayer(coll.gameObject.layer, 13))
        {
            AffectStatusEffect(ref coll);
            Attack(ref coll);
            Ignore(ref coll);
        }
    }

    // trigger
    public override void Collision(ref Collider2D coll)
    {
        if (OwnerType.Enemy == bullet.GetOwnerType() && UtilityClass.CheckLayer(coll.gameObject.layer, 16))
        {
            // Enemy가 Player 공격 : 관통 처리 o, 공격 o
            AffectStatusEffect(ref coll);
            Attack(ref coll);
            Ignore(ref coll);
        }
        else if (OwnerType.Player == bullet.GetOwnerType() && UtilityClass.CheckLayer(coll.gameObject.layer, 13))
        {
            AffectStatusEffect(ref coll);
            Attack(ref coll);
            Ignore(ref coll);
        }
    }

    public override void Init(Bullet bullet)
    {
        base.Init(bullet);
    }
}