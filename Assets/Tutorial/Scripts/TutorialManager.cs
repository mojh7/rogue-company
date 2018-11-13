using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviourSingleton<TutorialManager> {
    [SerializeField]
    WeaponInfo weapon1, weapon2, weapon3;
    private void Start()
    {
        SpawnPlayer();
        DrawUI();

        StartCoroutine("First");
    }

    void CallWeapon(int i)
    {
        Item obj = ObjectPoolManager.Instance.CreateWeapon(weapon1) as Item;
        ItemManager.Instance.CreateItem(obj, Vector3.zero,Vector3.zero);
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
        yield return new WaitForSeconds(1.5f);
        TutorialUIManager.Instance.SetFocus(0);
        yield return null;
    }
}
