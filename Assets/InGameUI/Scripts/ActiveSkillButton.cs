using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class ActiveSkillButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Image fireImage;
    private Player player;

    //public bool IsAttackTouchDown { get { return isAttackTouchDown; } }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }
    // 터치 했을 때
    public void OnPointerDown(PointerEventData ped)
    {
        if (UIManager.Instance.GetActived())
            return;
        player.ActiveSkill();
    }
}
