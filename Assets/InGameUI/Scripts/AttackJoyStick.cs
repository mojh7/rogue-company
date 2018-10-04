using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackJoyStick : Joystick
{
    [SerializeField]
    private Sprite attackSprite;
    [SerializeField]
    private Sprite interactSprite;
    private CustomObject interactiveObject;
    private CustomObject olderInteractiveObject;
    private PointerEventData pointerEventData;

    private void Update()
    {
        if (isTouchDown)
        {
            if (UIManager.Instance.GetActived() || character.IsEvade())
                return;
            character.SetAim();
            character.GetWeaponManager().AttackButtonDown();
        }
        else
        {
            olderInteractiveObject = interactiveObject;
            if (olderInteractiveObject != null)
                olderInteractiveObject.DeIndicateInfo();
            interactiveObject = character.Interact();
            if (interactiveObject == null)
            {
                joystickImage.sprite = attackSprite;
            }
            else
            {
                interactiveObject.IndicateInfo();
                joystickImage.sprite = interactSprite;
            }
        }
    }

    public override void OnDrag(PointerEventData ped)
    {
        if (character.IsEvade() || !isTouchDown)
            return;
        base.OnDrag(ped);
    }

    public override void OnPointerDown(PointerEventData ped)
    {
        if (interactiveObject != null)
        {
            if (character.IsEvade())
                return;
            interactiveObject.Active();
        }
        else
        {
            if (character.IsEvade())
                return;
            base.OnPointerDown(ped);
        }
    }

    public override void OnPointerUp(PointerEventData ped)
    {
        if (!isTouchDown)
            return;
        base.OnPointerUp(ped);
        character.GetWeaponManager().AttackButtonUP();
    }
}
