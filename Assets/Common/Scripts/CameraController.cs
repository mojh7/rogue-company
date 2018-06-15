using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviourSingleton<CameraController> {

    Player m_player;
    Vector2 targetPos = Vector2.zero;
    Vector2 m_velocity = Vector2.zero;
    float m_shakeTime, m_shakeAmount;
    float m_cameraDepth = -1;
    bool m_findPlayer = true;

    private void FixedUpdate()
    {
        if (m_findPlayer)
            FindPlayer();
        else
            FindOther(targetPos);
        if (m_shakeTime > 0)
            ShakeCamera(m_shakeAmount);
    }
    #region func
    void FindOther(Vector2 _targetPos)
    {
        Vector2 temp = Vector2.SmoothDamp(transform.position, _targetPos, ref m_velocity, 5, 0.5f, .45f);
        transform.position = new Vector3(temp.x, temp.y, m_cameraDepth);
    }
    void FindPlayer()
    {
        if (m_player == null)
        {
            m_player = PlayerManager.Instance.GetPlayer();
            transform.position = new Vector2(m_player.transform.position.x, m_player.transform.position.y);
            return;
        }
        Vector2 targetPos = new Vector2(m_player.transform.position.x, m_player.transform.position.y) 
            + Vector2.Scale((Vector2)m_player.GetDirVector(), new Vector3(1f, 1f));
        Vector2 temp = Vector2.SmoothDamp(transform.position, targetPos, ref m_velocity, 5, 0.5f, .45f);
        transform.position = new Vector3(temp.x, temp.y, m_cameraDepth);
    }
    public void Shake(float _amount, float _time)
    {
        m_shakeTime = _time;
        m_shakeAmount = _amount;
    }
    void ShakeCamera(float _amount)
    {
        Vector2 shakePos = Random.insideUnitCircle * _amount;

        transform.position = new Vector3(transform.position.x + shakePos.x, transform.position.y + shakePos.y, m_cameraDepth);
        m_shakeTime -= Time.deltaTime;
    }
    #endregion
}
