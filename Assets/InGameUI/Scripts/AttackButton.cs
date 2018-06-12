using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;


// 공격 버튼.
// 버튼 Down, Up 이벤트에 맞춰서 함수 실행.
public class AttackButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private Sprite attackSprite;
    [SerializeField]
    private Sprite interactSprite;
    private CustomObject interactiveObject;
    private Player player; 

    private bool isAttackTouchDown; // true or false
    //public bool IsAttackTouchDown { get { return isAttackTouchDown; } }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }
    void Awake()
    {
        isAttackTouchDown = false;
    }
    void Update()
    {
        if(isAttackTouchDown)
        {
            if (UIManager.Instance.GetActived())
                return;
            //Debug.Log("버튼 다운으로 인한 공격 시도");
            player.GetWeaponManager().AttackButtonDown();
        }
        else
        {
            CustomObject olderInteractiveObject = interactiveObject;
            interactiveObject = player.Interact();
            if (interactiveObject == null)
            {
                if(olderInteractiveObject != null)
                    olderInteractiveObject.DeIndicateInfo();
                ChangeImage(attackSprite);
            }
            else
            {
                interactiveObject.IndicateInfo();
                ChangeImage(interactSprite);
            }
        }
    }
    void ChangeImage(Sprite sprite)
    {
        image.sprite = sprite;
    }
    // 터치 했을 때
    public void OnPointerDown(PointerEventData ped)
    {
        //Debug.Log("Attack Touch Down");
        // 일반 공격 : 계속 공격
        // 차징 공격 : 차징 중
        //isAttackTouchDown = true;
        //Debug.Log("공격 버튼 다운");
        if (interactiveObject != null)
        {
            interactiveObject.Active();
        }
        else
        {
            isAttackTouchDown = true;
        }
    }

    // 땠을 때
    public void OnPointerUp(PointerEventData ped)
    {
        //Debug.Log("Attack Touch Up");
        // 일반 공격 : 공격 중지
        // 차징 공격 : 차징된 공격
        if (isAttackTouchDown)
        {
            player.GetWeaponManager().AttackButtonUP();
            isAttackTouchDown = false;
        }
        //Debug.Log("공격 버튼 업");
    }
}
