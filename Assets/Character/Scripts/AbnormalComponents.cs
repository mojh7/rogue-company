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
    private GameObject freezeEffect;

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

    public GameObject FreezeEffect
    {
        get
        {
            return freezeEffect;
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

