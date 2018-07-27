using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    static public GameManager Gm;

    void Awake()
    {
        Gm = this;

    }
	// Use this for initialization
	void Start () {

       
        m_Anim = this.transform.Find("model").GetComponent<Animator>();
        
    }

    public int EffectID =23;
    public int CurrentID = 1;


    //애니메이션
    public Animator m_Anim;

    //UI
    public Text Text_Anim;
    public Text Text_Count;
    //이펙트
    public Image[] UI_Image_Arrow;


    // Update is called once per frame

    void Update () {

     

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            InitEffect();
          
            
            Color myColor = new Color32(180, 180, 180, 255);

            UI_Image_Arrow[1].color = myColor;

            if (CurrentID >= EffectID)
            {
                CurrentID = 1;
                Text_Count.text = CurrentID + "/26";
                return;
            }
           

            CurrentID++;
            Text_Count.text = CurrentID + "/26";
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            InitEffect();
            Color myColor = new Color32(180, 180, 180, 255);

            UI_Image_Arrow[0].color = myColor;

            if (CurrentID <= 1)
            {
                CurrentID = EffectID;
                Text_Count.text = CurrentID + "/26";
                return;
            }
       

            CurrentID--;
            Text_Count.text = CurrentID + "/26";
        }


        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            InitColor();
        }

        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            InitColor();
        }




        ChangeAnim();


    }

    void InitColor()
    {

        for (int i = 0; i < UI_Image_Arrow.Length; i++)
        {
            UI_Image_Arrow[i].color = new Color(255, 255, 255);


        }

    }

    public int AnimID = 0;

    public GameObject[] EffectPrefabGroup;

    public Transform[]  EffectPosGroup;

    // Animation Change function
    private void ChangeAnim()
    {
        switch (CurrentID)
        {
            case 1:
                if (AnimID == CurrentID)
                    return;

                m_Anim.Play("Idle");
                Text_Anim.text = "Idle";
                AnimID = CurrentID;
                break;
            case 2:
                if (AnimID == CurrentID)
                    return;

                m_Anim.Play("Run");
                Text_Anim.text = "Run_Smoke";
                AnimID = CurrentID;


     
                break;
            case 3:
                if (AnimID == CurrentID)
                    return;

                m_Anim.Play("Hit");
                Text_Anim.text = "Hit_White_Small";
                AnimID = CurrentID;

                

                break;
            case 4:
                if (AnimID == CurrentID)
                    return;

                m_Anim.Play("Hit M");
                Text_Anim.text = "Hit_White_Medium";
                AnimID = CurrentID;



                break;
            case 5:
                if (AnimID == CurrentID)
                    return;

                m_Anim.Play("Hit L");
                Text_Anim.text = "Hit_White_Large";
                AnimID = CurrentID;



                break;
            case 6:
                if (AnimID == CurrentID)
                    return;

                m_Anim.Play("Hit_Arrow");
                Text_Anim.text = "Arrow_Hit";
                AnimID = CurrentID;



                break;
            case 7:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Die");
                Text_Anim.text = "Die";
                AnimID = CurrentID;

          
                break;
            case 8:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Jump");
                Text_Anim.text = "Jump";
                AnimID = CurrentID;
                
                break;
            case 9:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Punch");
                Text_Anim.text = "Punch";
                AnimID = CurrentID;
             
                break;
            case 10:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Spell");
                Text_Anim.text = "Spell";
                AnimID = CurrentID;
         
                break;
            case 11:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Casting");
                Text_Anim.text = "Casting";
                AnimID = CurrentID;
              
                break;
            case 12:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Throw_Grenade");
                Text_Anim.text = "Explosion_S";
                AnimID = CurrentID;
              
                break;
            case 13:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Throw_Grenade_2");
                Text_Anim.text = "Explosion_M";
                AnimID = CurrentID;

                break;
            case 14:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Throw_Magic");
                Text_Anim.text = "Magic_Smoke";
                AnimID = CurrentID;

                break;
            case 15:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Attack_Dagger");
                Text_Anim.text = "Attack_Dagger";
                AnimID = CurrentID;
             
                break;
            case 16:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Attack2_Dagger");
                Text_Anim.text = "Attack2_Dagger";
              
                AnimID = CurrentID;
                break;
            case 17:
              
                if (AnimID == CurrentID)
                    return;

                m_Anim.Play("Attack_Sword");
                Text_Anim.text = "Attack_Sword";
                AnimID = CurrentID;
              
                break;
            case 18:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Attack2_Sword");
                Text_Anim.text = "Attack2_Sword";

                AnimID = CurrentID;
       
                break;
            case 19:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Attack_Spear");
                Text_Anim.text = "Attack_Spear";
                AnimID = CurrentID;
           
                break;
            case 20:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Attack2_Spear");
                Text_Anim.text = "Attack2_Spear";
                AnimID = CurrentID;
        
                break;
            case 21:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Guard_Hit");
                Text_Anim.text = "Guard_Hit";
                AnimID = CurrentID;
              
                break;
            case 22:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("GunShot");
                Text_Anim.text = "GunShot";
                AnimID = CurrentID;
               
                break;
            case 23:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("GunShot 1");
                Text_Anim.text = "GunShot 1";
                AnimID = CurrentID;
            
                break;

            case 24:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("GunShot 2");
                Text_Anim.text = "GunShot 2";
                AnimID = CurrentID;
            
                break;

            case 25:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Stun");
                Text_Anim.text = "Stun";
                AnimID = CurrentID;

                break;


            case 26:
                if (AnimID == CurrentID)
                    return;
                m_Anim.Play("Confusion");
                Text_Anim.text = "Confusion";
                AnimID = CurrentID;

              
                break;


        }


    }


    // effect Event

    public List<GameObject> EffectTmpList = new List<GameObject>();

    public void InitEffect()
    {


       for (int i = 0; i < EffectTmpList.Count; i++)
       {
           
            Destroy(EffectTmpList[i]);
       }

        EffectTmpList.Clear();
    }

    public void CreateEffect(GameObject EffectPrefab,  Transform Pos)
    {

        GameObject tmpobj = Instantiate(EffectPrefab, Pos);
        tmpobj.transform.localPosition = new Vector3(0, 0, -10);
        tmpobj.AddComponent<DestroyEffect>();
        EffectTmpList.Add(tmpobj);
    }


    public void Run_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[0], EffectPosGroup[0]);
    }

    public void hit_S_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[1], EffectPosGroup[1]);
    }
    public void hit_M_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[2], EffectPosGroup[2]);
    }
    public void hit_L_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[3], EffectPosGroup[3]);
    }
    public void hit_Arrow_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[4], EffectPosGroup[4]);
    }

    public void KnockBack_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[5], EffectPosGroup[5]);
    }
    public void Jump_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[6], EffectPosGroup[6]);
    }

    public void Punch_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[7], EffectPosGroup[7]);
    }

    public void Spell_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[8], EffectPosGroup[8]);
    }

    public void MagicCasting_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[9], EffectPosGroup[9]);
    }
    public void Explosion_S_AnimEvent()
    {
        GameObject tmpobj = Instantiate(EffectPrefabGroup[10]);
        tmpobj.transform.position = EffectPosGroup[10].transform.position;
        tmpobj.AddComponent<DestroyEffect>();
        EffectTmpList.Add(tmpobj);

    }
    public void Explosion_L_AnimEvent()
    {
        GameObject tmpobj = Instantiate(EffectPrefabGroup[11]);
        tmpobj.transform.position = EffectPosGroup[11].transform.position;
        tmpobj.AddComponent<DestroyEffect>();
        EffectTmpList.Add(tmpobj);

    }
    public void Explosion_Magic_AnimEvent()
    {
        GameObject tmpobj = Instantiate(EffectPrefabGroup[12]);
        tmpobj.transform.position = EffectPosGroup[12].transform.position;
        tmpobj.AddComponent<DestroyEffect>();
        EffectTmpList.Add(tmpobj);

    }
    public void Sword_Cut_Small_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[13], EffectPosGroup[13]);
    }
    public void Sword_Thrust_Small_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[14], EffectPosGroup[14]);
    }
    public void Sword_Cut_Medium_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[15], EffectPosGroup[15]);
    }
    public void Sword_Thrust_Medium_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[16], EffectPosGroup[16]);
    }
    public void Sword_Cut_L_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[17], EffectPosGroup[17]);
    }
    public void Sword_Thrust_L_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[18], EffectPosGroup[18]);
    }
    public void Guard_hit_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[19], EffectPosGroup[19]);
    }
    public void Gun_Shoot_AnimEvent()
    {

        GameObject tmpobj = Instantiate(EffectPrefabGroup[20]);
        tmpobj.transform.position = EffectPosGroup[20].transform.position;
        tmpobj.AddComponent<DestroyEffect>();
        EffectTmpList.Add(tmpobj);
    }
    public void Gun_Shoot1_AnimEvent()
    {
        GameObject tmpobj = Instantiate(EffectPrefabGroup[21]);
        tmpobj.transform.position = EffectPosGroup[21].transform.position;
        tmpobj.AddComponent<DestroyEffect>();
        EffectTmpList.Add(tmpobj);
    }
    public void Gun_Shoot2_AnimEvent()
    {
        
        GameObject tmpobj = Instantiate(EffectPrefabGroup[22]);
        tmpobj.transform.position = EffectPosGroup[22].transform.position;
        tmpobj.AddComponent<DestroyEffect>();
        EffectTmpList.Add(tmpobj);

    
    }


    public void Stun_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[23], EffectPosGroup[23]);
    }
    public void Confusion_AnimEvent()
    {
        CreateEffect(EffectPrefabGroup[24], EffectPosGroup[24]);
    }



}
