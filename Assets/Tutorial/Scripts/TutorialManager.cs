using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviourSingleton<TutorialManager> {
    [SerializeField]
    WeaponInfo weapon1, weapon2, weapon3;

    private void Start()
    {
        SpawnPlayer();
        DrawUI();

        // 테스트용
        ControllerUI.Instance.WeaponSwitchButton.UpdateWeaponSprite(weapon1.sprite);

        TutorialUIManager.Instance.HoldAll(true);

        StartCoroutine("First");
    }

    private void Update()
    {
        if (TutorialUIManager.Instance.count == 2 && TextUI.Instance.count <= 1)
        {
            TutorialUIManager.Instance.FirstTest();
        }
    }

    void CallWeapon()
    {
        Weapon weapon = ObjectPoolManager.Instance.CreateWeapon(weapon1) as Weapon;
        Debug.Log(weapon.name);
        ItemManager.Instance.CreateItem(weapon, Vector3.zero, Vector3.zero);
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
