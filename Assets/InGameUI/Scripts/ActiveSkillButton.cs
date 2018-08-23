using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class ActiveSkillButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Image fireImage;
    private Character character;

    public void SetPlayer(Character character)
    {
        this.character = character;
    }
    // 터치 했을 때
    public void OnPointerDown(PointerEventData ped)
    {
        if (UIManager.Instance.GetActived())
            return;
        character.ActiveSkill();
    }
}
