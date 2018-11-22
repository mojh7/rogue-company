using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviourSingleton<TutorialManager>
{
    [SerializeField]
    public WeaponInfo weapon1, weapon2, weapon3;
    [SerializeField] GameObject portal;
    GameObject obj;

    public bool isis = false;

    private void Start()
    {
        SpawnPlayer();
        DrawUI();

        CallWeapon();

        TutorialUIManager.Instance.HoldAll(true);
        TutorialUIManager.Instance.SetFocus(TutorialUIManager.Instance.layers[0]);
    }

    private void Update()
    {
        if (!PlayerManager.Instance.GetPlayer().GetWeaponManager().WeaponEmpty() && TutorialUIManager.Instance.count == 1)
        {
            TutorialUIManager.Instance.FirstTest();
        }
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    AStar.TileGrid.Instance.BakeTutorial();
        //    AStar.Pathfinder.Instance.Bake();
        //    //CallSwapWeapon();
        //}
        if (isis && TutorialUIManager.Instance.count == 4)
        {
            portal.SetActive(true);
            TutorialUIManager.Instance.FirstTest();
        }
    }

    public void CallEnemy()
    {
        EnemyData skeleton = EnemyManager.Instance.GetEnemyToTutorial(5);
        obj = EnemyManager.Instance.GenerateObj(new Vector3(0, 3.5f, 0), skeleton);

        StartShake(2, 2, 1);

        //obj.GetComponent<Enemy>().GetCharacterComponents().AIController.StopMove();
        //obj.GetComponent<Enemy>().GetCharacterComponents().AIController.StopAttack();
    }

    public void StartAstar()
    {
        AStar.TileGrid.Instance.BakeTutorial();
        AStar.Pathfinder.Instance.Bake();
    }

    void CallWeapon()
    {
        Weapon weapon = ObjectPoolManager.Instance.CreateWeapon(weapon1) as Weapon;
        ItemManager.Instance.CreateItem(weapon, new Vector3(0, 1.5f, 0));
    }

    void SpawnPlayer()
    {
        PlayerManager.Instance.FindPlayer(); // 플레이어 스폰
    }

    void DrawUI()
    {
        UIManager.Instance.FadeInScreen(); // 화면 밝히기
    }

    public void StartShake(float maxX, float maxY, float ShakeTime)
    {
        StartCoroutine(CameraMove(maxX, maxY, ShakeTime));
    }

    IEnumerator CameraMove(float maxX, float maxY, float ShakeTime)
    {
        GameObject player = GameObject.Find("Player(Clone)");
        GameObject cc = player.transform.GetChild(5).gameObject;
        float counter = 0f;
        Vector3 uiPosition = cc.transform.position;
        while (true)
        {
            counter += Time.deltaTime;
            if (counter >= ShakeTime)
            {
                TutorialUIManager.Instance.FirstTest();
                yield break;
            }
            else
            {
                cc.transform.position = uiPosition + new Vector3((ShakeTime - counter) * Random.Range(-maxX, maxX), (ShakeTime - counter) * Random.Range(-maxY, maxY));
            }
            yield return null;
        }
    }

    // 테스트용
    public void CallSwapWeapon()
    {
        CallWeapon(weapon2);
        CallWeapon(weapon3);
    }

    public void CallWeapon(WeaponInfo weapon)
    {
        Item tutorialWeapon = ObjectPoolManager.Instance.CreateWeapon(weapon) as Weapon;
        PlayerManager.Instance.GetPlayer().GetWeaponManager().TutorialWeapon(tutorialWeapon);
    }
}