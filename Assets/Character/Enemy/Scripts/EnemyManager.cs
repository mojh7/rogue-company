using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviourSingleton<EnemyManager>
{
    [System.Serializable]
    struct FloorData
    {
        public EnemyData[] enemyDatas;
        public EnemyData[] bossEnemyDatas;
    }

    private Sprite sprite;
    [SerializeField]
    private FloorData[] floorDatas;
    [SerializeField]
    private ObjectPool enemyPool;
    [SerializeField]
    private ObjectPool indicatorPool;
    private List<Enemy> enemyList;
    private List<BoxCollider2D> enemyColliderList;
    Vector3 zero = Vector3.zero;

    private int bossIdx;
    private int floor;
    private EnemyData bossData;
    // 0516 모장현
    private int aliveEnemyTotal;
    // 1123 윤아
    private bool tutorial;

    private void Start()
    {
        enemyList = new List<Enemy>();
        enemyColliderList = new List<BoxCollider2D>();
        aliveEnemyTotal = 0;
    }

    public void Generate(EnemyData enemyData, Character owner)
    {
        Vector3 pos = zero;

        switch (enemyData.SpawnPos)
        {
            default:
            case EnemyData.SpawnPosition.RANDOM:
                pos = RoomManager.Instance.GetCurrentRoomAvailableArea();
                break;
            case EnemyData.SpawnPosition.MID:
                pos = RoomManager.Instance.MidPos;
                break;
            case EnemyData.SpawnPosition.TOP:
                pos = RoomManager.Instance.TopPos;
                break;
            case EnemyData.SpawnPosition.BOTTOM:
                pos = RoomManager.Instance.BottomPos;
                break;
            case EnemyData.SpawnPosition.LEFT:
                pos = RoomManager.Instance.LeftPos;
                break;
            case EnemyData.SpawnPosition.RIGHT:
                pos = RoomManager.Instance.RightPos;
                break;
        }
        GameObject obj = ResourceManager.Instance.objectPool.GetPooledObject();
        obj.transform.position = pos;
        obj.AddComponent<Alert>();
        obj.GetComponent<Alert>().Init(CallBack, enemyData, 0, 0, owner);
        obj.GetComponent<Alert>().Active();
    }

    public void Generate(EnemyData enemyData)
    {
        Vector3 pos = zero;

        switch (enemyData.SpawnPos)
        {
            default:
            case EnemyData.SpawnPosition.RANDOM:
                pos = RoomManager.Instance.GetCurrentRoomAvailableArea();
                break;
            case EnemyData.SpawnPosition.MID:
                pos = RoomManager.Instance.MidPos;
                break;
            case EnemyData.SpawnPosition.TOP:
                pos = RoomManager.Instance.TopPos;
                break;
            case EnemyData.SpawnPosition.BOTTOM:
                pos = RoomManager.Instance.BottomPos;
                break;
            case EnemyData.SpawnPosition.LEFT:
                pos = RoomManager.Instance.LeftPos;
                break;
            case EnemyData.SpawnPosition.RIGHT:
                pos = RoomManager.Instance.RightPos;
                break;
        }

        GameObject obj = ResourceManager.Instance.objectPool.GetPooledObject();
        obj.transform.position = pos;
        obj.AddComponent<Alert>();
        obj.GetComponent<Alert>().Init(CallBack, enemyData, 0, 0, null);
        obj.GetComponent<Alert>().Active();
    }

    public GameObject GenerateObj(Vector3 _position, EnemyData enemyData, bool tutorial)
    {
        GameObject obj = ResourceManager.Instance.objectPool.GetPooledObject();
        obj.transform.position = _position;
        obj.AddComponent<Alert>();
        obj.GetComponent<Alert>().Init(CallBack, enemyData, 0, 0, null);
        obj.GetComponent<Alert>().Active();
        this.tutorial = tutorial;
        return obj;
    }

    public void CreateBossData()
    {
        floor = InGameManager.Instance.GetFloor();
        if (floor >= floorDatas.Length)
        {
            floor = floorDatas.Length - 1;
        }
        bossIdx = Random.Range(0, floorDatas[floor].bossEnemyDatas.Length);
    }

    public Sprite GetBossSprite()
    {
        sprite = floorDatas[floor].bossEnemyDatas[bossIdx].Sprite;
        return sprite;
    }

    public string GetBossName()
    {
        return floorDatas[floor].bossEnemyDatas[bossIdx].Name;
    }

    GameObject SpawnEnemy(Vector3 position)
    {
        GameObject obj = enemyPool.GetPooledObject();
        obj.transform.position = position;
        obj.transform.localScale = new Vector3(1, 1, 0);

        return obj;
    }

    public void SpawnBoss(int _floor)
    {
        aliveEnemyTotal += 1;
        BossEnemy enemy;
        EnemyData bossData = GetEnemy(true);
        Vector3 pos = zero;

        switch (bossData.SpawnPos)
        {
            default:
            case EnemyData.SpawnPosition.RANDOM:
                pos = RoomManager.Instance.GetCurrentRoomAvailableArea();
                break;
            case EnemyData.SpawnPosition.MID:
                pos = RoomManager.Instance.MidPos;
                break;
            case EnemyData.SpawnPosition.TOP:
                pos = RoomManager.Instance.TopPos;
                break;
            case EnemyData.SpawnPosition.BOTTOM:
                pos = RoomManager.Instance.BottomPos;
                break;
            case EnemyData.SpawnPosition.LEFT:
                pos = RoomManager.Instance.LeftPos;
                break;
            case EnemyData.SpawnPosition.RIGHT:
                pos = RoomManager.Instance.RightPos;
                break;
        }

        GameObject obj = SpawnEnemy(pos);
        enemy = obj.AddComponent<BossEnemy>();

        UIManager.Instance.bossHPUI.Toggle();
        UIManager.Instance.bossHPUI.Init(enemy);
        UIManager.Instance.bossHPUI.Init(enemy);

        enemy.Init(bossData);
        enemyList.Add(enemy);
        CharacterIndicator indicator = indicatorPool.GetPooledObject().GetComponent<CharacterIndicator>();
        indicator.SetTarget(enemy);
        enemyColliderList.Add(enemy.GetHitBox());
    }

    void CallBack(Vector3 position, object temporary, float amount, Character owner)
    {
        Enemy enemy;
        GameObject obj = SpawnEnemy(position);

        enemy = obj.AddComponent<Enemy>();
        if (this.tutorial)
        {
            enemy.SetTutorial(tutorial);
        }
        enemy.Init(temporary as EnemyData);
        if (owner == null)
        {
            aliveEnemyTotal += 1;
            enemyList.Add(enemy);
        }
        else
        {
            owner.SpawnServant(enemy);
            if((temporary as EnemyData).FollowBoss)
                enemy.GetCharacterComponents().AIController.ChangeTarget(owner);
        }
        CharacterIndicator indicator = indicatorPool.GetPooledObject().GetComponent<CharacterIndicator>();
        indicator.SetTarget(enemy);
        enemyColliderList.Add(enemy.GetHitBox());
    }

    public EnemyData GetEnemy(bool isBoss)
    {
        floor = InGameManager.Instance.GetFloor();
        if (floor >= floorDatas.Length)
        {
            floor = floorDatas.Length - 1;
        }
        if (isBoss)
        {
            return floorDatas[floor].bossEnemyDatas[bossIdx];
        }
        else
        {
            int rand = Random.Range(0, floorDatas[floor].enemyDatas.Length);

            return floorDatas[floor].enemyDatas[rand];
        }
    }

    public EnemyData GetEnemyToTutorial(int i)
    {
        return floorDatas[0].enemyDatas[i];
    }

    public List<Enemy> GetEnemyList
    {
        get
        {
            if (enemyList == null)
                return null;
            return enemyList;
        }
    }

    public List<BoxCollider2D> GetEnemyColliderList
    {
        get
        {
            if (enemyColliderList == null)
                return null;
            return enemyColliderList;
        }
    }

    public void DeleteEnemy(Enemy _enemy)
    {
        if (enemyList == null)
            return;
        aliveEnemyTotal -= 1;
        enemyColliderList.Remove(_enemy.GetHitBox());
        enemyList.Remove(_enemy);
    }

    // 0516 모장현
    public int GetAliveEnemyTotal()
    {
        return aliveEnemyTotal;
    }
}
