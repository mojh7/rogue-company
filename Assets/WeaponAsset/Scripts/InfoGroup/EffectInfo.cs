using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectInfo", menuName = "GameData/EffectInfo", order = 5)]
public class EffectInfo : ScriptableObject
{
    [Tooltip("적용하거나 쓰이는 곳, 사용하는 사람, 간단한 설명 등등 이것 저것 메모할 공간")]
    [SerializeField]
    [TextArea(3, 100)] private string memo;

    public float lifeTime;          // Effect가 생성되있는 시간
    public string animationName;    // 애니메이션 이름 effect0, effect1, effect2 등등등
    // 크기
    public float scaleX;
    public float scaleY;

    public bool particleActive; // partlcle systme on / off

    public EffectInfo()
    {
        scaleX = 1.0f;
        scaleY = 1.0f;
    }

}
