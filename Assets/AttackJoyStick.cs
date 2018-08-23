using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackJoyStick : Joystick
{
    [SerializeField]
    private Sprite attackSprite;
    [SerializeField]
    private Sprite interactSprite;
    private Character character;
    private CustomObject interactiveObject;
    private CustomObject olderInteractiveObject;
    private bool isAttackTouchDown = false;
    private float currentTime = 0;

    public bool GetAttackDown()
    {
        return isAttackTouchDown;
    }

    private void Update()
    {
        if (isAttackTouchDown)
        {
            if (UIManager.Instance.GetActived())
                return;
            character.GetWeaponManager().AttackButtonDown();
        }
        else
        {
            olderInteractiveObject = interactiveObject;
            interactiveObject = character.Interact();
            if (interactiveObject == null)
            {
                if (olderInteractiveObject != null)
                    olderInteractiveObject.DeIndicateInfo();
                joystickImage.sprite = attackSprite;
            }
            else
            {
                interactiveObject.IndicateInfo();
                joystickImage.sprite = interactSprite;
            }
        }
    }

    public override void OnPointerDown(PointerEventData ped)
    {
        base.OnPointerDown(ped);
        if (interactiveObject != null)
        {
            interactiveObject.Active();
        }
        else
        {
            isAttackTouchDown = true;
        }
    }

    public override void OnPointerUp(PointerEventData ped)
    {
        base.OnPointerUp(ped);
        if (isAttackTouchDown)
        {
            character.GetWeaponManager().AttackButtonUP();
            isAttackTouchDown = false;
        }
    }

    public void SetPlayer(Character character)
    {
        this.character = character;
    }
}
