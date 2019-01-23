using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingClass : MonoBehaviour {
    
    public void SettingPlayerAim(int i)
    {
        PlayerManager.Instance.GetPlayer().SetAimType((CharacterInfo.AimType)i);
        //switch (i)
        //{
        //    case 0:
        //        PlayerManager.Instance.GetPlayer().SetAutoAim();
        //        break;
        //    case 1:
        //        PlayerManager.Instance.GetPlayer().SetSemiAutoAim();
        //        break;
        //    case 2:
        //        PlayerManager.Instance.GetPlayer().SetManualAim();
        //        break;
        //    default:
        //        PlayerManager.Instance.GetPlayer().SetAutoAim();
        //        break;
        //}
    }
}
