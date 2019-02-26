using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticleAssistance : MonoBehaviour
{
    [SerializeField]
    private TrailRenderer trailRenderer;
    [SerializeField]
    private AudioSource audioSource;

    private void Awake()
    {
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void OnEnable()
    {
        if(null != trailRenderer)
        {
            trailRenderer.Clear();
        }

        if (null != audioSource)
        {
            audioSource.volume = AudioManager.Instance.GetSFXVolume();
        }
    }
}
