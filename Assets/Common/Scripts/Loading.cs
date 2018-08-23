using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

public class Loading : MonoBehaviour {
    [SerializeField]
    private float minTime = 1.5f; //로딩씬이 유지되는 최소 시간
    [SerializeField]
    private Slider sliderbar = null; //하단 슬라이더바
    [SerializeField]
    private Text Tip = null; //상단 팁 텍스트
    //[SerializeField]
    //private Transform wheel = null;//중앙 회전 이미지
    [SerializeField]
    private Image image = null;
    [SerializeField]
    private string[] Tips = null;//팁 텍스트 모음
    [SerializeField]
    private Sprite[] cSprite;
    [SerializeField]
    private RuntimeAnimatorController[] anim;
    private int selectChar;
    private bool isLoad = false; //중복 실행 방지
    private float timer = 0; //시간 측정
    //private Vector3 wheeleuler;//중앙 회전 이미지 오일러 각도측정용
    AsyncOperation async;

    private void Awake()
    {
        selectChar = (int)Random.RandomRange(0, cSprite.Length);
        image.sprite = cSprite[selectChar];
        image.GetComponent<Animator>().runtimeAnimatorController = anim[selectChar];
        Debug.Log(selectChar);
        if (Tips.Length <= 0)
            return;
        int loc = UnityEngine.Random.Range(0, Tips.Length - 1); //배열내에서 무작위로 인덱스를 얻는다.
        StringBuilder sb = new StringBuilder("팁 : ");
        sb.Append(Tips[loc]); //배열 내 무작위 요소를 출력한다.
        Tip.text = sb.ToString();
    }
    void Start()
    {
        StartCoroutine("LoadingScene");
    }
    private void Update()
    {
        if (async == null)
            return;
        //wheeleuler = wheel.rotation.eulerAngles;
        //wheeleuler.z -= 3f;
        //wheel.rotation = Quaternion.Euler(wheeleuler);
        timer += Time.deltaTime;
    }
    IEnumerator LoadingScene()
    {
        if (isLoad == false)
        {
            isLoad = true;
            async = SceneManager.LoadSceneAsync(SceneDataManager.NextScene);
            //async = SceneManager.LoadSceneAsync("SelectScene"); // 디버깅용
            SceneDataManager.NextScene = null; //다시 다음씬이 지정될때까지 null로 둔다.
            async.allowSceneActivation = false; //다음 씬의 준비가 완료되더라도 바로 로딩되는걸 막는다.
            while (!async.isDone)
            {
                if (async.progress == 0.9f)
                {
                    async.allowSceneActivation = true;
                    //Tip.text = "화면을 눌러주세요.";
                    //if (Input.anyKey)
                    //    async.allowSceneActivation = true;
                    sliderbar.value = 1;
                }
                else
                    sliderbar.value = async.progress;
                yield return null;
            }
        }
    }
}
