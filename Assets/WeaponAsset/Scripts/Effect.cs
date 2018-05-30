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

    private Coroutine deleteOnLifeTime;

    private bool active;

    void Awake()
    {
        objTransform = GetComponent<Transform>();
        animator = GetComponentInChildren<Animator>();
        scaleVector = new Vector3(1f, 1f, 1f);
        active = false;
    }

    // 전체 회수용.
    private void OnDisable()
    {
        if(true == active)
        {
            DeleteEffect();
        }
    }

    public void Init(int id, Vector3 pos)
    {
        info = DataStore.Instance.GetEffectInfo(id);
        //info = DataStore.Instance.GetEffectInfo(Random.Range(0, 7));
        scaleVector.x = info.scaleX;
        scaleVector.y = info.scaleY;
        objTransform.localScale = scaleVector;
        transform.position = pos;

        if(info.particleActive == false)
        {
            particleSystemObject.SetActive(false);
        }
        animator.SetTrigger(info.animationName);

        deleteOnLifeTime = StartCoroutine("DeleteOnLifeTime");

        active = true;
    }

  

    public void DeleteEffect()
    {
        active = false;
        if (null != deleteOnLifeTime) 
        {
            StopCoroutine("DeleteOnLifeTime");
        }
        ObjectPoolManager.Instance.DeleteEffect(gameObject);
    }

    // delete 함수 n초뒤에 실행
    private IEnumerator DeleteOnLifeTime()
    {
        float time = 0;
        while (true)
        {
            if (time >= info.lifeTime)
            {
                DeleteEffect();
            }
            time += Time.fixedDeltaTime;
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}