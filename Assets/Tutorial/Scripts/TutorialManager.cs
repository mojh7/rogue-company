using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviourSingleton<TutorialManager>
{
    [SerializeField]
    public WeaponInfo weapon1, weapon2, weapon3;
    [SerializeField] GameObject portal;
    [SerializeField] Sprite spritePortal;
    GameObject obj;

    private void Start()
    {
        if(Time.timeScale == 0)
            Time.timeScale = 1;

        GameDataManager.Instance.isFirst = true;
        SpawnPlayer();
        DrawUI();
        StartAstar();

        TutorialUIManager.Instance.HoldAll(true);
        TutorialUIManager.Instance.SetFocus(TutorialUIManager.Instance.layers[0]);
    }

    private void Update()
    {
        if (!PlayerManager.Instance.GetPlayer().GetWeaponManager().WeaponEmpty() && TutorialUIManager.Instance.count == 1)
        {
            TutorialUIManager.Instance.ActiveText();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
        }
    }

    public void SetPortal()
    {
        portal.SetActive(true);
        portal.AddComponent<PortalTutorial>().LoadAwake();
        portal.GetComponent<PortalTutorial>().sprites = new Sprite[1] { spritePortal };
        portal.GetComponent<PortalTutorial>().Init();
    }

    public void CallEnemy()
    {
        EnemyData skeleton = EnemyManager.Instance.GetEnemyToTutorial(0);
        obj = EnemyManager.Instance.GenerateObj(new Vector3(7, 12, 0), skeleton, true);

        StartShake(2, 2, 1);

        StartCoroutine(DelayStop());
    }

    public void AwakeEnemy()
    {
        StartCoroutine(DelayPlay());
    }
    private IEnumerator WaitForSpawn()
    {
        do
        {
            yield return null;
        } while (EnemyManager.Instance.GetEnemyList.Count == 0);
    }
    IEnumerator DelayStop()
    {
        yield return WaitForSpawn();

        Enemy enemy = EnemyManager.Instance.GetEnemyList[0];
        CameraController.Instance.FindOther(enemy.GetPosition());
        enemy.GetCharacterComponents().AIController.StopMove();
    }
    IEnumerator DelayPlay()
    {
        Enemy enemy = EnemyManager.Instance.GetEnemyList[0];
        enemy.GetCharacterComponents().AIController.PlayMove();
        yield return YieldInstructionCache.WaitForSeconds(.1f);
        CameraController.Instance.ComeBackPosition();
    }

    public void StartAstar()
    {
        AStar.TileGrid.Instance.BakeTutorial();
        AStar.Pathfinder.Instance.Bake();
    }

    public void CallWeapon()
    {
        Weapon weapon = ObjectPoolManager.Instance.CreateWeapon(weapon1) as Weapon;
        ItemManager.Instance.CreateItem(weapon, new Vector3(7, 8, 0));
    }

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
                TutorialUIManager.Instance.ActiveText();
                yield break;
            }
            else
            {
                cc.transform.position = uiPosition + new Vector3((ShakeTime - counter) * Random.Range(-maxX, maxX), (ShakeTime - counter) * Random.Range(-maxY, maxY));
            }
            yield return null;
        }
    }

    void SpawnPlayer()
    {
        PlayerManager.Instance.FindPlayer(); // 플레이어 스폰
    }

    void DrawUI()
    {
        UIManager.Instance.FadeInScreen(); // 화면 밝히기
    }

}