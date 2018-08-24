using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 연출에 따라서 지속적으로 하면 어지러운 것들이 좀 있어서
// 무기 사용시 넉백같은 느낌으로 카메라 흔든것도 있는데 가만히 있을 때 쓰면 느껴지는데
// 움직이면서 카메라 따라오면서 쉐이크하면 느낌이 안나고 어지러움증 유발함
// 
// 봐가지고 스킬이나 기타 큼직큼직한 연출에서 써야할 듯.

public class CameraController : MonoBehaviourSingleton<CameraController> {

    Player m_player;
    Transform cameraTransform;
    Vector2 m_velocity = Vector2.zero;
    float m_shakeTime, m_shakeAmount;
    [SerializeField]
    float m_cameraDepth = -1;
    Vector2 shakePos;
    Vector3 zeroPos;
    private void Start()
    {
        cameraTransform = this.transform;
        zeroPos = new Vector3(0, 0, m_cameraDepth);
    }
    private void Update()
    {
        if (m_shakeTime > 0)
            ShakeCamera(m_shakeAmount);
        else
        {
            cameraTransform.localPosition = zeroPos;
        }
    }
    #region func
    void FindOther(Vector2 _targetPos)
    {
        Vector2 temp = Vector2.SmoothDamp(cameraTransform.position, _targetPos, ref m_velocity, 5, 0.5f, .45f);
        cameraTransform.position = new Vector3(temp.x, temp.y, m_cameraDepth);
    }

    public void Shake(float amout, float time)
    {
        m_shakeTime = time;
        m_shakeAmount = amout;
    }

    void ShakeCamera(float _amount)
    {
        shakePos = Random.insideUnitCircle * _amount;

        cameraTransform.position = new Vector3(cameraTransform.position.x + shakePos.x, cameraTransform.position.y + shakePos.y, m_cameraDepth);
        m_shakeTime -= Time.fixedDeltaTime;
    }
    #endregion
}
