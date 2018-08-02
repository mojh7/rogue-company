using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;

public class ParticlePool : MonoBehaviourSingleton<ParticlePool>
{
    [SerializeField]
    bool willGrow;
    [SerializeField]
    int objectNum = 10;
    Dictionary<string, List<ParticleSystem>> poolsDictionary;
    [SerializeField]
    ParticleSystem[] particles;

    private void Awake()
    {
        poolsDictionary = new Dictionary<string, List<ParticleSystem>>();

        for (int i=0;i<particles.Length;i++)
        {
            if(!poolsDictionary.ContainsKey(particles[i].name))
            {
                poolsDictionary[particles[i].name] = new List<ParticleSystem>(10);
                for(int j=0;j< objectNum;j++)
                {
                    ParticleSystem particle = GameObject.Instantiate(particles[i], new Vector3(0, 0, 0), new Quaternion()) as ParticleSystem;
                    particle.hideFlags = HideFlags.HideInHierarchy;
                    particle.gameObject.SetActive(false);
                    poolsDictionary[particles[i].name].Add(particle);

                }
            }
        }
    }

    public ParticleSystem getAvailabeParticle(string str)
    {
        if (!poolsDictionary.ContainsKey(str))
            return null;
        List<ParticleSystem> particles = poolsDictionary[str];
        if (particles == null)
            return null;
        for (int i = 0; i < particles.Count; i++)
        {
            if (!particles[i].gameObject.activeInHierarchy)
            {
                particles[i].gameObject.SetActive(true);
                return particles[i];
            }
        }

        if (willGrow) // 오브젝트 더 생성.
        {
            ParticleSystem obj = GameObject.Instantiate(particles[0], new Vector3(0, 0, 0), new Quaternion()) as ParticleSystem;
            //obj.hideFlags = HideFlags.HideInHierarchy;
            obj.gameObject.SetActive(false);
            obj.transform.parent = transform;
            particles.Add(obj);
            return obj;
        }

        return null;
    }
}
