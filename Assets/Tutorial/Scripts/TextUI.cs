using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviourSingleton<TextUI>
{
    [HideInInspector] public int count = 0;
    [SerializeField] private Text touch;
    [SerializeField] private Text text;
    private bool isHide = true;
    private string str;

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
                        text.text = "총의 위치까지 이동해주세요!";
                        break;
                    case 4:
                        count = 0;
                        TutorialUIManager.Instance.SetLayersActive(0, false);
                        this.gameObject.SetActive(false);
                        break;
                }
                break;
            case "attack":
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
                        text.text = "총의 위치까지 이동해주세요!";
                        break;
                    case 4:
                        count = 0;
                        TutorialUIManager.Instance.SetLayersActive(0, false);
                        this.gameObject.SetActive(false);
                        break;
                }
                break;
        }
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
    }

    public void TouchUp()
    {
        count++;
        Test_Frist(str);
    }
}
