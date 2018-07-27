using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectParticle : MonoBehaviour {

    [SerializeField]
    ParticleSystem particleSystem;

    public void Active()
    {
        particleSystem.Play();
    }
}
