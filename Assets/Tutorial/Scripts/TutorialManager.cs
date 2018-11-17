using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviourSingleton<TutorialManager> {
    [SerializeField]
    WeaponInfo weapon1, weapon2, weapon3;

    private void Start()
    {
        SpawnPlayer();
        DrawUI();

        CallWeapon();
        CallEnemy();
        TutorialUIManager.Instance.HoldAll(true);

        StartCoroutine("First");
    }

    private void Update()
    {
        if (!PlayerManager.Instance.GetPlayer().GetWeaponManager().WeaponEmpty() && TutorialUIManager.Instance.count == 1)
        {
            TutorialUIManager.Instance.FirstTest();
        }
    }

    void CallEnemy()
    {
        EnemyData skeleton = EnemyManager.Instance.GetEnemyToTutorial(5);
        GameObject obj = EnemyManager.Instance.GenerateObj(new Vector3(0, 3, 0), skeleton);

        AStar.TileGrid.Instance.BakeTutorial();
        AStar.Pathfinder.Instance.Bake();

        obj.GetComponent<Enemy>().GetCharacterComponents().AIController.StopMove();
        obj.GetComponent<Enemy>().GetCharacterComponents().AIController.StopAttack();
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

    IEnumerator First()
    {
        yield return new WaitForSeconds(1f);
        TutorialUIManager.Instance.SetFocus(0);
        yield return null;
    }

    // 테스트용
    public void TestSpriteWeapon()
    {
        ControllerUI.Instance.WeaponSwitchButton.UpdateNextWeaponSprite(weapon2.sprite);
        ControllerUI.Instance.WeaponSwitchButton.UpdatePrevWeaponSprite(weapon3.sprite);
    }
}
