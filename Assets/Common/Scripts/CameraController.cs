using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 연출에 따라서 지속적으로 하면 어지러운 것들이 좀 있어서
// 무기 사용시 넉백같은 느낌으로 카메라 흔든것도 있는데 가만히 있을 때 쓰면 느껴지는데
// 움직이면서 카메라 따라오면서 쉐이크하면 느낌이 안나고 어지러움증 유발함
// 
// 봐가지고 스킬이나 기타 큼직큼직한 연출에서 써야할 듯.

public class CameraController : MonoBehaviourSingleton<CameraController> {

    public enum CameraShakeType { NOTSHAKE, RANDOM, WEAPON_REVERSE_DIRECTION, UP, DOWN, UP_DOWN, RIGHT_LEFT}
    Player m_player;
    Transform cameraTransform;
    //Vector2 targetPos = Vector2.zero;
    Vector2 m_velocity = Vector2.zero;
    Vector2 weaponReverseDirection;
    float m_shakeTime, m_shakeAmount;
    CameraShakeType m_cameraShakeType;
    [SerializeField]
    float m_cameraDepth = -1;
    //bool m_findPlayer = true;
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
    //private void FixedUpdate()
    //{
    //    if (m_findPlayer)
    //        FindPlayer();
    //    else
    //        FindOther(targetPos);
    //    if (m_shakeTime > 0)
    //        ShakeCamera(m_shakeAmount);
    //}
    #region func
    void Focus()
    {
        if (m_player == null)
            m_player = PlayerManager.Instance.GetPlayer();
        Vector3 v = m_player.GetPosition();
        cameraTransform.position = new Vector3(v.x, v.y, m_cameraDepth);
    }
    void FindOther(Vector2 _targetPos)
    {
        Vector2 temp = Vector2.SmoothDamp(cameraTransform.position, _targetPos, ref m_velocity, 5, 0.5f, .45f);
        cameraTransform.position = new Vector3(temp.x, temp.y, m_cameraDepth);
    }
    void FindPlayer()
    {
        if (m_player == null)
        {
            m_player = PlayerManager.Instance.GetPlayer();
            cameraTransform.position = new Vector2(m_player.transform.position.x, m_player.transform.position.y);
            return;
        }
        Vector2 targetPos = new Vector2(m_player.transform.position.x, m_player.transform.position.y)
            + Vector2.Scale((Vector2)m_player.GetDirVector(), new Vector3(1f, 1f));

        //TODO : Player타겟으로 돌아올 때의 속도? 를 멀 수록 빨리 온다던가 가속, 감속 같은 처리를 해야할 듯 지속적으로 개선

        //Vector2 temp = Vector2.SmoothDamp(transform.position, targetPos, ref m_velocity, Random.Range(1f, 3.5f), 0.5f, Random.Range(.35f, .55f));
        Vector2 temp = Vector2.SmoothDamp(cameraTransform.position, targetPos, ref m_velocity, 3, 0.7f, .45f);
        //Vector2 temp = Vector2.SmoothDamp(transform.position, targetPos, ref m_velocity, 5, 0.5f, .45f);
        cameraTransform.position = new Vector3(temp.x, temp.y, m_cameraDepth);
    }
    public void Shake(float _amount, float _time, CameraShakeType _cameraShakeType, Vector2 _dir)
    {
        m_cameraShakeType = _cameraShakeType;
        m_shakeTime = _time;
        m_shakeAmount = _amount;
        weaponReverseDirection = -_dir;
    }
    void ShakeCamera(float _amount)
    {
        //TODO : sin, cos sin^2, cos^2 등을 이용해서 가속, 감속? 속도 커브를 좀 다양하게 해야 할듯, 지속적으로 개선
        //Focus();
        switch (m_cameraShakeType)
        {
            case CameraShakeType.RANDOM:
                shakePos = Random.insideUnitCircle * _amount;
                break;
            case CameraShakeType.WEAPON_REVERSE_DIRECTION:
                shakePos = _amount * weaponReverseDirection;
                break;
            case CameraShakeType.UP:
                shakePos = Random.Range(0, _amount) * Vector2.up ;
                break;
            case CameraShakeType.DOWN:
                shakePos = Vector2.down * Random.Range(0.5f, 1.0f) * _amount;
                break;
            case CameraShakeType.UP_DOWN:
                shakePos = Random.Range(-_amount, _amount) * Vector2.up;
                break;
            case CameraShakeType.RIGHT_LEFT:
                shakePos = Random.Range(-_amount, _amount) * Vector2.right;
                break;
            default:
                return;
        }
        cameraTransform.position = new Vector3(cameraTransform.position.x + shakePos.x, cameraTransform.position.y + shakePos.y, m_cameraDepth);
        m_shakeTime -= Time.fixedDeltaTime;
    }
    #endregion
}
