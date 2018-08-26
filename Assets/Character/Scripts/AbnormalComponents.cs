using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbnormalComponents : MonoBehaviour
{
    #region components
    [SerializeField]
    private GameObject poisonEffect;
    [SerializeField]
    private GameObject burnEffect;

    [SerializeField]
    private GameObject nagEffect;
    [SerializeField]
    private GameObject climbingEffect;
    [SerializeField]
    private GameObject graveyardShiftEffect;
    [SerializeField]
    private GameObject freezeEffect;
    [SerializeField]
    private GameObject reactanceEffect;

    [SerializeField]
    private GameObject stunEffect;
    [SerializeField]
    private GameObject charmEffect;
    #endregion

    #region parameter
    public GameObject PoisonEffect
    {
        get
        {
            return poisonEffect;
        }
    }
    public GameObject BurnEffect
    {
        get
        {
            return burnEffect;
        }
    }

    public GameObject NagEffect
    {
        get
        {
            return nagEffect;
        }
    }

    public GameObject ClibmingEffect
    {
        get
        {
            return climbingEffect;
        }
    }
    public GameObject GraveyardShiftEffect
    {
        get
        {
            return graveyardShiftEffect;
        }
    }
    public GameObject FreezeEffect
    {
        get
        {
            return freezeEffect;
        }
    }
    public GameObject ReactanceEffect
    {
        get
        {
            return reactanceEffect;
        }
    }

    public GameObject StunEffect
    {
        get
        {
            return stunEffect;
        }
    }
    public GameObject CharmEffect
    {
        get
        {
            return charmEffect;
        }
    }

    #endregion
}

