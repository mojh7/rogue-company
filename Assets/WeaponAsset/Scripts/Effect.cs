using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 최대한 몬스터 충돌 처리 및 기타 처리가 없이
 * 단순히 화면에 보여지기 위한 effect class 가 되려함
 * effect 여러 종류가 있겠지만 일단
 * 1. Sprite animation으로 되있는 effect
 * 
 * 있어야 될 기능, 것
 *  - 생성 및 생성 후 n초 뒤에 삭제(메모리풀 회수)
 *  
 * 애니메이션 구분 int형 id 혹은 string ? 
 */ 

/// <summary>
/// Effect Class
/// </summary>
public class Effect : MonoBehaviour {

    private EffectInfo info;
    private Transform objTransform;
    private Animator animator;
    [SerializeField]
    private GameObject particleSystemObject;    // 파티클 시스템을 포함한 Effect의 자식 오브젝트
    private Vector3 scaleVector;
    

    void Awake()
    {
        objTransform = GetComponent<Transform>();
        animator = GetComponentInChildren<Animator>();
        scaleVector = new Vector3(1f, 1f, 1f);
    }
    
    public void Init(int id)
    {
        info = DataStore.Instance.GetEffectInfo(id);
        //info = DataStore.Instance.GetEffectInfo(Random.Range(0, 7));
        scaleVector.x = info.scaleX;
        scaleVector.y = info.scaleY;
        objTransform.localScale = scaleVector;
        if(info.particleActive == false)
        {
            particleSystemObject.SetActive(false);
        }
        animator.SetTrigger(info.animationName);
        //
        // 일단 임시로 생성 삭제고 오브젝트 풀로 옮겨야 됨.
        // 생성 될 때 처리 및 delete 함수 n초뒤에 실행
        Invoke("DeleteEffect", info.lifeTime);
    }

    public void DeleteEffect()
    {
        Destroy(gameObject);
    }
}