using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviourSingleton<TextUI>
{
    [HideInInspector] public int count = 0;
    [SerializeField] private Text touch;
    [SerializeField] private Text text;
    [SerializeField] private Image[] focus;
    private bool isHide = true;
    private string str;

    TutorialManager tm;
    TutorialUIManager tu;

    public void Test_Frist(string str)
    {
        this.str = str;
        switch (str)
        {
            case "move":
                switch (count)
                {
                    case 0:
                        text.text = "이동식 조이스틱 입니다!";
                        break;
                    case 1:
                        text.text = "자유롭게 조이스틱으로 이동하세요.";
                        break;
                    case 2:
                        text.text = "앞에 총이 떨어져 있네요!";
                        break;
                    case 3:
                        text.text = "총의 위치까지 이동하여\n총이나 오른쪽 조이스틱 터치!";
                        break;
                    case 4:
                        tu.HoldAll(false);
                        tu.SetLayersActive(0, false);
                        tu.SetLayersActive(1, false);
                        this.gameObject.SetActive(false);
                        count = 0;
                        tu.count++;
                        //TutorialUIManager.Instance.FirstTest();
                        break;
                }
                break;
            case "swap":
                switch (count)
                {
                    case 0:
                        text.text = "축하해요!\n권총을 얻으셨네요!";
                        tu.SetLayersActive(1, true);
                        break;
                    case 1:
                        text.text = "원거리, 근거리 무기를 하나씩 드릴게요!";
                        break;
                    case 2:
                        tm.CallSwapWeapon();
                        tu.SetLayersActive(4, false);
                        count++;
                        break;
                    case 3:
                        text.text = "무기는 최대 3개까지!";
                        break;
                    case 4:
                        text.text = "좌우 스와이프로\n무기를 바꿀 수 있답니다!";
                        break;
                    case 5:
                        text.text = "원거리는 한정된 총알이\n근거리는 스태미나가 닳아요!";
                        break;
                    case 6:
                        count = 0;
                        tu.HoldAll(false);
                        tu.count++;
                        ControllerUI.Instance.WeaponSwitchButton.StartShake(2f, 2f, 1, true);
                        this.gameObject.SetActive(false);
                        break;
                }
                break;
            case "enemy":
                switch (count)
                {
                    case 0:
                        text.text = "이제 공격방식을\n변경하는 법에 대해 알아볼까요?";
                        break;
                    case 1:
                        tu.StartCoroutine("ActiveTrueMenu");
                        tu.SetLayersActive(2, false);
                        count++;
                        this.gameObject.SetActive(false);
                        break;
                    case 2:
                        text.text = "공격방식은 오토, 세미오토, 수동\n세 가지 입니다!";
                        break;
                    case 3:
                        tm.CallEnemy();
                        tu.HoldAll(true);
                        tu.SetLayersActive(0, true);
                        count++;
                        this.gameObject.SetActive(false);
                        break;
                    case 4:
                        text.text = "아앗! 저기에 몬스터가 있어요!";
                        break;
                    case 5:
                        tu.SetFocus(focus[0]);
                        count++;
                        tu.StartCoroutine("StartText");
                        this.gameObject.SetActive(false);
                        break;
                    case 6:
                        text.text = "해골 사원을 향해 공격!";
                        break;
                    case 7:
                        count = 0;
                        tu.HoldAll(false);
                        tu.SetLayersActive(0, false);
                        tu.SetLayersActive(1, false);
                        tu.count++;
                        this.gameObject.SetActive(false);
                        break;

                }
                break;
            case "attack":
                switch (count)
                {
                    case 0:
                        text.text = "아앗! 해골몬스터가\n잠에서 깨어나고 말았어요!";
                        break;
                    case 1:
                        tu.SetFocus(focus[2]);
                        tu.SetFocus(focus[1]);
                        tu.StartCoroutine("StartText");
                        count++;
                        this.gameObject.SetActive(false);
                        count++;
                        break;
                    case 2:
                        text.text = "와! 구르기와 스킬을\n쓸 수 있게 되었어요!";
                        break;
                    case 3:
                        text.text = "구르기를 통해\n몬스터의 공격을 피할 수 있답니다!";
                        break;
                    case 4:
                        text.text = "스킬을 통해\n더 강해질 수 있어요!";
                        break;
                    case 5:
                        count = 0;
                        tm.StartAstar();
                        tu.count++;
                        tu.HoldAll(false);
                        this.gameObject.SetActive(false);
                        tu.SetLayersActive(2, false);
                        tu.SetLayersActive(3, false);

                        tu.FirstTest();
                        break;
                }
                break;
            case "clear":
                switch (count)
                {
                    case 0:
                        text.text = "와우 깔끔한 클리어!";
                        break;
                    case 1:
                        text.text = "튜토리얼은 아쉽지만\n여기까지 입니다!";
                        break;
                    case 2:
                        text.text = "토탈 생기기, 포커스";
                        // count++
                        // this.gameObject.SetActive(false);
                        break;
                    case 3:
                        text.text = "포탈을 누르시면\n다음 스테이지로 이동할 수 있습니다";
                        break;
                    case 4:
                        text.text = "메뉴에서 다시 튜토리얼을\n하실 수 있습니다.";
                        break;
                    case 5:
                        this.gameObject.SetActive(false);
                        tu.HoldAll(false);
                        break;
                }
                break;
        }
    }
    private void Awake()
    {
        tm = TutorialManager.Instance;
        tu = TutorialUIManager.Instance;
    }
    void Update()
    {
        if (isHide)
        {
            Color color = touch.color;
            color.a = color.a - Time.deltaTime;

            if (color.a < 0)
            {
                color.a = 0.0f;
                isHide = false;
            }

            touch.color = color;
        }
        else
        {
            Color color = touch.color;
            color.a = color.a + Time.deltaTime;

            if (color.a > 1)
            {
                color.a = 1.0f;
                isHide = true;
            }
            touch.color = color;
        }
        if (this.gameObject.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                count++;
                Test_Frist(str);
            }
        }
    }

    public void TouchUp()
    {
        count++;
        Test_Frist(str);
    }
}