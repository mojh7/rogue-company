using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviourSingleton<TutorialManager> {
    [SerializeField]
    WeaponInfo weapon1, weapon2, weapon3;

    public bool isis = false;

    private void Start()
    {
        SpawnPlayer();
        DrawUI();

        CallWeapon();
        CallEnemy();

        TutorialUIManager.Instance.HoldAll(true);
        TutorialUIManager.Instance.SetFocusImage(TutorialUIManager.Instance.layers[0]);
    }

    private void Update()
    {
        //if (!PlayerManager.Instance.GetPlayer().GetWeaponManager().WeaponEmpty() && TutorialUIManager.Instance.count == 1)
        //{
        //    TutorialUIManager.Instance.FirstTest();
        //}
        if (Input.GetKeyDown(KeyCode.X))
        {
            AStar.TileGrid.Instance.BakeTutorial();
            AStar.Pathfinder.Instance.Bake();
        }
        if (isis)
        {
            //StartCoroutine(CameraMove());
        }
    }

    void CallEnemy()
    {
        EnemyData skeleton = EnemyManager.Instance.GetEnemyToTutorial(5);
        GameObject obj = EnemyManager.Instance.GenerateObj(new Vector3(0, 3.5f, 0), skeleton);

        //AStar.TileGrid.Instance.BakeTutorial();
        //AStar.Pathfinder.Instance.Bake();

        //obj.GetComponent<Enemy>().GetCharacterComponents().AIController.StopMove();
        //obj.GetComponent<Enemy>().GetCharacterComponents().AIController.StopAttack();
    }

    void CallWeapon()
    {
        Weapon weapon = ObjectPoolManager.Instance.CreateWeapon(weapon1) as Weapon;
        Debug.Log(weapon.name);
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

    IEnumerator CameraMove()
    {
        GameObject cc = GameObject.Find("Player(Clone)").transform.GetChild(5).gameObject;
        //Vector3 v2 = new Vector3(cc.transform.position.x, cc.transform.position.y, cc.transform.position.z);
        Vector3 v = new Vector3(cc.transform.position.x, 3, cc.transform.position.z);
        cc.transform.position = Vector3.Lerp(cc.transform.position, v, Time.deltaTime);
        yield return new WaitForSeconds(5f);
        cc.transform.position = Vector3.zero;
        isis = false;
    }

    // 테스트용
    public void TestSpriteWeapon()
    {
        ControllerUI.Instance.WeaponSwitchButton.UpdateNextWeaponSprite(weapon2.sprite);
        ControllerUI.Instance.WeaponSwitchButton.UpdatePrevWeaponSprite(weapon3.sprite);
    }
}
