using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectInfo", menuName = "GameData/EffectInfo", order = 5)]
public class EffectInfo : ScriptableObject {

    public float lifeTime;          // Effect가 생성되있는 시간
    public string animationName;    // 애니메이션 이름 effect0, effect1, effect2 등등등
    // 크기
    public float scaleX;
    public float scaleY;

    public bool particleActive; // partlcle systme on / off

    [Tooltip("이 정보를 쓰고 있는 사람, 쓰이는 곳, 간단한 설명 등등 이것 저것 메모할 것들 적는 곳")]
    [SerializeField]
    [TextArea(3, 100)]
    private string memo;

    public EffectInfo()
    {
        scaleX = 1.0f;
        scaleY = 1.0f;
    }

}
